using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : ImageButton
{
    public PlayerAction ActionToSet;
    protected override void ButtonPushed()
    {
        PlayerMouseinput.Instance.SetPlayerAction(ActionToSet);
    }
}
