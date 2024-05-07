using Core;
using System.Collections.Generic;
public class Match
{
    private List<Matchable> _matchableList;
    public int Count => _matchableList.Count;
    public bool Collectable => Count >= MIN_MATCH;

    public bool OriginExclusive { get => _originExclusive; set => _originExclusive = value; }
    public List<Matchable> MatchableList { get => _matchableList; set => _matchableList = value; }

    private bool _originExclusive;
    private MatchableGrid _grid;
    private MatchablePool _pool;
    private Matchable _originMatchable;
    private const int MIN_MATCH = 3;

    public Match(Matchable matchable)
    {
        _matchableList = new List<Matchable>();
        _originMatchable = matchable;
        _matchableList.Add(matchable);
        _grid = (MatchableGrid)MatchableGrid.Instance;
        _pool = (MatchablePool)MatchablePool.Instance;
    }
    private void CollectMatchPoint()
    {
        int addScore = _matchableList.Count;
        GameManager.Instance.IncreaseScore(addScore);
        ScorePointFX scorePointFX = ScorePointFXPool.Instance.GetObject();
        scorePointFX.PlayVFX(_originMatchable.transform.position, addScore, _originMatchable.Variant.color);
    }
    private bool TryTransform()
    {
        bool doTransfrom = true;
        foreach (Matchable matchable in _matchableList)
        {
            if (matchable.Variant.type != MatchableType.Normal) doTransfrom = false;
        }

        if (!doTransfrom) return false;

        bool isMixedOrientation = false;
        int minX = _matchableList[0].GridPosition.x;
        int maxX = _matchableList[0].GridPosition.x;
        int minY = _matchableList[0].GridPosition.y;
        int maxY = _matchableList[0].GridPosition.y;
        foreach (Matchable matchable in _matchableList)
        {
            int x = matchable.GridPosition.x;
            int y = matchable.GridPosition.y;
            if (x >= maxX) maxX = x;
            if (x <= minX) minX = x;
            if (y >= maxY) maxY = y;
            if (y <= minY) minY = y;
        }
        if (minX != maxX)
            if (minY != maxY) isMixedOrientation = true;

        if (isMixedOrientation)
        {
            MatchableColor color = _originMatchable.Variant.color;
            _originMatchable.SetVariant(_pool.GetVariant(color, MatchableType.AreaExplode));
            SoundManager.Instance.PlaySound(12);
        }
        else
        {
            MatchableColor color = _originMatchable.Variant.color;
            if (_matchableList.Count > 4)
            {
                _originMatchable.SetVariant(_pool.GetVariant(MatchableColor.None, MatchableType.ColorExplode));
                SoundManager.Instance.PlaySound(4);
            }
            else
            {
                if (minX == maxX)
                    _originMatchable.SetVariant(_pool.GetVariant(color, MatchableType.VerticalExplode));
                else if (minY == maxY)
                    _originMatchable.SetVariant(_pool.GetVariant(color, MatchableType.HorizontalExplode));
                SoundManager.Instance.PlaySound(8);
            }

        }
        return true;
    }
    public void AddMatchable(Matchable matchable)
    {
        if (!_matchableList.Contains(matchable))
        {
            _matchableList.Add(matchable);
        }
    }
    public Match Merge(Match matchToMerge)
    {
        foreach (Matchable mathableToAdd in matchToMerge._matchableList)
        {
            AddMatchable(mathableToAdd);
        }
        return this;
    }
    public void Resolve()
    {
        CollectMatchPoint();
        for (int i = 0; i < _matchableList.Count; i++)
        {
            Matchable matchable = _matchableList[i];
            if (matchable.Variant.type == MatchableType.AreaExplode)
            {
                _grid.TriggerAreaExplode(matchable, this);
            }
            else if (matchable.Variant.type == MatchableType.HorizontalExplode)
            {
                _grid.StartCoroutine(_grid.TriggerHorizontalExplode(matchable, this));
            }
            else if (matchable.Variant.type == MatchableType.VerticalExplode)
            {
                _grid.StartCoroutine(_grid.TriggerVerticalExplode(matchable, this));
            }
        }

        bool isTransformed = false;
        if (_matchableList.Count > 3)
        {
            isTransformed = TryTransform();
        }

        for (int i = 0; i < _matchableList.Count; i++)
        {
            Matchable matchable = _matchableList[i];
            if (matchable.IsMoving) continue;
            if (isTransformed && matchable == _originMatchable) continue;
            _grid.RemoveItemAt(matchable.GridPosition);
            _pool.ReturnObject(matchable);
            _grid.ColumnCoroutines[matchable.GridPosition.x] = _grid.StartCoroutine(_grid.CollapseRepopulateAndScanColumn(matchable.GridPosition.x));
        }
    }
    public override string ToString()
    {
        string s = "";
        s = $"Matchable Count: {_matchableList.Count}, \r\n";
        foreach (Matchable matchable in _matchableList)
        {
            s += "Matchable at " + matchable.GridPosition + ", Variant: " + matchable.Variant + ",\r\n";
        }
        return s;
    }
}
