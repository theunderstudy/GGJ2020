using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalWeather : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(GetWeather());
    }
    IEnumerator GetWeather()
    {


        WWW _cityRequest = new WWW("https://api.ipdata.co/city?api-key=9b458d7bc15d0035c5564696960d4fedea2533b290e9383f69b06f10");
        yield return _cityRequest;

        if (_cityRequest.error == null)
        {
           
        }
        else
        {
            Debug.Log("City get failed" + _cityRequest.error);
            yield break;
        }


        string _location = _cityRequest.text    ;
        WWW _weatherRequest = new WWW("http://api.openweathermap.org/data/2.5/weather?q=" + _location + "&APPID=af38440a4558fb5178b658ed3bbddfb0");
        yield return _weatherRequest;
        if (_weatherRequest.error == null)
        {
            Debug.Log(_weatherRequest.text);
        }
        else
        {
            Debug.Log("Weather get failed " +_weatherRequest.error);
        }
    }
}
