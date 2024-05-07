using TMPro;
using UnityEngine;
using Tools;
using UnityEngine.UI;

namespace Core
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public int LevelIndex;
        public int MinScore;
        [SerializeField] private int _maxAllowedMove = 40;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI targetScoreText;
        [SerializeField] private TextMeshProUGUI moveText;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Vector2Int dimensions = new(10, 10);
        private int _score;
        private MatchableGrid _grid;

        [ContextMenu("ClearAndPopulate")]
        private void ClearAndPopulate()
        {
            _grid.ClearGrid();
            _grid.PopulateGrid();
        }
        [ContextMenu("Populate")]
        private void Populate()
        {
            _grid.PopulateGrid();
        }
        [ContextMenu("ClearGrid")]
        private void ClearGrid()
        {
            _grid.ClearGrid();
        }
        protected override void Awake()
        {
            base.Awake();
            _grid = (MatchableGrid)MatchableGrid.Instance;
            _grid.InitializeGrid(dimensions);
            _grid.PopulateGrid();
            scoreText.text = _score.ToString("D7");
            moveText.text = _maxAllowedMove.ToString();
            targetScoreText.text = MinScore.ToString("D7");
        }
        public void IncreaseScore(int value)
        {
            _score += value;
            scoreText.text = _score.ToString("D7");
        }
        public bool CanMoveMatchables()
        {
            if (_maxAllowedMove < 1)
            {
                Signals.OnGameFinished?.Invoke(true);
                Signals.OnGetScore?.Invoke($"level{LevelIndex}Score", _score);
                if (_score > MinScore)
                {
                    Signals.OnLevelCompleted?.Invoke(LevelIndex+1);
                    mainMenuButton.gameObject.transform.parent.gameObject.SetActive(true);
                }
                return false;
            }
            return true;
        }
        public void DecreaseMove()
        {
            _maxAllowedMove--;
            moveText.text = _maxAllowedMove.ToString();
        }


    }
}

