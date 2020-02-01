using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{

    private GridManager m_GM;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        m_GM = (GridManager)target;
        if (GUILayout.Button("Generate grid"))
        {
            m_GM.GenerateGrid();
        }

        //if (GUILayout.Button("Clear grid"))
        //{
        //    m_GM.ClearGrid();
        //}

        if (GUILayout.Button("Load grid from world"))
        {
            m_GM.LoadGridFromWorld();
        }

        //if (GUILayout.Button("Regenerate grid"))
        //{
        //    m_GM.RegenerateGrid();
        //}


    }
}
#endif
public class GridManager : Singleton<GridManager>
{
    public GridTile TilePrefab;

    public int gridsize;

    public TileKey[] GridKeys;
    public Dictionary<TileKey, GridTile> Grid = new Dictionary<TileKey, GridTile>();

    //public NavMeshSurface NavMesh;

    #region Pathfinding
    private List<GridTile> m_ClosedList = new List<GridTile>();
    private List<GridTile> m_OpenList = new List<GridTile>();
    private List<GridTile> m_Path = new List<GridTile>();

    private List<GridTile> m_PathableUpgrades = new List<GridTile>();
    #endregion
    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
        if (transform.childCount == 0)
        {
            GenerateGrid();
        }
        else
        {
            LoadGridFromWorld();
        }
        //  NavMesh.BuildNavMesh();
    }


    private void Start()
    {
        // NavMesh.BuildNavMesh();
    }


    public void GenerateGrid()
    {
        ClearGrid();
        for (int i = 0; i < gridsize; i++)
        {
            for (int x = 0; x < gridsize; x++)
            {
                TileKey _newKey = new TileKey(x, i);

                GridTile _newTile = (GridTile)PrefabUtility.InstantiatePrefab(TilePrefab, transform);
                _newTile.Init(_newKey);
                Grid.Add(_newKey, _newTile);
                _newTile.name = _newKey.X + " " + _newKey.Z;
            }
        }


        foreach (KeyValuePair<TileKey, GridTile> item in Grid)
        {
            item.Value.SetSurroundingTiles(this);
        }

        // NavMesh.BuildNavMesh();
    }

    public void ClearGrid()
    {
        for (int _transformChild = transform.childCount - 1; _transformChild > -1; _transformChild--)
        {
            DestroyImmediate(transform.GetChild(_transformChild).gameObject);
        }
        Grid.Clear();
    }

    public void RegenerateGrid()
    {
        if (Grid.Count == 0)
        {
            GridTile[] _array = FindObjectsOfType<GridTile>();

        }
        GridKeys = new TileKey[Grid.Count];
        int _index = 0;
        foreach (var item in Grid)
        {
            GridKeys[_index] = item.Key;
            if (item.Value.Upgrade != null)
            {
                item.Value.Upgrade.transform.parent = null;
            }
            _index += 1;
        }
        ClearGrid();
        GenerateGrid();
        LoadGridFromWorld();
    }
    public void LoadGridFromWorld()
    {
        foreach (var item in Grid)
        {
            item.Value.ClearSurroundingTiles(this);
        }
        Grid.Clear();

        GridTile[] _array = FindObjectsOfType<GridTile>();
        UpgradeBase[] _upgrades = FindObjectsOfType<UpgradeBase>();

        // find closest tile to upgrade and assign

        for (int i = 0; i < _upgrades.Length; i++)
        {
            float _lowestDist = 10000;
            GridTile _closestTile = null;

            UpgradeBase _upgrade = _upgrades[i];

            // check if this upgrade has a parent tile
            if (_upgrade.ParentTile)
            {
                continue;
            }
            for (int tileIndex = 0; tileIndex < _array.Length; tileIndex++)
            {
                if (Mathf.Abs(Vector3.Distance(_upgrade.transform.position, _array[tileIndex].transform.position)) < _lowestDist)
                {
                    _lowestDist = Mathf.Abs(Vector3.Distance(_upgrade.transform.position, _array[tileIndex].transform.position));
                    _closestTile = _array[tileIndex];
                }
            }

            if (_closestTile != null)
            {
                _upgrade.ResetUpgrade();
                _closestTile.UpgradeTile(_upgrade);
            }
        }

        // inactive gridtiles should not be included in this fetch
        for (int i = 0; i < _array.Length; i++)
        {
            GridTile _tile = _array[i];
            TileKey _key = GetKeyFromVector(_tile.transform.position);
            Vector3 _position = Vector3.zero;
            _position.x = _key.X;
            _position.z = _key.Z;
            _tile.transform.parent = transform;
            _tile.name = _key.X + " " + _key.Z;
            _tile.Init(_key);
            Grid.Add(_key, _tile);
        }


        foreach (KeyValuePair<TileKey, GridTile> item in Grid)
        {
            item.Value.SetSurroundingTiles(this);
        }

        Debug.Log(Grid.Count);
    }

    public List<GridTile> GetPath(GridTile startTile, GridTile goalTile)
    {
        m_OpenList.Clear();
        m_ClosedList.Clear();
        m_Path.Clear();

        GridTile _CurrentTile;
        GridTile _PathTile;
        // Reset values of grid tiles
        foreach (KeyValuePair<TileKey, GridTile> item in Grid)
        {
            _CurrentTile = item.Value;
            _CurrentTile.f = Mathf.Infinity;
            _CurrentTile.h = Mathf.Infinity;
            _CurrentTile.g = Mathf.Infinity;
        }

        startTile.g = 0;
        startTile.h = 0;
        startTile.f = 0;
        m_OpenList.Add(startTile);

        if (startTile == goalTile)
        {
            m_OpenList.Add(goalTile);
            return m_OpenList;
        }
        while (m_OpenList.Count != 0)
        {
            // Sort the list playerList.Sort((p1,p2)=>p1.score.CompareTo(p2.score));
            m_OpenList.Sort((tileOne, tileTwo) => tileOne.f.CompareTo(tileTwo.f));

            // Get the lowest f
            _CurrentTile = m_OpenList[0];
            m_OpenList.RemoveAt(0);
            m_ClosedList.Add(_CurrentTile);
            // Iterate all surrounding tiles
            GridTile _surroundingTile;
            float _newH, _newG, _newF;
            foreach (KeyValuePair<TileKey, GridTile> item in _CurrentTile.SurroundingTiles)
            {
                _surroundingTile = item.Value;

                if (m_ClosedList.Contains(_surroundingTile) || !TileIsClear(_surroundingTile))
                {
                    continue;
                }

                if (_surroundingTile == goalTile)
                {
                    // Assign parent tile
                    _surroundingTile.ParentTile = _CurrentTile;
                    _PathTile = _surroundingTile;
                    while (_PathTile != startTile)
                    {
                        m_Path.Add(_PathTile);
                        _PathTile = _PathTile.ParentTile;
                    }

                    m_Path.Add(_PathTile);
                    // Goal found, create path
                    return m_Path;
                }
                _newH = GetHeuristic(_surroundingTile.Key, goalTile.Key);
                _newG = _CurrentTile.g + 1;
                _newF = _newG + _newH;
                // check this is a better path
                if (_newF < _surroundingTile.f)
                {
                    _surroundingTile.ParentTile = _CurrentTile;
                    _surroundingTile.h = _newH;
                    _surroundingTile.g = _newG;
                    _surroundingTile.f = _newF;

                    m_OpenList.Add(_surroundingTile);

                }
            }
        }

        return null;
    }

    public List<GridTile> GetPathToUpgrade(GridTile startTile, UpgradeTypes goalUpgrade)
    {

        GridTile _CurrentTile;
        GridTile _PathTile;
        GridTile _GoalTile = FindPathableTileOfType(startTile, goalUpgrade);
        if (_GoalTile == null)
        {
            return null;
        }

        m_OpenList.Clear();
        m_ClosedList.Clear();
        m_Path.Clear();

        m_OpenList.Add(startTile);

        // Reset values of grid tiles
        foreach (KeyValuePair<TileKey, GridTile> item in Grid)
        {
            _CurrentTile = item.Value;
            _CurrentTile.f = Mathf.Infinity;
            _CurrentTile.h = Mathf.Infinity;
            _CurrentTile.g = Mathf.Infinity;
        }

        startTile.g = 0;
        startTile.h = 0;
        startTile.f = 0;
        m_OpenList.Add(startTile);

        while (m_OpenList.Count != 0)
        {
            // Sort the list playerList.Sort((p1,p2)=>p1.score.CompareTo(p2.score));
            m_OpenList.Sort((tileOne, tileTwo) => tileOne.f.CompareTo(tileTwo.f));

            // Get the lowest f
            _CurrentTile = m_OpenList[0];
            m_OpenList.RemoveAt(0);
            m_ClosedList.Add(_CurrentTile);
            // Iterate all surrounding tiles
            GridTile _surroundingTile;
            float _newH, _newG, _newF;
            foreach (KeyValuePair<TileKey, GridTile> item in _CurrentTile.AdjacentTiles)
            {
                _surroundingTile = item.Value;

                if (m_ClosedList.Contains(_surroundingTile) || (!TileIsClear(_surroundingTile) && !TileIsUpgrade(_surroundingTile, goalUpgrade)))
                {
                    continue;
                }

                if (_surroundingTile == _GoalTile)
                {
                    // Assign parent tile
                    _surroundingTile.ParentTile = _CurrentTile;
                    _PathTile = _surroundingTile;
                    while (_PathTile != startTile)
                    {
                        m_Path.Add(_PathTile);
                        _PathTile = _PathTile.ParentTile;
                    }

                    m_Path.Add(_PathTile);
                    // Goal found, create path
                    return m_Path;
                }
                _newH = GetHeuristic(_surroundingTile.Key, _GoalTile.Key);
                _newG = _CurrentTile.g + 1;
                _newF = _newG + _newH;
                // check this is a better path
                if (_newF < _surroundingTile.f)
                {
                    _surroundingTile.ParentTile = _CurrentTile;
                    _surroundingTile.h = _newH;
                    _surroundingTile.g = _newG;
                    _surroundingTile.f = _newF;

                    m_OpenList.Add(_surroundingTile);

                }
            }
        }

        return null;
    }
    private float GetHeuristic(TileKey fromKey, TileKey toKey)
    {
        return Mathf.Abs(toKey.X - fromKey.X) + Mathf.Abs(toKey.Z - fromKey.Z);
    }
    private bool TileIsClear(GridTile tile)
    {
        return tile.Pathable();
    }

    public GridTile FindPathableTileOfType(GridTile startTile, params UpgradeTypes[] goalUpgrade)
    {
        m_OpenList.Clear();
        m_ClosedList.Clear();
        m_Path.Clear();

        GridTile _CurrentTile;

        for (int i = 0; i < goalUpgrade.Length; i++)
        {
            if (startTile.UpgradeType == goalUpgrade[i])
            {
                return startTile;
            }
        }

        m_OpenList.Add(startTile);
        // Find a tile matching the upgrade
        while (true)
        {
            while (m_OpenList.Count != 0)
            {
                _CurrentTile = m_OpenList[0];
                m_OpenList.RemoveAt(0);
                foreach (var _surroundingTile in _CurrentTile.SurroundingTiles)
                {
                    if (m_Path.Contains(_surroundingTile.Value))
                    {
                        continue;
                    }
                    m_Path.Add(_surroundingTile.Value);
                    if (TileIsUpgrade(_surroundingTile.Value, goalUpgrade))
                    {
                        return _surroundingTile.Value;
                    }
                    if (TileIsClear(_surroundingTile.Value))
                    {
                        m_ClosedList.Add(_surroundingTile.Value);
                    }
                }
            }

            while (m_ClosedList.Count != 0)
            {
                _CurrentTile = m_ClosedList[0];
                m_ClosedList.RemoveAt(0);
                foreach (var _surroundingTile in _CurrentTile.SurroundingTiles)
                {

                    if (m_Path.Contains(_surroundingTile.Value))
                    {
                        continue;
                    }
                    m_Path.Add(_surroundingTile.Value);

                    if (TileIsUpgrade(_surroundingTile.Value, goalUpgrade))
                    {
                        return _surroundingTile.Value;
                    }
                    if (TileIsClear(_surroundingTile.Value))
                    {
                        m_OpenList.Add(_surroundingTile.Value);
                    }
                }
            }

            if (m_ClosedList.Count == 0 && m_OpenList.Count == 0)
            {
                return null;
            }
        }
    }
    public List<GridTile> FindPathableTilesOfType(GridTile startTile, params UpgradeTypes[] goalUpgrade)
    {
        m_OpenList.Clear();
        m_ClosedList.Clear();
        m_Path.Clear();
        m_PathableUpgrades.Clear();
        GridTile _CurrentTile;


        m_OpenList.Add(startTile);
        // Find a tile matching the upgrade
        while (true)
        {
            while (m_OpenList.Count != 0)
            {
                _CurrentTile = m_OpenList[0];
                m_OpenList.RemoveAt(0);
                foreach (var _surroundingTile in _CurrentTile.SurroundingTiles)
                {
                    if (m_Path.Contains(_surroundingTile.Value))
                    {
                        continue;
                    }
                    m_Path.Add(_surroundingTile.Value);
                    if (TileIsUpgrade(_surroundingTile.Value, goalUpgrade))
                    {

                        if (!m_PathableUpgrades.Contains(_surroundingTile.Value))
                        {
                            m_PathableUpgrades.Add(_surroundingTile.Value);
                        }

                    }
                    if (TileIsClear(_surroundingTile.Value))
                    {
                        m_ClosedList.Add(_surroundingTile.Value);
                    }
                }
            }

            while (m_ClosedList.Count != 0)
            {
                _CurrentTile = m_ClosedList[0];
                m_ClosedList.RemoveAt(0);
                foreach (var _surroundingTile in _CurrentTile.SurroundingTiles)
                {

                    if (m_Path.Contains(_surroundingTile.Value))
                    {
                        continue;
                    }
                    m_Path.Add(_surroundingTile.Value);

                    if (TileIsUpgrade(_surroundingTile.Value, goalUpgrade))
                    {
                        if (!m_PathableUpgrades.Contains(_surroundingTile.Value))
                        {
                            m_PathableUpgrades.Add(_surroundingTile.Value);
                        }
                    }
                    if (TileIsClear(_surroundingTile.Value))
                    {
                        m_OpenList.Add(_surroundingTile.Value);
                    }
                }
            }

            if (m_ClosedList.Count == 0 && m_OpenList.Count == 0)
            {
                return m_PathableUpgrades;
            }
        }
    }

    private bool TileIsUpgrade(GridTile tile, params UpgradeTypes[] upgrades)
    {

        for (int i = 0; i < upgrades.Length; i++)
        {

            if (tile.UpgradeType == upgrades[i])
            {
                return true;
            }
        }
        return false;
    }

    public bool GetTile(TileKey key, out GridTile tile)
    {
        return Grid.TryGetValue(key, out tile);

    }

    public GridTile AddTileAtKey(TileKey key)
    {
        GridTile _newTile = Instantiate(TilePrefab, transform);
        _newTile.Init(key);
        Grid.Add(key, _newTile);
        _newTile.UpdateSurroundingTiles(this);
        // NavMesh.BuildNavMesh();
        return _newTile;
    }

    public void RemoveTileFromGrid(GridTile _tile)
    {
        _tile.UpgradeTile(UpgradeTypes.grass);
        _tile.ClearSurroundingTiles(this);
        Grid.Remove(_tile.Key);
        Destroy(_tile.gameObject);
    }
    public GridTile GetTile(TileKey key)
    {
        if (Grid.TryGetValue(key, out GridTile _returnTile))
        {
            return _returnTile;
        }

        return null;
    }

    public GridTile GetTile(int x, int z)
    {
        TileKey _key = new TileKey(x, z);
        if (Grid.TryGetValue(_key, out GridTile _returnTile))
        {
            return _returnTile;
        }

        return null;
    }

    public GridTile GetTile(Vector3 position)
    {
        Grid.TryGetValue(GetKeyFromVector(position), out GridTile _returnGridTile);
        return _returnGridTile;
    }

    public GridTile GetRandomPathableTile(GridTile startTile)
    {
        m_OpenList.Clear();
        m_ClosedList.Clear();
        m_OpenList.Add(startTile);

        GridTile _currentTile;
        while (m_OpenList.Count != 0)
        {
            _currentTile = m_OpenList[0];
            m_OpenList.RemoveAt(0);
            m_ClosedList.Add(_currentTile);

            foreach (var item in _currentTile.SurroundingTiles)
            {
                if (!m_ClosedList.Contains(item.Value))
                {
                    m_OpenList.Add(item.Value);
                }
            }
        }

        _currentTile = m_ClosedList[Random.Range(0, m_ClosedList.Count)];

        return _currentTile;

    }

    public GridTile GetRandomTile()
    {
        int _randomValue = Random.Range(0, Grid.Count);
        int _currentIteration = 0;
        foreach (var item in Grid)
        {
            _currentIteration += 1;
            if (_currentIteration == _randomValue)
            {
                return item.Value;
            }
        }
        return null;
    }
    public static TileKey GetKeyFromVector(Vector3 vec)
    {
        return new TileKey(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.z));
    }

}
