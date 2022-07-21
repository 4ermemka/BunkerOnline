using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Timer playerTimer;
    //public Timer votingTimer;
    public Text timerText;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public int currentPlayer = 0;

    void Start()
    {
        button1.GetComponent<Button>().interactable = true;
        button2.GetComponent<Button>().interactable = false;
        button3.GetComponent<Button>().interactable = false;
        playerTimer = GetComponent<Timer>();
        //votingTimer = GetComponent<Timer>();
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        playerTimer.OnEndTimer += ChangePlayer;
    }

    void Update()
    {
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
    }

    public void ChangePlayer(object sender, EventArgs e)
    {
        if (currentPlayer == 0)
        {
            button1.GetComponent<Button>().interactable = false;
            button2.GetComponent<Button>().interactable = true;
            button3.GetComponent<Button>().interactable = false;
            playerTimer.timerRunning = true;
            currentPlayer = 1;
            playerTimer.SetTime(15f);
        }
        else if (currentPlayer == 1)
        {
            button1.GetComponent<Button>().interactable = false;
            button2.GetComponent<Button>().interactable = false;
            button3.GetComponent<Button>().interactable = true;
            playerTimer.timerRunning = true;
            currentPlayer = 2;
            playerTimer.SetTime(15f);
        }
        else
        {
            button1.GetComponent<Button>().interactable = true;
            button2.GetComponent<Button>().interactable = false;
            button3.GetComponent<Button>().interactable = false;
            playerTimer.timerRunning = true;
            currentPlayer = 0;
            playerTimer.SetTime(15f);
        }
    }

    public void ButtonTimerClick()
    {
        playerTimer.timerRunning = !playerTimer.timerRunning;
    }
}
