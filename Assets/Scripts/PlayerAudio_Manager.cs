using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAudio_Manager : Singleton<PlayerAudio_Manager>
{
    [Header("Audio Settings")]
    [EventRef]
    public List<string> botNoises = new List<string>();

    private void OnEnable()
    {

        PlayerController.EnergyUpdatedEvent += BotChargeLevel;
    }

    public void BotChatter(int botsfx)
    {
        RuntimeManager.PlayOneShot(botNoises[botsfx], transform.position);
    }

    public void BotChargeLevel(float chargeLevel)
    {
        if (chargeLevel >= 1) BotChatter(3);
        if (chargeLevel > 0.2 && chargeLevel <= 0.5) BotChatter(0);
        if (chargeLevel <= 0.2) BotChatter(1);

    }

}
