using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairAction : PlayerAction
{
    public int WoodCost = 5;


    public delegate void WindmillFixed();
    public static event WindmillFixed WindMillFixedEvent;

    public override void MouseDown()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile != null)
        {
            _newTile.MoveTileVerticallyOverTime(0, 0.2f);

            if (!CanUpgrade(_newTile.UpgradeType))
            {
                return;
            }
            if (_newTile.Upgrade)
            {
                WindmillUpgrade _windmill = (WindmillUpgrade)_newTile.Upgrade;
                if (_windmill.bUpgraded == false)
                {
                    PlayerController.Instance.StartWork(_newTile, 5f);

                    _windmill.bUpgraded = true;


                    _windmill.FixedWindmill.SetActive(true);
                    _windmill.BrokeWindmill.SetActive(false);
                    WindMillFixedEvent?.Invoke();
                    Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Lovely! The windmill is all shiny and new again. And look, you’ve cleared the dust clouds away from the nearby tiles.", 6);
                    WindMillFixedEvent();
                }
            }

        }
    }

}