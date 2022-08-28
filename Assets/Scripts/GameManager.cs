using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public enum CurrentStage
{
    PreGameDelay,
    Turn,
    TurnDelay,
    Debate,
    Voting,
    AfterVotingDelay
}

public struct PlayerKit
{
    public string playerName;
    public List<DeckCard> cardsKit;
}

public class GameManager : MonoBehaviour
{
    #region GameManagerFields
    public User user;
    private int inlistId;
    private List<User> users;
    private List<User> players;
    private List<int> votingList;

    [SerializeField] PlayerInfo playerInfoPref; 
    [SerializeField] Attribute atrPref; 
    [SerializeField] Card cardInHandPref;
    [SerializeField] GameObject playersGrid;
    [SerializeField] GameObject observersGrid;
    [SerializeField] GameObject handPanel;
    [SerializeField] OpenedCardsPanel openedCardsPanel;

    List<PlayerInfo> playerInfoList;
    PlayerInfo myPanel;
    List<DeckCard> myCards;

    [SerializeField] private TextMeshProUGUI displayNickname;
    [SerializeField] private TextMeshProUGUI hostStatus;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI currentPlayerTurnText;

    [SerializeField] private ChatManager chat;

    public Server server;
    public Client client;
    private NetManager nm;

    public int countForEndGame;
    public float timeToTurn;
    public float timeToDebate;
    public float timeToVote;
    public float timeToDelay;
    private Timer playerTimer;

    public User currentPlayer;
    public CurrentStage currentStage;

    #endregion

    public void ConvertToGameManager(List<User> users, User user)
    {
        this.user = user;
        this.users = users;
    }

    void Start()
    {
        playerTimer = gameObject.GetComponent<Timer>();

        nm = FindObjectOfType<NetManager>();
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();
        myCards = new List<DeckCard>();

        ConvertToGameManager(nm.GetUsersList(), nm.GetUser());
        playerInfoList = new List<PlayerInfo>();

        MessageProcessing.SetGameManager(this);

        foreach(User u in users) u.isReady = false;

        for (int i = 0; i < users.Count; i++)
        {
            PlayerInfo temp = Instantiate(playerInfoPref) as PlayerInfo;
            temp.SetNickname(users[i].Nickname);
            temp.SetUser(users[i]);
            temp.gameObject.transform.SetParent(playersGrid.transform);
            temp.gameObject.transform.localScale = new Vector3(1, 1, 1);
            temp.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            if(users[i].id == user.id) myPanel = temp; 
            playerInfoList.Add(temp);
        }

        timeToTurn = 30;
        timeToDebate = 30;
        timeToVote = 30;
        timeToDelay = 5;
        
        players = users;

        votingList = new List<int>();
        NullVotes();

        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname + " id: " + user.id;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        timerText.text = playerTimer.remainingTimeMin;
        currentStage = CurrentStage.PreGameDelay;

        MessageProcessing.ReadyForGame(user);
        if(server != null)
        {
            SetUserActivity(user.id, true);
        }
    }

    public bool AllUsersReady()
    {
        foreach(User u in players)
            if(!u.isReady) return false;
        //Debug.Log("ALL PLAYERS READY!");
        chat.AddMessage("SYSTEM","Все подключены, начинаем игру...");
        return true;
    }

    public void StartGame() 
    {
        playerTimer.timerRunning = true;
        playerTimer.SetTime(120);
        if(server!=null)
        {
            List<Category> allCards = new List<Category>();
            
            gameObject.GetComponent<Deck>().UpdateDeck("DefaultDeck.json");
            allCards = gameObject.GetComponent<Deck>().GetCategories();
            //Debug.Log(allCards == null);
            List<PlayerKit> kits = new List<PlayerKit>();
            kits = SortDeckForKits(allCards,users);
            
            for(int i=1; i<users.Count; i++)
            {
                foreach(DeckCard card in kits[i].cardsKit)
                server.SendClient(nm.hostId, users[i].id,
                MessageProcessing.ServerPlayerKitMsg(card));
            }
            //Debug.Log(kits[0].cardsKit.Count);
            foreach(DeckCard card in kits[0].cardsKit)
                SetCardToList(card);
            server.SendOther(MessageProcessing.ServerGameStartedMsg());
            chat.AddMessage("Server","Карты раздал...");
        }
        if(client != null)
        {
            chat.AddMessage("Client","Карты получил...");
        }
        openedCardsPanel.OnCastCard += AddCardToMyPanel;
        SwitchTurn();
    }

    public void SetUserActivity(int id, bool ready)
    {
        users.Find(x=> x.id == id).isReady = ready;
        if(AllUsersReady()) StartGame();
    }

    public List<PlayerKit> SortDeckForKits(List<Category> categories, List<User> users)
    {
        Random random = new Random();
        List<PlayerKit> kits = new List<PlayerKit>();
        for(int i = 0; i < users.Count; i++) 
        {
            PlayerKit kit;
            kit.cardsKit = new List<DeckCard>();
            foreach(Category category in categories) 
            {
                int randomIndex = random.Next(category.GetCategoryCards().Count);
                DeckCard randomCard = category.GetCategoryCards()[randomIndex];
                category.GetCategoryCards().RemoveAt(randomIndex);
                kit.cardsKit.Add(randomCard);
            }
            kit.playerName = users[i].Nickname;
            kits.Add(kit);
        }
        return kits;
    }

    public void SetCardToList(DeckCard card)
    {
        myCards.Add(card);
        UpdateHand();
    }

    public void AddCardToPlayerPanel(User user, DeckCardSerializable card)
    {
        Attribute newAtr = Instantiate(atrPref) as Attribute;
        newAtr.DeckCardSerializableToAttribute(card);
        
        playerInfoList.Find(x=> x.GetUser().id == user.id).AddAttribute(newAtr);
    }

    public void AddCardToMyPanel(object sender, OpenedCardsPanel.OnCastCardEventArgs e)
    {
        AddCardToPlayerPanel(user, e.card.AttributeToDeckCardSerializable());
        MessageProcessing.AddCardToPanel(user, e);
    }

    public void UpdateHand()
    {
        foreach(Transform child in handPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(DeckCard card in myCards) 
        {
            Card cardInHand = Instantiate(cardInHandPref) as Card;

            cardInHand.SetAttributeName(card.name);
            cardInHand.SetCategory(card.category);
            cardInHand.SetDescription(card.description);
            cardInHand.SetIcon(card.iconName);
            cardInHand.SetColor(card.color);

            cardInHand.transform.SetParent(handPanel.transform);
            cardInHand.transform.localScale = new Vector3(1,1,1);
        }
    }

    void Update()
    {
        UpdateCurrentPlayerText();
        UpdateStageText();
        if(playerTimer.GetTime()<=10f && (currentStage == CurrentStage.Debate 
                                       || currentStage == CurrentStage.Turn
                                       || currentStage == CurrentStage.Voting)) 
        {
            timerText.color = Color.red;
            timerText.text = playerTimer.remainingTimeSec;
        }
        else
        {
            timerText.color = Color.white;
            timerText.text = playerTimer.remainingTimeMin;
        }
    }

    public void UpdateStageText()
    {
       switch (currentStage) {
        case CurrentStage.PreGameDelay:
            stageText.text = "Ожидание игроков";
            break;
            
        case CurrentStage.Turn:
            stageText.text = "Сброс карт";
            break;

        case CurrentStage.TurnDelay:
            stageText.text = "Ход окончен...";
            break;

        case CurrentStage.Debate:
            stageText.text = "Обсуждение";
            break;
        
        case CurrentStage.Voting:
            stageText.text = "Голосование";
            break;

        case CurrentStage.AfterVotingDelay:
            stageText.text = "Решение принято";
            break;

       }
    }

    public void UpdateCurrentPlayerText()
    {
        currentPlayerTurnText.text = currentPlayer.Nickname + " id: " + currentPlayer.id;
    }

    public string GetMyNick()
    {
        return user.Nickname;
    }

    public void SwitchTurn()
    {
        Debug.Log("Turn Switched");
        switch (currentStage) 
        {
        case CurrentStage.PreGameDelay:
            currentPlayer = players[0];
            currentStage = CurrentStage.Turn;

            playerTimer.SetTime(timeToTurn);
            playerTimer.SetAction(MakeRandomCast);
            break;

        case CurrentStage.Turn:
                currentStage = CurrentStage.TurnDelay;
                playerTimer.SetTime(timeToDelay);
                playerTimer.SetAction(SwitchTurn);
            break;

        case CurrentStage.TurnDelay:
            if(players.FindIndex(x=> x == currentPlayer) != players.Count-1)//if not last plaing user
            {
                currentPlayer = players[players.FindIndex(x => x == currentPlayer)+1];//switch to next player
                currentStage = CurrentStage.Turn;
                playerTimer.SetTime(timeToTurn);
                playerTimer.SetAction(MakeRandomCast);
            }
            else // if last player (then go to next stage)
            {   
                currentPlayer = players[0];
                currentStage = CurrentStage.Debate;
                playerTimer.SetTime(timeToDebate);
                playerTimer.SetAction(SwitchTurn);
            }
            break;

        case CurrentStage.Debate:
            playerTimer.SetTime(timeToVote);
            currentStage = CurrentStage.Voting;
            
            currentPlayer = players[0];
            playerTimer.SetAction(MakeRandomChoise);
            break;

        case CurrentStage.Voting:
            if(players.FindIndex(x => x == currentPlayer) != players.Count-1)//if not last plaing user
            {
                currentPlayer = players[players.FindIndex(x => x == currentPlayer)+1];//switch to next player
                currentStage = CurrentStage.Voting;
                playerTimer.SetTime(timeToVote);
            }
            else // if last player (then go to next stage)
            {   
                currentPlayer = players[0];
                currentStage = CurrentStage.AfterVotingDelay;
                playerTimer.SetAction(SwitchTurn);
                playerTimer.SetTime(timeToDelay);
                Kick();

                NullVotes();
            }
            break;

        case CurrentStage.AfterVotingDelay:
            if(players.Count <= countForEndGame) Debug.Log("WINING!");
            else
            {
                currentPlayer = players[0];
                currentStage = CurrentStage.Turn;

                playerTimer.SetTime(timeToTurn);
                playerTimer.SetAction(MakeRandomCast);
            }
            break;
       }
    }

    public void MakeRandomCast()
    {
        if(currentPlayer.id == user.id)
        {
            Random random = new Random();
            int randomIndex = random.Next(handPanel.transform.childCount);

            Card randomCard = handPanel.transform.GetChild(randomIndex).GetComponent<Card>();
            openedCardsPanel.AddCardToList(randomCard);
            Destroy(randomCard.gameObject);
        }
        SwitchTurn();
    }
    
    public void MakeRandomChoise()
    {
        if(currentPlayer == user)
        {
            KickConfirm panel = FindObjectOfType<KickConfirm>();
            if(panel != null) Destroy(panel.gameObject);

            Random random = new Random();
            int randomIndex = random.Next(players.Count);
            MyVoteFor(players[randomIndex]);
        }
        SwitchTurn();
    }

    public bool IsMyTurn()
    {
        if(currentStage == CurrentStage.Turn && currentPlayer.id == user.id) return true;
        return false;
    }

    public bool IsMyVoteTurn()
    {
        if(currentStage == CurrentStage.Voting && currentPlayer.id == user.id) return true;
        return false;
    }

    private void NullVotes()
    {
        foreach(User p in players) p.votesFor = 0;
    }

    public void VotingForPlayer(int id)
    {
        players.Find(x=>x.id == id).votesFor++;
        SwitchTurn();
    }

    public void MyVoteFor(User user)
    {
        VotingForPlayer(user.id);
        MessageProcessing.VoteFor(user);
    }

    private User FindPlayerToKick()
    {
        int max = 0;
        foreach (User p in players)
            if (p.votesFor > max) max = p.votesFor;

        return players.Find(x=>x.votesFor == max);
    }

    public void Kick()
    {
        User playerToKick = FindPlayerToKick();
        NullVotes();
        
        if(playerToKick!=null)
        {
            chat.AddMessage("KICK_MANAGER","Кикаем игрока "+ playerToKick.Nickname);
            players.Remove(playerToKick);
            Destroy(playerInfoList.Find(x=>x.GetUser().id == playerToKick.id).gameObject);
            playerInfoList.Remove(playerInfoList.Find(x=>x.GetUser().id == playerToKick.id));
            
            UserInfo newObserver = Instantiate(nm.MenuInterfaceManager.userInfo) as UserInfo;
            
            newObserver.nickname = playerToKick.Nickname;
            newObserver.id = playerToKick.id;
            newObserver.isHost = playerToKick.isHost;

            newObserver.transform.SetParent(observersGrid.transform);
            newObserver.transform.localScale = Vector3.one;
        }
    }

    public void OnUserLeave(int id)
    {
        User kickedPlayer = players.Find(x=>x.id == id);
        if(kickedPlayer!=null)
        {
            players.Remove(players.Find(x=>x.id == id));
            Destroy(playerInfoList.Find(x=>x.GetUser().id == id).gameObject);
            playerInfoList.Remove(playerInfoList.Find(x=>x.GetUser().id == id));
        }
    }
}