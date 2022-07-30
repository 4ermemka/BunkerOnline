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

    public int CountForEndGame = 1;
    public float timeToTurn = 15;
    public float timeToVote = 15;
    private Timer playerTimer;
    private Timer DebateTimer;
    private Text timerText;
    private int currentPlayer = 0;

    CurrentStage currentStage = CurrentStage.Turn;

    public void FromUsersToPlayers(List<User> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            Player player = new Player(users[i].id, users[i].Nickname);
            players.Add(player);
        }
    }

    GameManager()
    {
        players = null;
    }

    GameManager(List<User> users, float timeToTurn, float timeToVote, int CountForEndGame)
    {
        FromUsersToPlayers(users);
        this.timeToTurn = timeToTurn;
        this.timeToVote = timeToVote;
        this.CountForEndGame = CountForEndGame;
    }

    void Start()
    {
        players = new List<Player>();


        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        playerTimer = GetComponent<Timer>();
        DebateTimer = GetComponent<Timer>();
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        playerTimer.OnEndTimer += ChangePlayer;
        MenuInterfaceManager.OnStartGame += Game;
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

    public void Game(object sender, EventArgs e)
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
