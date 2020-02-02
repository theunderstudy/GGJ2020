using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightManager : Singleton<DayNightManager>
{
    public delegate void NewDay(EWeather weatherType);
    public static event NewDay EndDayEvent;
    public static event NewDay NewDayEvent;
    public int CurrentDay = 1;


    public void EndDay()
    {
        EWeather _currentWeather = EWeather.Clear;

        EndDayEvent?.Invoke(_currentWeather);
    }
    public void StartNewDay()
    {
        EWeather _currentWeather = EWeather.Clear;
        CurrentDay += 1;
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 fully charged beep 𝅘𝅥𝅮");
        NewDayEvent?.Invoke(_currentWeather);
    }
}
