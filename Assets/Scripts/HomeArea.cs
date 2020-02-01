using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.Instance.ShowNewDayButton(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.Instance.ShowNewDayButton(false);
        }

    }
}
