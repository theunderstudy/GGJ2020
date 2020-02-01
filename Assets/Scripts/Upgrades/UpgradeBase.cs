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
        SetupFreshUpgrade();
    }
    public virtual void RemoveFromTile(GridTile tile)
    {
        ResetTileFromUpgrade();
    }
    protected virtual void SetupFreshUpgrade()
    {

    }

    protected virtual void SetupTileForUpgrade()
    {

    } 
    
    protected virtual void ResetTileFromUpgrade()
    {

    }

    public virtual bool CostsMet()
    {
        return true;
    }
    public abstract void ResetUpgrade();

    public abstract void StartNewDay(EWeather newWeather);

    private void OnEnable()
    {
        DayNightManager.NewDayEvent += StartNewDay;
    }
    private void OnDisable()
    {
        DayNightManager.NewDayEvent -= StartNewDay;

    }
}
