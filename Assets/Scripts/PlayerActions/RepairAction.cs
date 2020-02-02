using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairAction : PlayerAction
{
    public int WoodCost = 5;
    public override void MouseDown()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile != null)
        {
            if (!CanWorkTile(_newTile))
            {
                return;
            }

            if (WoodCost > ObjectPool.Instance.WoodCount)
            {
                return;
            }
                TreeUpgrade _treeUpgrade = (TreeUpgrade)_newTile.Upgrade;

              
                    _newTile.UpgradeTile(UpgradeToPlace);

                    ObjectPool.Instance.WoodCount -= WoodCost;
                    _newTile.MoveTileVerticallyOverTime(0, 0.2f);
                    PlayerController.Instance.StartWork(_newTile.transform.position, 1f);

                

            
        }
    }

}
