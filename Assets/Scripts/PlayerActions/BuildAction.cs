﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAction : PlayerAction
{
    public int BuildingCost = 3;
    public override bool CanSelectAction()
    {
        if (ObjectPool.Instance.WoodCount < BuildingCost)
        { 
            return true;
        }
        return base.CanSelectAction();
    }

    public override void MouseDown()
    {
        base.MouseDown();
        ObjectPool.Instance.WoodCount -= BuildingCost;
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 happy robot noises 𝅘𝅥𝅮");
    }

    public override void MousePositionUpdated()
    {
        base.MousePositionUpdated();
    }
}
