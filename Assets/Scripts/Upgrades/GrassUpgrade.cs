﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassUpgrade : UpgradeBase
{
    public int TurnsTillDegrade = 3;
    private int m_CurrentTurn = 0;
    public Color FreshGreenColor;
    public Color ShitDirtColor;
    private MeshRenderer[] m_UpgradeRenderers;
    public override void ResetUpgrade()
    {
        m_CurrentTurn = 0;
    }

    protected override void SetupTileForUpgrade()
    {
        ParentTile.DisableRenderers();
        if (m_UpgradeRenderers == null)
        {
            m_UpgradeRenderers = UpgradeModel.GetComponentsInChildren<MeshRenderer>();
        }
    }

    protected override void ResetTileFromUpgrade()
    {
        ParentTile.EnableRenderers();
    }

    public override void EndOfDay(EWeather newWeather)
    {
        m_CurrentTurn += 1;

        Color _newColor = Color.Lerp(FreshGreenColor,ShitDirtColor , (float) m_CurrentTurn / TurnsTillDegrade);
        for (int i = 0; i < m_UpgradeRenderers.Length; i++)
        {
            m_UpgradeRenderers[i].material.color =(_newColor);
        }

        // check if degrade to dirt
        if (m_CurrentTurn > TurnsTillDegrade)
        {
            ParentTile.UpgradeTile(UpgradeTypes.dirt);
        }
    }

    public override void WaterTile()
    {
        bWatered = true;
        m_CurrentTurn = 0;
        // Change color of tile
        Color _newColor = Color.Lerp(FreshGreenColor, ShitDirtColor, (float)m_CurrentTurn / TurnsTillDegrade);
        for (int i = 0; i < m_UpgradeRenderers.Length; i++)
        {
            m_UpgradeRenderers[i].material.color = (_newColor);
        }
    }

    public override void StartNewDay(EWeather newWeather)
    {
        switch (newWeather)
        {
            case EWeather.Thunderstorm:
                goto case EWeather.Rain;
            case EWeather.Drizzle:
                goto case EWeather.Rain;
            case EWeather.Rain:
                WaterTile();
                break;
            case EWeather.Snow:
                goto case EWeather.Rain;
            case EWeather.Fog:
                break;
            case EWeather.Clear:
                break;
            case EWeather.Clouds:
                break;
            default:
                break;
        }
    }
}
