using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.StartNightSequence();
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 charging robot noises 𝅘𝅥𝅮");
    }

    private void OnTriggerExit(Collider other)
    {
       
    }
}
