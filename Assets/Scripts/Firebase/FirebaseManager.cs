using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using Core;
using System;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    private FirebaseUser _user;
    private DatabaseReference _dbReference;

    private int _maxCore;
    private const byte FIVE = 5;


    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }


    private void OnDisable()
    {
        UnsubscribeEvents();
    }





    private void InitializeFirebase()
    {
        _user = BackendManager.Instance.User;
        _dbReference = BackendManager.Instance.DBreference;
    }

    public void SaveDataButton<T>(string key, T value)
    {
        StartCoroutine(SaveMaxScoreToDatabase(key, value));
    }
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }
    public void StartLoadUserData()
    {
        StartCoroutine(LoadUserData());
        StartCoroutine(UpdateUsernameDatabase(_user.DisplayName));

    }

    private IEnumerator UpdateUsernameAuth(string username)
    {
        //Create a user profile and set the username
        UserProfile profile = new() { DisplayName = username };

        //Call the Firebase auth update user profile function passing the profile with the username
        Task ProfileTask = _user.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string username)
    {
        Task DBTask = _dbReference.Child("users").Child(_user.UserId).Child("username").SetValueAsync(username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }


    private IEnumerator SaveMaxScoreToDatabase<T>(string key, T value)
    {
        Task<DataSnapshot> DBTaskLoad = _dbReference.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(() => DBTaskLoad.IsCompleted);

        Task<DataSnapshot> DBTaskLoad2 = _dbReference.Child("users").Child(_user.UserId).Child(key).GetValueAsync();
        yield return new WaitUntil(() => DBTaskLoad2.IsCompleted);

        DataSnapshot snapshot = DBTaskLoad2.Result;


        if (Convert.ToInt32(value) > Convert.ToInt32(snapshot.Value))
        {
            Task DBTask = _dbReference.Child("users").Child(_user.UserId).Child(key).SetValueAsync(value);
            Debug.Log(value);
            yield return new WaitUntil(() => DBTask.IsCompleted);
        }
    }

    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = _dbReference.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            for (int i = 0; i < FIVE; i++)
            {
                if (snapshot.Child($"level{i}Score").Value == null)
                {
                    MenuController.Instance.MaxScores[i].text = "0";
                    continue;
                }
                MenuController.Instance.MaxScores[i].text = snapshot.Child($"level{i}Score").Value.ToString();
                Debug.Log($"Level {i} score: {snapshot.Child($"level{i}Score").Value}");
            }
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = _dbReference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            //foreach (Transform child in scoreboardContent.transform)
            //{
            //    Destroy(child.gameObject);
            //}

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //Instantiate new scoreboard elements
                //GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                //scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, kills, deaths, xp);
            }

            //Go to scoareboard screen
            UIManager.Instance.ScoreboardScreen();
        }
    }






    private void SubscribeEvents()
    {
        Signals.OnGetScore += SaveDataButton;
    }

    private void UnsubscribeEvents()
    {
        Signals.OnGetScore -= SaveDataButton;
    }

}
