using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using Core;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    public DatabaseReference DBreference;

    private void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
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
        Debug.Log("Setting up Firebase Auth");
        _auth = BackendManager.Instance.Auth;
        _user = BackendManager.Instance.User;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveDataButton<T>(string key, T value)
    {
        StartCoroutine(UpdateDatabaseValue(key, value));

    }
    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new() { DisplayName = _username };

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


    private IEnumerator UpdateDatabaseValue<T>(string key, T value)
    {
        Debug.Log(value);
        //Set the currently logged in user data in the database
        Task DBTask = DBreference.Child("users").Child(_user.UserId).Child(key).SetValueAsync(value);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log($"{key} is now updated with value: {value}");
        }
    }

    private IEnumerator LoadUserData(string key)
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(_user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            //xpField.text = "0";
            //killsField.text = "0";
            //deathsField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //xpField.text = snapshot.Child("xp").Value.ToString();
            //killsField.text = snapshot.Child("kills").Value.ToString();
            //deathsField.text = snapshot.Child("deaths").Value.ToString();
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("kills").GetValueAsync();

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
