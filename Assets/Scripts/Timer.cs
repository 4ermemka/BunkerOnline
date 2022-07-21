using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/* Поля:
 * OnEndTimer - ивент, срабатывающий при достижении таймером нуля.
 * float time - время до конца таймера.
 * private bool timerRunning - таймер запущен или нет.
 * float remainingTimeFloat - для вывода на экран с плавающей запятой.
 * int remainingTimeInt - для вывода на экран целочисленного значения секунд.
 * 
 * Методы:
 * public Timer(), public Timer(float timeStart) - конструкторы класса.
 * public void SetTime (float timeStart) - метод для установки нового значения таймера.
 * bool GetTimerRunning() - метод для получения информации о запуске/остановке таймера.
 * public bool TimeIsUp() - проверка на обнуление таймера.
 * void Update() - обновление таймера.
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

    void Start()
    {
       
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
            OnEndTimer?.Invoke(this, EventArgs.Empty);
        }
    }
}
