using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject UserDataPanel;
    public GameObject ScoreboardPanel;
    public GameObject LevelFinishPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }


    private void OnDisable()
    {
        UnsubscribeEvents();
    }


    public void ClearScreen()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        UserDataPanel.SetActive(false);
        ScoreboardPanel.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearScreen();
        LoginPanel.SetActive(true);
    }
    public void RegisterScreen()
    {
        ClearScreen();
        RegisterPanel.SetActive(true);
    }

    public void UserDataScreen()
    {
        ClearScreen();
        UserDataPanel.SetActive(true);
    }

    public void ScoreboardScreen()
    {
        ClearScreen();
        ScoreboardPanel.SetActive(true);
    }

    public void LevelFinishScreen()
    {
        LevelFinishPanel.SetActive(true);
    }

    private void SubscribeEvents()
    {

    }

    private void UnsubscribeEvents()
    {
        Signals.OnGameFinished -= ctx => LevelFinishPanel.SetActive(true);
    }
}
