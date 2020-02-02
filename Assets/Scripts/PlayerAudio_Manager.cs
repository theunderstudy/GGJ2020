using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAudio_Manager : Singleton<PlayerAudio_Manager>
{
    [Header("Audio Settings")]
    [EventRef]
    public List<string> botNoises = new List<string>();

    public void BotChatter(string botsfx)
    {
        RuntimeManager.PlayOneShot(botNoises.Find(sfx => sfx.Contains(botsfx)), transform.position);
    }

}
