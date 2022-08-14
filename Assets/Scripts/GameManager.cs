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
    Turn,
    Debate,
    Voting
}

public struct PlayerKit
{
    public string playerName;
    public List<DeckCard> cardsKit;
}

public class GameManager : MonoBehaviour
{
    #region GameManagerFields
    private User user;
    private int inlistId;
    private List<User> users;
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

    private Server server;
    private Client client;
    private NetManager nm;
    private MessageProcessing mp;

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
        List<Category> allCards = new List<Category>();
        playerTimer = gameObject.GetComponent<Timer>();

        nm = FindObjectOfType<NetManager>();
        mp = nm.GetMessageProcessing();
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();
        myCards = new List<DeckCard>();

        nm.GetMessageProcessing().SetGameManager(this);

        ConvertToGameManager(nm.GetUsersList(), nm.GetUser());
        playerInfoList = new List<PlayerInfo>();

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
        votingList = new List<int>();
        NullList(votingList);

        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        playerTimer.SetTime(15);

        timerText.text = playerTimer.remainingTimeFloat.ToString("F2");
        //playerTimer.OnEndTimer += ChangePlayer;
        
        if(server!=null)
        {
            server.SendOther(mp.ServerGameStartedMsg());
            
            gameObject.GetComponent<Deck>().UpdateDeck("DefaultDeck.json");
            allCards = gameObject.GetComponent<Deck>().GetCategories();
            List<PlayerKit> kits = new List<PlayerKit>();
            kits = SortDeckForKits(allCards,users);
            
            for(int i=1; i<users.Count; i++)
            {
                foreach(DeckCard card in kits[i].cardsKit)
                server.SendClient(nm.hostId, users[i].id, 
                mp.ServerPlayerKitMsg(card));
            }

            foreach(DeckCard card in kits[0].cardsKit)
                SetCardToList(card);
        }
        MenuInterfaceManager.OnStartGame += Game;
        openedCardsPanel.OnCastCard += AddCardToMyPanel;
        Debug.Log("Game started!");
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
        if(client!=null) client.SendServer(mp.CastCardMsg(user, e.card));
        if(server!=null) server.SendOther(mp.CastCardMsg(user, e.card));
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

    public void SetvotingList()
    {
        
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
                        //Turn();
                    }
                    currentStage = CurrentStage.Debate;
                    break;

                case CurrentStage.Debate:
                    playerTimer.SetTime(timeToVote);
                    foreach (User element in users)
                    {

                    }
                    currentStage = CurrentStage.Voting;
                    break;

                case CurrentStage.Voting:
                    foreach (User element in users)
                    {

                    }
                    currentStage = CurrentStage.Turn;
                    break;
            }
        }
    }

    public List<int> GetVotingList()
    {
        return votingList;
    }

    public void SetVotingList(List<int> newList)
    {
        this.votingList = newList;
    }

    //Voting methods
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
        //if(server!=null) server.SendOther();
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
}
