using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/* ТАЙМЕР:
 * OnEndTimer - Событие. Срабатывает при истечении времени таймера.
 * float time - Счетчик таймера.
 * private bool timerRunning - Индикатор запущенности таймера.
 * float remainingTimeSec - Публичная переменная, оставшееся время с миллисекундами.
 * int remainingTimeMin - Публичная переменная, оставшееся время в целых секундах.
 * 
 * МЕТОДЫ:
 * Timer(), public Timer(float timeStart) - Конструкторы.
 * void SetTime (float timeStart) - Задать время.
 * bool GetTimerRunning() - Отследить, запущен ли таймер.
 * bool TimeIsUp() - Индикатор истекшего таймера.
 * void Update() - Обновление таймера.
 */

public class Timer : MonoBehaviour
{

    private Action timerCallback;

    public float time;
    public bool timerRunning = false;
    public string remainingTimeSec;
    public string remainingTimeMin;

    public void SetTime (float timeStart)
    {
        this.time = timeStart;
        TimeSpan t = TimeSpan.FromSeconds(time);
        remainingTimeMin = t.Minutes + ":" + t.Seconds;
        remainingTimeSec = t.ToString(@"ss\,fff");
    }

    public void SetAction(Action timerCallback)
    {
        this.timerCallback = timerCallback;
    }

    public bool GetTimerRunning()
    {
        return timerRunning;
    }

    public bool TimeIsUp()
    {
        if (time == 0) return true;
        else return false;
    }

    void Start() 
    {
        time = 120f;
        TimeSpan t = TimeSpan.FromSeconds(time);
        remainingTimeMin = t.Minutes + ":" + t.Seconds;
        remainingTimeSec = t.ToString(@"ss\,fff");
    }
    
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(time);
            remainingTimeMin = t.Minutes + ":" + t.Seconds;
            remainingTimeSec = t.ToString(@"ss\,fff");
        }
        if (time < 0 || !timerRunning)
        {
            time = 0;
            TimeSpan t = TimeSpan.FromSeconds(time);
            remainingTimeMin = t.Minutes + ":" + t.Seconds;
            remainingTimeSec = t.ToString(@"ss\,fff");
            if(timerCallback != null) timerCallback();
        }
    }

    public float GetTime()
    {
        return time;
    }
}
