using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentStage
{
    Turn,
    Voting,
    Kick
}

public class GameManager : MonoBehaviour
{
    public List<Player> players;

    public float timeToTurn;
    public float timeToVote;
    public Timer playerTimer;
    public Timer votingTimer;
    public Text timerText;
    public int currentPlayer = 0;
    public bool isAllPlayersReadyToVote = false;

    GameManager()
    {
        players = null;
    }

    GameManager(Player players, float timeToTurn)
    {
        this.players = players;
        this.timeToTurn = timeToTurn;
    }

    void Start()
    {
        playerTimer = GetComponent<Timer>();
        votingTimer = GetComponent<Timer>();
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        playerTimer.OnEndTimer += ChangePlayer;
    }

    void Update()
    {
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
    }

    public void ChangePlayer(object sender, EventArgs e)
    {
        if (players.Count < currentPlayer)
        {
            currentPlayer++;
            playerTimer.SetTime(timeToTurn);
        }
        else
        {
            currentPlayer = 0;
            isAllPlayersReadyToVote = true;
            voitingTimer.SetTime(timeToVote);
        }
        

    }

    public void ButtonTimerClick()
    {
        playerTimer.timerRunning = !playerTimer.timerRunning;
    }
}
