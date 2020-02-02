using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerUpgrade : UpgradeBase
{
    public override void EndOfDay(EWeather newWeather)
    {

    }

    public override void ResetUpgrade()
    {

    }

    public override void StartNewDay(EWeather newWeather)
    {
        // Water all tiles around the sprinkler
        foreach (var item in ParentTile.SurroundingTiles)
        {
            UpgradeBase _upgrade = item.Value.Upgrade;
            if (_upgrade!=null)
            {
                _upgrade.WaterTile();
            }
        }
    }

    public override void WaterTile()
    {

    }
}
