﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HarvestAction : PlayerAction
{
    public int WoodPerTree=2;
    public override void MouseDown()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile != null)
        {
            if (CanUpgrade(_newTile.UpgradeType))
            {
                TreeUpgrade _treeUpgrade = (TreeUpgrade)_newTile.Upgrade;

                _newTile.UpgradeTile(UpgradeToPlace);

                ObjectPool.Instance.WoodCount += WoodPerTree;
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

        if (CanUpgrade(m_SelectedTile.UpgradeType))
        {
            m_SelectedTile.MoveTileVerticallyOverTime(0.15f, 0.02f);
        }
        else
        {
            m_SelectedTile = null;
        }
    }
}
