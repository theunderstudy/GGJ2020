using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCondition : MonoBehaviour
{
    public EWeather condition;
    public Color skyColor;

    public void SetEnabled(bool isEnabled, Transform cameraTransform)
    {
        gameObject.SetActive(isEnabled);
        if (isEnabled)
        {
            Camera.main.backgroundColor = skyColor;
            ParticleSystem[] loopingParticleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem loopingParticleSystem in loopingParticleSystems)
            {
                loopingParticleSystem.gameObject.SetActive(true);
            }
        }
    }
}
