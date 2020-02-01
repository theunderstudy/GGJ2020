using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToBed : ImageButton
{
    protected override void ButtonPushed()
    {
        UIManager.Instance.ShowNewDayButton(false);
        GameManager.Instance.StartNightSequence();
    }
}
