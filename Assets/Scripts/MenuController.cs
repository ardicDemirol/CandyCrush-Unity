using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    public Button[] LevelButtons;
    public TextMeshProUGUI[] MaxScores;
    public GameObject ScoreboardPanel;
    public GameObject LevelSelectPanel;
    public Transform ScoreboardContent;
    public GameObject ScoreElement;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null) Destroy(this);
    }

    public void LevelSelectScreen()
    {
        LevelSelectPanel.SetActive(true);
        ScoreboardPanel.SetActive(false);
    }

    public void ScoreboardScreen()
    {
        LevelSelectPanel.SetActive(false);
        ScoreboardPanel.SetActive(true);
    }

}
