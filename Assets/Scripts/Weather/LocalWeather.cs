﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class LocalWeather : MonoBehaviour
{
    public EWeather currentWeather;
    public WeatherCondition currentWeatherCondition;
    public WeatherCondition[] supportedWeatherConditions;

    private void Start()
    {
        StartCoroutine(GetWeather());
    }

    private void SetWeather(EWeather newWeather) {
        Debug.Log("Set Weather called with" + newWeather.ToString());
        foreach (var weatherCondition in supportedWeatherConditions)
        {
            weatherCondition.gameObject.SetActive(false);
            if (weatherCondition.condition == newWeather)
            {
                currentWeatherCondition = weatherCondition;
                weatherCondition.gameObject.SetActive(true);
            }
        }
        currentWeather = newWeather;
        RenderSettings.skybox = currentWeatherCondition.skybox;

        currentWeatherCondition.IntializeWeather(Camera.main.transform);
    }
    IEnumerator GetWeather()
    {
        UnityWebRequest  cityRequest = UnityWebRequest.Get("https://api.ipdata.co/city?api-key=9b458d7bc15d0035c5564696960d4fedea2533b290e9383f69b06f10");
        yield return cityRequest.SendWebRequest();

        if (cityRequest.isNetworkError || cityRequest.isHttpError)
        {
            Debug.Log("City get failed" + cityRequest.error);
            yield break;
        }

        string location = cityRequest.downloadHandler.text;
        UnityWebRequest weatherRequest = UnityWebRequest.Get("http://api.openweathermap.org/data/2.5/weather?q=" + location + "&APPID=af38440a4558fb5178b658ed3bbddfb0");
        yield return weatherRequest.SendWebRequest();
        if (weatherRequest.isNetworkError || weatherRequest.isHttpError)
        {
            Debug.Log("Weather get failed " + weatherRequest.error);
        }
        else
        {
            string weatherResponseRaw = weatherRequest.downloadHandler.text;
            Debug.Log(weatherResponseRaw);
            dynamic weatherResponse = JObject.Parse(weatherResponseRaw);
            int weatherCode = (int) weatherResponse["weather"][0]["id"];
            int weatherFamily = (weatherCode - (weatherCode % 100)) / 100 - 2;
            this.SetWeather ((EWeather) weatherFamily);
            Debug.Log(currentWeather);
        }
    }
}
