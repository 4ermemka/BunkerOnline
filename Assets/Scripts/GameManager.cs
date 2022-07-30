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
    private List<User> users;

    [SerializeField] PlayerInfo playerInfoPref; 
    [SerializeField] Gameobject playersGrid;
    //PlayerInfo temp = Instantiate(playerInfoPref) as PlayerInfo;
    //temp.SetNickname() (образец, дописать методы).
    //temp.gameObject.SetParent(playersGrid.transfrom) (либо transform.SetParent)
    //temp.gameObject.transform.localScale = new Vector3(1,1,1);
    [SerializeField] Text displayNickname;
    [SerializeField] Text hostStatus;
    [SerializeField] ChatManager chat;

    private Server server;
    private Client client;
    private NetManager nm;

    public int CountForEndGame = 1;
    public float timeToTurn = 15;
    public float timeToVote = 15;
    private Timer playerTimer;
    private Timer DebateTimer;
    private Text timerText;
    private int currentPlayer = 0;

    CurrentStage currentStage = CurrentStage.Turn;

    public void ConvertToGameManager(List<User> users, User user)
    {
        this.user = user;
        this.users = users;
    }

    GameManager()
    {
        players = null;
    }

    GameManager(List<User> users, float timeToTurn, float timeToVote, int CountForEndGame)
    {
        ConvertFromUsersToPlayers(users);
        this.timeToTurn = timeToTurn;
        this.timeToVote = timeToVote;
        this.CountForEndGame = CountForEndGame;
    }

    void Start()
    {
        nm = FindObjectOfType<NetManager>();
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();
        ConvertToGameManager(nm.GetUsersList(), nm.GetUser());

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
