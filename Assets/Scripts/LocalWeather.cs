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
        // build location text
        string location = "wellington";
        WWW request = new WWW("http://api.openweathermap.org/data/2.5/weather?q=" + location + "&APPID=af38440a4558fb5178b658ed3bbddfb0");
        // WWW request = new WWW("http://api.openweathermap.org/data/2.5/weather?q=Anuradhapura,Sri Lanka&APPID=e3a642cec13d52496490dfa8e9ba11d3");
        yield return request;

        if (request.error == null)
        {
            Debug.Log(request.text);
        }
        else
        {
            Debug.Log(request.error);
        }
    }
}
