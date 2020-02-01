using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool bInSequence = false    ;
    private float fadetime =1.5f;



    public void StartIntroSequence()
    {
        
    }
    public void StartNightSequence()
    {
        if (bInSequence )
        {
            return;
        }
        StartCoroutine(NightSequence());
    }

    IEnumerator NightSequence()
    {
        UIManager.Instance.NightFadeOut(fadetime);

        yield return new WaitForSeconds(fadetime*1.2f);
        DayNightManager.Instance.EndDay();
        //update local weather

        // start new day
        DayNightManager.Instance.StartNewDay();
        UIManager.Instance.NightFadeIn(fadetime);
        yield return new WaitForSeconds(fadetime * 1.2f);

    }
}
