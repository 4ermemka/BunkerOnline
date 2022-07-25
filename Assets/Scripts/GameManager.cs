using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentStage
{
    Turn,
    Debate,
    Voting
}

public class GameManager : MonoBehaviour
{
    private User user;
    private int inlistId;
    private List<Player> players;

    [SerializeField] Text displayNickname;
    [SerializeField] Text hostStatus;
    [SerializeField] ChatManager chat;

    private int CountForEndGame;
    private float timeToTurn;
    private float timeToVote;
    private Timer playerTimer;
    private Timer DebateTimer;
    private Text timerText;
    private int currentPlayer = 0;

    CurrentStage currentStage = CurrentStage.Turn;

    GameManager()
    {
        players = null;
    }

    GameManager(List<Player> players, float timeToTurn, float timeToVote, int CountForEndGame)
    {
        this.players = players;
        this.timeToTurn = timeToTurn;
        this.timeToVote = timeToVote;
        this.CountForEndGame = CountForEndGame;
    }

    void Start()
    {
        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname;

        if(user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        playerTimer = GetComponent<Timer>();
        DebateTimer = GetComponent<Timer>();
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        playerTimer.OnEndTimer += ChangePlayer;
        Game();
    }

    void Update()
    {
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
    }

    public string GetMyNick() 
    {
        return user.Nickname;
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
                    {

                    }
                    currentStage = CurrentStage.Debate;
                    break;

                case CurrentStage.Debate:
                    playerTimer.SetTime(timeToVote);
                    foreach (Player element in players)
                    {

                    }
                    currentStage = CurrentStage.Voting;
                    break;

                case CurrentStage.Voting:
                    foreach (Player element in players)
                    {

                    }
                    currentStage = CurrentStage.Turn;
                    break;
            }
        }
    }
}
