using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCondition : MonoBehaviour
{
    public EWeather condition;
    public Material skybox;
    public GameObject[] LoopingGameParticles;
  


    public void IntializeWeather(Transform cameraTransform)
    {
        for (int i = 0; i < LoopingGameParticles.Length; i++)
        {
            LoopingGameParticles[i].transform.parent = cameraTransform;
            LoopingGameParticles[i].transform.localPosition = Vector3.zero;

        }
    }
}
