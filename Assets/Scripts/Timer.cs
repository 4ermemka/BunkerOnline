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
 * float remainingTimeFloat - Публичная переменная, оставшееся время с миллисекундами.
 * int remainingTimeInt - Публичная переменная, оставшееся время в целых секундах.
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
    public event EventHandler OnEndTimer;

    private float time;
    public bool timerRunning = true;
    public float remainingTimeFloat;
    public int remainingTimeInt;


    public Timer()
    {
        time = 15;
        remainingTimeFloat = time;
        remainingTimeInt = (int)time;
    }

    public Timer(float timeStart)
    {
        this.time = timeStart;
        remainingTimeFloat = time;
        remainingTimeInt = (int)time;
    }

    public void SetTime (float timeStart)
    {
        this.time = timeStart;
        remainingTimeFloat = time;
        remainingTimeInt = (int)time;
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

    void Start() { }
    
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            remainingTimeFloat = (float)Math.Round(time, 2);
            remainingTimeInt = (int)time;
        }
        if (time < 0 || !timerRunning)
        {
            time = 0;
            OnEndTimer?.Invoke(this, EventArgs.Empty);
        }
    }
}
