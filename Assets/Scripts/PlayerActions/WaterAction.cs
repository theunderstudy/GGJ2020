using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAction : PlayerAction
{
    public override void MouseDown()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile != null)
        {
            if (_newTile.Upgrade != null)
            {
                if (_newTile.Upgrade.CanWater())
                {
                    _newTile.Upgrade.WaterTile();

                    PlayerController.Instance.StartWork(_newTile.transform.position, 1f);
                }              
            }
        }
    }

    public override void MousePositionUpdated()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile == null)
        {
            if (m_SelectedTile != null)
            {
                m_SelectedTile.MoveTileVerticallyOverTime(0, 0.2f);
                m_SelectedTile = null;
            }
            return;
        }

        if (_newTile == m_SelectedTile)
        {
            return;
        }

        if (m_SelectedTile != null)
        {
            m_SelectedTile.MoveTileVerticallyOverTime(0, 0.2f);
        }



        m_SelectedTile = _newTile;

        if (m_SelectedTile.Upgrade != null)
        {
            if (m_SelectedTile.Upgrade.CanWater())
            {
                m_SelectedTile.MoveTileVerticallyOverTime(0.15f, 0.02f);
            }
        }
        else
        {
            m_SelectedTile = null;
        }
    }
}
