using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterUpgrade : UpgradeBase
{
    public override void ResetUpgrade()
    {

    }

    protected override void SetupTileForUpgrade()
    {
        ParentTile.DisableRenderers();
    }

    protected override void ResetTileFromUpgrade()
    {
        ParentTile.EnableRenderers();
    }

    public override void StartNewDay(EWeather newWeather)
    {
      
    }
}
