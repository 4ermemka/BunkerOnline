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

    public int CountForEndGame;
    public float timeToTurn;
    public float timeToVote;
    public Timer playerTimer;
    public Timer votingTimer;
    public Text timerText;
    public int currentPlayer = 0;


    CurrentStage currentStage = CurrentStage.Turn;

    GameManager()
    {
        players = null;
    }

    GameManager(Player players, float timeToTurn, float timeToVote, int CountForEndGame)
    {
        this.players = players;
        this.timeToTurn = timeToTurn;
        this.timeToVote = timeToVote;
        this.CountForEndGame = CountForEndGame;
    }

    void Start()
    {

        playerTimer = GetComponent<Timer>();
        votingTimer = GetComponent<Timer>();
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        playerTimer.OnEndTimer += ChangePlayer;
        Game();
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
        else currentPlayer = 0;
    }

    public void ButtonTimerClick()
    {
        playerTimer.timerRunning = !playerTimer.timerRunning;
    }

    public void Game()
    {
        while (players.Count > CountForEndGame)
        {
            switch (currentStage)
            {

                case CurrentStage.Turn:
                    foreach (Player element in players)
                    {//тут ход
                    }
                    currentStage = CurrentStage.Voting;

                case CurrentStage.Voting:
                    voitingTimer.SetTime(timeToVote);
                    foreach (Player element in players)
                    {//тут голосование
                    }
                    currentStage = CurrentStage.Kick;
                case CurrentStage.Kick:
                    //удаление игрока
                    currentStage = CurrentStage.Turn;
            }
        }
    }

}
