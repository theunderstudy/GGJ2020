using UnityEngine;

public abstract class UpgradeBase : MonoBehaviour
{
    public UpgradeTypes UpgradeType;
    public GridTile ParentTile;
    public GameObject UpgradeModel;
    public virtual void SetPositioning(Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
    }
    public virtual void AssignTile(GridTile tile)
    {
        ParentTile = tile;
        SetPositioning(tile.transform);
        SetupTileForUpgrade();
    }
    public virtual void RemoveFromTile(GridTile tile)
    {
      
    }
    protected virtual void SetupTileForUpgrade()
    {

    }

    public virtual bool CostsMet()
    {
        return true;
    }
    public abstract void ResetUpgrade();

}
