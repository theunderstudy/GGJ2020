using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterUpgrade : UpgradeBase
{
    public override void ResetUpgrade()
    {
        bWatered = true;
    }

    protected override void SetupTileForUpgrade()
    {
        bWatered = true;
        ParentTile.DisableRenderers();
    }

    protected override void ResetTileFromUpgrade()
    {
        ParentTile.EnableRenderers();
    }

    public override void StartNewDay(EWeather newWeather)
    {
      
    }

    public override void WaterTile()
    {
       
    }
}
