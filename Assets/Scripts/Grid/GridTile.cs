//using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public TileKey Key;
    private Color MyColor;
    public UpgradeBase Upgrade;
    public Dictionary<TileKey, GridTile> SurroundingTiles = new Dictionary<TileKey, GridTile>();
    public Dictionary<TileKey, GridTile> AdjacentTiles = new Dictionary<TileKey, GridTile>();


    public Renderer[] TileRenderers;
    public int InstanceId { get { return m_UUID == -1 ? m_UUID = GetInstanceID() : m_UUID; } set { m_UUID = value; } }
    private int m_UUID = -1;

    public UpgradeTypes UpgradeType = UpgradeTypes.grass;
   // public NavMeshModifier[] NavModifiersForTile;
    // 
    #region Pathfinding


    // F: An estimate of the total distance if taking this route.It’s calculated simply using F = G + H.
    [HideInInspector]
    public float f;
    // H: The esitmated distance to end goal
    [HideInInspector]
    public float h;
    //G: The length of the path from the start node to this node.
    [HideInInspector]
    public float g;
    [HideInInspector]
    public GridTile ParentTile;


    #endregion

    public void Init(TileKey key)
    {
        this.Key = key;
        SetLocalPositionByKey(Key);
        if (TileRenderers.Length == 0)
        {
            TileRenderers = GetComponentsInChildren<MeshRenderer>();
        }
    }

    public void SetLocalPositionByKey(TileKey key)
    {
        Vector3 _newPosition = Vector3.zero;

        _newPosition.x = key.X;
        _newPosition.z = key.Z;

        transform.localPosition = _newPosition;
    }

    public void SetSurroundingTiles(GridManager gridManager)
    {
        TileKey[] _surroundingKeys = Types.CardinalKeys;
        for (int i = 0; i < _surroundingKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _surroundingKeys[i], out GridTile _gridTile))
            {
                SurroundingTiles[Key + _surroundingKeys[i]] = _gridTile;
            }
        }

        TileKey[] _adjacentKeys = Types.SurroundingKeys;
        for (int i = 0; i < _adjacentKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _adjacentKeys[i], out GridTile _gridTile))
            {
                AdjacentTiles[Key + _adjacentKeys[i]] = _gridTile;
            }
        }

    }

    public void UpdateSurroundingTiles(GridManager gridManager)
    {
        TileKey[] _surroundingKeys = Types.CardinalKeys;
        for (int i = 0; i < _surroundingKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _surroundingKeys[i], out GridTile _gridTile))
            {
                SurroundingTiles[Key + _surroundingKeys[i]] = _gridTile;
                // notify other tile
                _gridTile.SurroundingTiles[Key + (_surroundingKeys[i] * -1)] = this;
            }
        }

        TileKey[] _adjacentKeys = Types.SurroundingKeys;
        for (int i = 0; i < _adjacentKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _adjacentKeys[i], out GridTile _gridTile))
            {
                AdjacentTiles[Key + _adjacentKeys[i]] = _gridTile;
                // notify other tile
                _gridTile.AdjacentTiles[Key + (_adjacentKeys[i] * -1)] = this;
            }
        }
    }

    public void ClearSurroundingTiles(GridManager gridManager)
    {
        TileKey[] _surroundingKeys = Types.SurroundingKeys;
        for (int i = 0; i < _surroundingKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _surroundingKeys[i], out GridTile _gridTile))
            {

                // notify other tile
                _gridTile.SurroundingTiles.Remove(Key + (_surroundingKeys[i] * -1));
            }
        }

        TileKey[] _adjacentKeys = Types.CardinalKeys;
        for (int i = 0; i < _adjacentKeys.Length; i++)
        {
            if (gridManager.GetTile(Key + _adjacentKeys[i], out GridTile _gridTile))
            {
                // notify other tile
                _gridTile.AdjacentTiles.Remove(Key + (_adjacentKeys[i] * -1));
            }
        }
    }

    public void ClearColor(float duration = 0)
    {
        SetColor(MyColor, duration);
    }

    public void SetColor(Color color, float duration = 0)
    {
        for (int i = 0; i < TileRenderers.Length; i++)
        {
           // DOTween.Complete(InstanceId);
           // TileRenderers[i].material.DOColor(color, duration).SetId(InstanceId);
        }

    }

    public void EnableRenderers()
    {
        for (int i = 0; i < TileRenderers.Length; i++)
        {
            TileRenderers[i].enabled = true;
        }
    }
    public void DisableRenderers()
    {
        for (int i = 0; i < TileRenderers.Length; i++)
        {
            TileRenderers[i].enabled = false;
        }
    }
    public void UpgradeTile(UpgradeBase newUpgrade)
    {
        if (Upgrade != null)
        {

            if (Upgrade == newUpgrade)
            {
                Upgrade.AssignTile(this);
                return;
            }

            Upgrade.RemoveFromTile(this);

            ObjectPool.Instance.ReturnUpgrade(Upgrade);

            Upgrade = null;
        }
        Upgrade = newUpgrade;
        newUpgrade.AssignTile(this);
        UpgradeType = Upgrade.UpgradeType;
    }

    public void UpgradeTile(UpgradeTypes newType, bool bforced = false)
    {
        UpgradeBase _upgrade = ObjectPool.Instance.GetUpgrade(newType);
        if (_upgrade)
        {
            UpgradeTile(_upgrade);
            return;
        }
        // Reset to default
        if (Upgrade != null)
        {
           
            ObjectPool.Instance.ReturnUpgrade(Upgrade);
            Upgrade = null;
        }
        UpgradeType = newType;
    }

    public void ResetTile()
    {
        EnableRenderers();
        UpgradeType = UpgradeTypes.grass;
    }
    public bool Pathable()
    {
        
        return true;
    }
    public Vector3 GetWalkPosition()
    {
        Vector3 _returnPos = transform.position;
        _returnPos.y += 1;
        return _returnPos;
    }

    public void MoveTileVerticallyOverTime(float newLocalY, float duration)
    {
      //  DOTween.Kill(InstanceId + 1); // will need a more unique id

      //  transform.DOLocalMoveY(newLocalY, duration).SetId(InstanceId + 1).OnComplete(() =>
        {
            if (newLocalY != 0)
            {
               // AudioManager.Instance.PlayClipAtPosition(AudioManager.Instance.LiftTile, transform.position);
            }
        };
    }

    public float GetGrowthRate()
    {
        float _growthRate = 1;
        foreach (var item in SurroundingTiles)
        {
            
        }

        return _growthRate;
    }
}
