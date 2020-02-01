using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeUpgrade : UpgradeBase
{
    public int TurnsToGrow = 10;
    private int m_CurrentTurn = 0;

    public bool bWatered = false;

    public Vector3 StartPosition = new Vector3(0,1,0);
    public Vector3 EndPosition = new Vector3(0,0,0);
    public override void ResetUpgrade()
    {
        m_CurrentTurn = 0;
    }
    protected override void SetupFreshUpgrade()
    {
        UpgradeModel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,0.1f);
        UpgradeModel.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, 0.1f);
    }

    public override void StartNewDay(EWeather newWeather)
    {
        m_CurrentTurn += bWatered ? 2:1;
        UpgradeModel.transform.localScale = Vector3.Lerp(Vector3.zero , Vector3.one , (float) m_CurrentTurn / TurnsToGrow);
          UpgradeModel.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, (float) m_CurrentTurn / TurnsToGrow);

        if (m_CurrentTurn >= TurnsToGrow)
        {
            // play some leaf particle
            Debug.Log( gameObject.name + " full grown");

        }
    }
}
