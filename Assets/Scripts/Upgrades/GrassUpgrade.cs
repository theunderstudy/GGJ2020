﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassUpgrade : UpgradeBase
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

}