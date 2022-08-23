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

    public int countForEndGame = 1;
    public float timeToTurn = 15;
    public float timeToVote = 15;
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

        players = users;

        votingList = new List<int>();
        NullList(votingList);

        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname + " id: " + user.id;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        timerText.text = playerTimer.remainingTimeMin;
        currentStage = CurrentStage.PreGameDelay;

        if(client != null)
        {
            client.SendServer(MessageProcessing.ClientReadyForGame(user.id));
        }

        if(server != null)
        {
            SetUserActivity(user.id, true);
            server.SendOther(MessageProcessing.ServerReadyForGame());
        }
    }

    public bool AllUsersReady()
    {
        foreach(User u in players)
            if(!u.isReady) return false;
        Debug.Log("ALL PLAYERS READY!");
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
            Debug.Log(allCards == null);
            List<PlayerKit> kits = new List<PlayerKit>();
            kits = SortDeckForKits(allCards,users);
            
            for(int i=1; i<users.Count; i++)
            {
                foreach(DeckCard card in kits[i].cardsKit)
                server.SendClient(nm.hostId, users[i].id,
                MessageProcessing.ServerPlayerKitMsg(card));
            }
            Debug.Log(kits[0].cardsKit.Count);
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
        if(client!=null) client.SendServer(MessageProcessing.CastCardMsg(user, e.card));
        if(server!=null) server.SendOther(MessageProcessing.CastCardMsg(user, e.card));
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
       switch (currentStage) 
       {
        case CurrentStage.PreGameDelay:
            currentPlayer = players[0];
            currentStage = CurrentStage.Turn;

            playerTimer.SetTime(60);
            playerTimer.SetAction(MakeRandomCast);
            break;

        case CurrentStage.Turn:
                currentStage = CurrentStage.TurnDelay;
                playerTimer.SetTime(10);
                playerTimer.SetAction(SwitchTurn);
            break;

        case CurrentStage.TurnDelay:
            if(players.FindIndex(x=> x == currentPlayer) != players.Count-1)//if not last plaing user
            {
                currentPlayer = players[players.FindIndex(x => x == currentPlayer)+1];//switch to next player
                currentStage = CurrentStage.Turn;
                playerTimer.SetTime(60);
                playerTimer.SetAction(MakeRandomCast);
            }
            else // if last player (then go to next stage)
            {   
                currentPlayer = players[0];
                currentStage = CurrentStage.Debate;
                playerTimer.SetTime(120);
                playerTimer.SetAction(SwitchTurn);
            }
            break;

        case CurrentStage.Debate:
            currentStage = CurrentStage.Voting;
            playerTimer.SetTime(60);
            
            playerTimer.SetAction(MakeRandomChoise);
            break;

        case CurrentStage.Voting:
            if(players.FindIndex(x=> x == currentPlayer) != players.Count-1)//if not last plaing user
            {
                currentPlayer = players[players.FindIndex(x => x == currentPlayer)+1];//switch to next player
                currentStage = CurrentStage.Turn;
                playerTimer.SetTime(60);
            }
            else // if last player (then go to next stage)
            {   
                currentPlayer = players[0];
                currentStage = CurrentStage.AfterVotingDelay;
                playerTimer.SetAction(MakeRandomChoise);
                playerTimer.SetTime(120);
            }
            break;

        case CurrentStage.AfterVotingDelay:
            
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
            //make random choise

        }
        SwitchTurn();
    }

    public bool IsMyTurn()
    {
        if(currentStage == CurrentStage.Turn && currentPlayer.id == user.id) return true;
        return false;
    }

    public List<int> GetVotingList()
    {
        return votingList;
    }

    public void SetVotingList(List<int> newList)
    {
        this.votingList = newList;
    }

    private void NullList(List<int> votingList)
    {
        for(int i = 0; i < votingList.Count; i++) 
        {
            votingList[i] = 0;
        }
    }

    public void Voting(int id)
    {
        Debug.Log("This voted for" + id);
        votingList[id]++;
    }

    private int FindPlayerToKick()
    {
        int max = 0;
        foreach (int p in votingList)
            if (p > max) max = p;
        return max;
    }

    public void Kick()
    {
        int playerToKick = FindPlayerToKick();
        NullList(votingList);
        users.RemoveAt(playerToKick);
    }

    public void OnUserLeave(string nickname)
    {
        Destroy(playerInfoList.Find(x=>x.GetUser().Nickname == nickname).gameObject);
    }
}
