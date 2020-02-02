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
        EndDayEvent?.Invoke(LocalWeather.Instance.currentWeather);
    }
    public void StartNewDay()
    {
        EWeather randomWeather = (EWeather) Random.Range(0, 7);
        LocalWeather.Instance.SetWeather(randomWeather);
        CurrentDay += 1;
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 fully charged beep 𝅘𝅥𝅮");
        NewDayEvent?.Invoke(randomWeather);
    }
}
