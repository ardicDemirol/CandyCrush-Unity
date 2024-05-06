using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{
    [SerializeField] private  TMP_Text usernameText;
    [SerializeField] private  TMP_Text level0ScoreText;
    [SerializeField] private  TMP_Text level1ScoreText;
    [SerializeField] private  TMP_Text level2ScoreText;
    [SerializeField] private  TMP_Text level3ScoreText;
    [SerializeField] private  TMP_Text level4ScoreText;
    [SerializeField] private  TMP_Text totalScoreText;

    public void NewScoreElement (string _username, string level0Score,string level1Score, string level2Score, string level3Score, string level4Score,string totalScore)
    {
        usernameText.text = _username;
        level0ScoreText.text = level0Score;
        level1ScoreText.text = level1Score;
        level2ScoreText.text = level2Score;
        level3ScoreText.text = level3Score;
        level4ScoreText.text = level4Score;
        totalScoreText.text = totalScore;
    }

}
