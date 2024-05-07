using UnityEngine;
using Firebase.Analytics;

internal class AnalyticsManager : MonoBehaviour
{
    #region Variables

    #endregion


    #region Unity Callbacks

    private void Awake()
    {

    }

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            }
            else
            {

            }
        });
    }

    #endregion


    #region Other Methods

    public void AnalyticsTest()
    {
        FirebaseAnalytics.LogEvent("testEvent","testEventName",5);
    }

    #endregion




}
