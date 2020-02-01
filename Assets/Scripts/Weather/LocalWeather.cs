using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
    IEnumerator GetWeather()
    {
        WWW _cityRequest = new WWW("https://api.ipdata.co/city?api-key=9b458d7bc15d0035c5564696960d4fedea2533b290e9383f69b06f10");
        yield return _cityRequest;

        if (_cityRequest.error != null)
        {
            Debug.Log("City get failed" + _cityRequest.error);
            yield break;
        }

        string _location = _cityRequest.text    ;
        WWW _weatherRequest = new WWW("http://api.openweathermap.org/data/2.5/weather?q=" + _location + "&APPID=af38440a4558fb5178b658ed3bbddfb0");
        yield return _weatherRequest;
        if (_weatherRequest.error == null)
        {
            // Debug.Log(_weatherRequest.text);
            dynamic weatherResponse = JObject.Parse(_weatherRequest.text);
            int weatherCode = (int) weatherResponse["weather"][0]["id"];
            int weatherFamily = (weatherCode - (weatherCode % 100)) / 100 - 2;
            this.SetWeather ((EWeather) weatherFamily);
            Debug.Log(currentWeather);
        }
        else
        {
            Debug.Log("Weather get failed " +_weatherRequest.error);
        }
    }
}
