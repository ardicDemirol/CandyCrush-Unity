using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using System;

public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase")]
    [SerializeField] private DependencyStatus dependencyStatus;
    private FirebaseUser _user;
    private DatabaseReference _dbReference;

    private int _totalScore;
    private string _level0Score;
    private string _level1Score;
    private string _level2Score;
    private string _level3Score;
    private string _level4Score;
    private string _totalScoreText;
    private string _username;

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

    public void SaveData<T>(string key, T value)
    {
        StartCoroutine(SaveMaxScore(key, value));
    }
    public void SaveLevelIndex<T>(T value)
    {
        StartCoroutine(SaveLastSuccessLevelIndex(value));
    }
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }
    public void StartLoadUserData()
    {
        StartCoroutine(LoadUserData());
        StartCoroutine(LoadLastSuccessLevelIndex());
        StartCoroutine(UpdateUsername(_user.DisplayName));
    }


    private IEnumerator UpdateUsernameAuth(string username)
    {
        UserProfile profile = new() { DisplayName = username };
        Task ProfileTask = _user.UpdateUserProfileAsync(profile);
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

    private IEnumerator UpdateUsername(string username)
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


    private IEnumerator SaveMaxScore<T>(string key, T value)
    {
        Task<DataSnapshot> DBTaskUserIDLoad = _dbReference.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(() => DBTaskUserIDLoad.IsCompleted);

        Task<DataSnapshot> DBTasUserIDChildLoad = _dbReference.Child("users").Child(_user.UserId).Child(key).GetValueAsync();
        yield return new WaitUntil(() => DBTasUserIDChildLoad.IsCompleted);

        DataSnapshot snapshotUserID = DBTasUserIDChildLoad.Result;
        if (Convert.ToInt32(value) > Convert.ToInt32(snapshotUserID.Value))
        {
            Task DBTask = _dbReference.Child("users").Child(_user.UserId).Child(key).SetValueAsync(value);
            Debug.Log(value);
            yield return new WaitUntil(() => DBTask.IsCompleted);

            Task<DataSnapshot> DBTaskUserIDLoad2 = _dbReference.Child("users").Child(_user.UserId).GetValueAsync();
            yield return new WaitUntil(() => DBTaskUserIDLoad2.IsCompleted);


            DataSnapshot snapshotUsers = DBTaskUserIDLoad2.Result;
            if (snapshotUsers.HasChild("level0Score"))
            {
                _level0Score = snapshotUsers.Child("level0Score").Value.ToString();
                _totalScore += Convert.ToInt32(_level0Score);
            }
            if (snapshotUsers.HasChild("level1Score"))
            {
                _level1Score = snapshotUsers.Child("level1Score").Value.ToString();
                _totalScore += Convert.ToInt32(_level1Score);
            }
            if (snapshotUsers.HasChild("level2Score"))
            {
                _level2Score = snapshotUsers.Child("level2Score").Value.ToString();
                _totalScore += Convert.ToInt32(_level2Score);
            }
            if (snapshotUsers.HasChild("level3Score"))
            {
                _level3Score = snapshotUsers.Child("level3Score").Value.ToString();
                _totalScore += Convert.ToInt32(_level3Score);
            }
            if (snapshotUsers.HasChild("level4Score"))
            {
                _level4Score = snapshotUsers.Child("level4Score").Value.ToString();
                _totalScore += Convert.ToInt32(_level4Score);
            }

            Task DBTaskTotalScore = _dbReference.Child("users").Child(_user.UserId).Child("totalScore").SetValueAsync(_totalScore);
            yield return new WaitUntil(() => DBTaskTotalScore.IsCompleted);

        }
    }

    private IEnumerator SaveLastSuccessLevelIndex<T>(T value)
    {
        Task DBTaskLastLevelIndex = _dbReference.Child("users").Child(_user.UserId).Child("lastLevelIndex").SetValueAsync(value);
        yield return new WaitUntil(() => DBTaskLastLevelIndex.IsCompleted);
    }

    private IEnumerator LoadUserData()
    {
        Task<DataSnapshot> DBTask = _dbReference.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            for (int i = 0; i < FIVE; i++)
            {
                if (snapshot.Child($"level{i}Score").Value == null)
                {
                    MenuController.Instance.MaxScores[i].text = "0";
                    Task DBCreateTask = _dbReference.Child("users").Child(_user.UserId).Child($"level{i}Score").SetValueAsync(0);
                    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
                    continue;
                }
                MenuController.Instance.MaxScores[i].text = snapshot.Child($"level{i}Score").Value.ToString();
            }
        }
    }

    private IEnumerator LoadLastSuccessLevelIndex()
    {
        Task<DataSnapshot> DBTaskLastLevelIndex = _dbReference.Child("users").Child(_user.UserId).Child("lastLevelIndex").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTaskLastLevelIndex.IsCompleted);

        DataSnapshot snapshot = DBTaskLastLevelIndex.Result;

        if (snapshot.Value == null)
        {
            Task DBCreateTask = _dbReference.Child("users").Child(_user.UserId).Child("lastLevelIndex").SetValueAsync(0);
            yield return new WaitUntil(predicate: () => DBCreateTask.IsCompleted);
        }

        Task<DataSnapshot> DBTaskLastLevelIndex2 = _dbReference.Child("users").Child(_user.UserId).Child("lastLevelIndex").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTaskLastLevelIndex2.IsCompleted);
        DataSnapshot snapshot2 = DBTaskLastLevelIndex2.Result;
        for (int i = 0; i <= int.Parse(snapshot2.Value.ToString()); i++)
        {
            MenuController.Instance.LevelButtons[i].interactable = true;
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        Task<DataSnapshot> DBTask2 = _dbReference.Child("users").OrderByChild("totalScore").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

        if (DBTask2.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask2.Result;

            foreach (Transform child in MenuController.Instance.ScoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {

                if (childSnapshot.HasChild("username"))
                {
                    _username = childSnapshot.Child("username").Value.ToString();
                }
                if (childSnapshot.HasChild("level0Score"))
                {
                    _level0Score = childSnapshot.Child("level0Score").Value.ToString();
                }
                if (childSnapshot.HasChild("level1Score"))
                {
                    _level1Score = childSnapshot.Child("level1Score").Value.ToString();
                }
                if (childSnapshot.HasChild("level2Score"))
                {
                    _level2Score = childSnapshot.Child("level2Score").Value.ToString();
                }
                if (childSnapshot.HasChild("level3Score"))
                {
                    _level3Score = childSnapshot.Child("level3Score").Value.ToString();
                }
                if (childSnapshot.HasChild("level4Score"))
                {
                    _level4Score = childSnapshot.Child("level4Score").Value.ToString();
                }
                if (childSnapshot.HasChild("totalScore"))
                {
                    _totalScoreText = childSnapshot.Child("totalScore").Value.ToString();
                }
                GameObject scoreboardElement = Instantiate(MenuController.Instance.ScoreElement, MenuController.Instance.ScoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(_username, _level0Score, _level1Score, _level2Score, _level3Score, _level4Score, _totalScoreText);
            }
            MenuController.Instance.ScoreboardScreen();
        }
    }


    private void SubscribeEvents()
    {
        Signals.OnGetScore += SaveData;
        Signals.OnLevelCompleted += SaveLevelIndex;
    }

    private void UnsubscribeEvents()
    {
        Signals.OnGetScore -= SaveData;
        Signals.OnLevelCompleted -= SaveLevelIndex;
    }


}
