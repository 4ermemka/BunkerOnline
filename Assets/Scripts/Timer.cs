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

    private float time = 60f;
    public bool timerRunning = false;
    public float remainingTimeFloat;
    public int remainingTimeInt;

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

    void Start() 
    { 
        remainingTimeFloat = time;
        remainingTimeInt = (int)time;
    }
    
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
            remainingTimeFloat = (float)Math.Round(time, 2);
            remainingTimeInt = (int)time;
            OnEndTimer?.Invoke(this, EventArgs.Empty);
        }
    }
}
