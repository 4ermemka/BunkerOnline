using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    #region GameManagerFields
    private User user;
    private int inlistId;
    public List<User> users;
    public List<int> votingArray;

    [SerializeField] PlayerInfo playerInfoPref; 
    [SerializeField] GameObject playersGrid;
    List<PlayerInfo> playerInfoList;

    [SerializeField] private TextMeshProUGUI displayNickname;
    [SerializeField] private TextMeshProUGUI hostStatus;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI currentPlayerTurnText;

    [SerializeField] private ChatManager chat;

    private Server server;
    private Client client;
    private NetManager nm;
    private MessageProcessing messageProcessing;

    public int countForEndGame = 1;
    public float timeToTurn = 15;
    public float timeToVote = 15;
    private Timer playerTimer;
    private int currentPlayer = 0;

    CurrentStage currentStage = CurrentStage.Turn;
    #endregion

    public void ConvertToGameManager(List<User> users, User user)
    {
        this.user = user;
        this.users = users;
    }

    //GameManager(List<User> users, float timeToTurn, float timeToVote, int CountForEndGame)
    //{
    //    ConvertFromUsersToPlayers(users);
    //    this.timeToTurn = timeToTurn;
    //    this.timeToVote = timeToVote;
    //    this.CountForEndGame = CountForEndGame;
    //}

    void Start()
    {
        nm = FindObjectOfType<NetManager>();
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();

        playerTimer = gameObject.GetComponent<Timer>();
        //ConvertToGameManager(nm.GetUsersList(), nm.GetUser());
        playerInfoList = new List<PlayerInfo>();
        for (int i = 0; i < users.Count; i++)
        {
            PlayerInfo temp = Instantiate(playerInfoPref) as PlayerInfo;
            temp.SetNickname(users[i].Nickname);
            temp.gameObject.transform.SetParent(playersGrid.transform);
            temp.gameObject.transform.localScale = new Vector3(1, 1, 1);
            temp.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            playerInfoList.Add(temp);
        }
        votingArray = new List<int>[users.Count];
        NullArray();

        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        playerTimer.SetTime(120);
        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        //playerTimer.OnEndTimer += ChangePlayer;
        MenuInterfaceManager.OnStartGame += Game;
        Debug.Log("Game started!");
    }

    void Update()
    {
        if(playerTimer == null) Debug.Log("TIMER NULL");
        if(playerTimer.remainingTimeFloat<=10f) 
        {
            timerText.color = Color.red;
            timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        }
        else
        {
            timerText.color = Color.white;
            timerText.text = playerTimer.remainingTimeInt.ToString();
        }
    }

    public string GetMyNick()
    {
        return user.Nickname;
    }

    public void ButtonTimerClick()
    {
        playerTimer.timerRunning = !playerTimer.timerRunning;
    }

    public void Game(object sender, EventArgs e)
    {
        while (users.Count > countForEndGame)
        {
            switch (currentStage)
            {

                case CurrentStage.Turn:
                    foreach (User element in users)
                    {
                        Turn();
                    }
                    currentStage = CurrentStage.Debate;
                    break;

                case CurrentStage.Debate:
                    
                    foreach (User element in users)
                    {

                    }
                    currentStage = CurrentStage.Voting;
                    break;

                case CurrentStage.Voting:
                    playerTimer.SetTime(timeToVote);
                    //here user click on another player and get id of him
                    int playerToKick = FindPlayerToKick();
                    Kick();
                    currentStage = CurrentStage.Turn;
                    break;
            }
        }
    }

    #region VotingMethods
    private void NullArray()
    {
        for (int i = 0; i < votingArray.Length; i++)
            votingArray[i] = 0;
    }

    public void Voting(int id)
    {
        Debug.Log(user.id + " voted for " + id);
        votingArray[id]++;
        if (server != null)
        {
            server.SendOther(messageProcessing.ServerUpdateVotingArray(votingArray));
            server.SendOther(messageProcessing.ServerPlayerVoteMsg(user, id));
        }
        else client.SendServer(messageProcessing.ServerPlayerVoteMsg(user, id));
    }

    private int FindPlayerToKick()
    {
        int max = 0;
        for (int i = 0; i < votingArray.Length; i++)
            if (votingArray[i] > max) max = votingArray[i];
        return max;
    }

    public void Kick()
    {
        int playerToKick = FindPlayerToKick();
        NullArray();
        users.RemoveAt(playerToKick);
    }
    #endregion

    public void Turn ()
    {
        //on/off the buttons and other...
    }
}
