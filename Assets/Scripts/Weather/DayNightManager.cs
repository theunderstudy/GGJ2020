using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightManager : Singleton<DayNightManager>
{
    public delegate void NewDay(EWeather weatherType);
    public static event NewDay NewDayEvent;
    public int CurrentDay = 1;

    public void StartNewDay()
    {
        EWeather _currentWeather = EWeather.Clear;
        CurrentDay += 1;
        NewDayEvent?.Invoke(_currentWeather);
    }
   
}
