using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using UnityEngine.SceneManagement;

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
    public bool isKicked;
    public bool endgameShown;

    private List<User> users;
    private List<User> players;
    private List<int> votingList;

    [SerializeField] EndgamePanel endgamePanel; 
    [SerializeField] PlayerInfo playerInfoPref; 
    [SerializeField] UserInfo userInfoPref; 
    [SerializeField] Attribute atrPref; 
    [SerializeField] Card cardInHandPref;
    [SerializeField] GameObject playersGrid;
    [SerializeField] GameObject observersGrid;
    [SerializeField] KickedPanel kickedPanel;
    [SerializeField] GameObject handPanel;
    [SerializeField] OpenedCardsPanel openedCardsPanel;

    [SerializeField] private TabGroup Buttons;
    [SerializeField] private TabButton CardsButton;
    [SerializeField] private TabButton ObserversButton;

    List<PlayerInfo> playerInfoList;
    List<UserInfo> observersList;
    PlayerInfo myPanel;
    public List<DeckCard> myCards;

    [SerializeField] private TextMeshProUGUI displayNickname;
    [SerializeField] private TextMeshProUGUI hostStatus;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI currentPlayerTurnText;

    [SerializeField] private ChatManager chat;

    public Server server;
    public Client client;
    private int hostId;

    public int countForEndGame;
    public float timeToTurn;
    public float timeToDebate;
    public float timeToVote;
    public float timeToDelay;
    private Timer playerTimer;

    public User currentPlayer;
    public CurrentStage currentStage;

    #endregion

    public void ConvertToGameManager(List<User> users, User user, int hostId)
    {
        this.user = user;
        this.users = users;
        this.hostId = hostId;
    }

    void Start()
    {
        playerTimer = gameObject.GetComponent<Timer>();

        NetManager nm = FindObjectOfType<NetManager>();
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();
        myCards = new List<DeckCard>();

        ConvertToGameManager(nm.GetUsersList(), nm.GetUser(), nm.hostId);
        playerInfoList = new List<PlayerInfo>();

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
        timeToDebate = 10;
        timeToVote = 30;
        timeToDelay = 5;
        
        players = users;
        countForEndGame = players.Count/2;
        if(countForEndGame <= 1) countForEndGame = 2;

        votingList = new List<int>();
        observersList = new List<UserInfo>();
        NullVotes();

        chat.SetNickname(user.Nickname);
        displayNickname.text = user.Nickname;

        if (user.isHost) hostStatus.text = "HOST";
        else hostStatus.text = string.Empty;

        timerText.text = playerTimer.remainingTimeMin;
        currentStage = CurrentStage.PreGameDelay;

        isKicked = false;
        endgameShown = false;
        MessageProcessing.SetGameManager(this);

        MessageProcessing.ReadyForGame(user);
        if(server != null)
        {
            SetUserActivity(user.id, true);
        }
        if(client != null)
        {
            client.OnDisconnect += Disconnect;
        }

        Destroy(nm.gameObject);
        MessageProcessing.netManager = null;
    }

    public bool AllUsersReady()
    {
        foreach(User u in players)
            if(!u.isReady) return false;
        //Debug.Log("ALL PLAYERS READY!");
        return true;
    }

    public void StartGame() 
    {
        chat.SystemMessage("SYSTEM","Все подключены, начинаем игру...","03FF00");
        playerTimer.isRunning = true;
        playerTimer.SetTime(120);
        if(MessageProcessing.server!=null)
        {
            List<Category> allCards = new List<Category>();
            
            gameObject.GetComponent<Deck>().UpdateDeck("DefaultDeck.json");

            FileHandler.SaveToJSON<DeckCard>(new List<DeckCard>(), "newfile");
            allCards = gameObject.GetComponent<Deck>().GetCategories();
            //Debug.Log(allCards == null);
            List<PlayerKit> kits = new List<PlayerKit>();
            kits = SortDeckForKits(allCards,users);
            
            for(int i=1; i<users.Count; i++)
            {
                foreach(DeckCard card in kits[i].cardsKit)
                server.SendClient(hostId, users[i].id,
                MessageProcessing.ServerPlayerKitMsg(card));
            }
            //Debug.Log(kits[0].cardsKit.Count);
            foreach(DeckCard card in kits[0].cardsKit)
                SetCardToList(card);
            server.SendOther(MessageProcessing.ServerGameStartedMsg());
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

        MessageProcessing.chatManager.SystemMessage("CARD_DROP", 
        "Игрок " + players.Find(x=>x.id == user.id).Nickname + 
        " открывает карту \"" + card.name + 
        "\" из категории \"" + card.category + 
        "\"", "00BAFF");
        if(CardsButton.isSelected) Buttons.OnTabSelected(CardsButton);
        if(ObserversButton.isSelected) Buttons.OnTabSelected(ObserversButton);
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
        else if (currentStage != CurrentStage.TurnDelay)
        {
            timerText.color = Color.green;
            timerText.text = playerTimer.remainingTimeMin;
        }
        else 
        {
            timerText.color = Color.white;
            timerText.text = playerTimer.remainingTimeInt;
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
        switch (currentStage) {
        case CurrentStage.PreGameDelay:
            currentPlayerTurnText.text = "-/-/-";
            break;
            
        case CurrentStage.Turn:
            currentPlayerTurnText.text = currentPlayer.Nickname;
            break;

        case CurrentStage.TurnDelay:
            currentPlayerTurnText.text = "Определяется...";
            break;

        case CurrentStage.Debate:
            currentPlayerTurnText.text = "Все";
            break;
        
        case CurrentStage.Voting:
            currentPlayerTurnText.text = currentPlayer.Nickname;
            break;

        case CurrentStage.AfterVotingDelay:
            currentPlayerTurnText.text = "-/-/-";
            break;

        }
    }

    public string GetMyNick()
    {
        return user.Nickname;
    }

    public void SwitchTurn()
    {
        if(endgameShown) return;
        //Debug.Log("Turn Switched");
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
            playerInfoList.Find(x=>x.GetUser() == currentPlayer).SelectPlayer();
            playerTimer.SetAction(MakeRandomChoise);
            break;

        case CurrentStage.Voting:
            playerInfoList.Find(x=>x.GetUser() == currentPlayer).DeselectPlayer();
            if(players.FindIndex(x => x == currentPlayer) != players.Count-1)//if not last player
            {
                currentPlayer = players[players.FindIndex(x => x == currentPlayer)+1];//switch to next player
                playerInfoList.Find(x=>x.GetUser().id == currentPlayer.id).SelectPlayer();
                currentStage = CurrentStage.Voting;
                playerTimer.SetTime(timeToVote);
            }
            else // if last player (then go to next stage)
            {   
                playerInfoList.Find(x=>x.GetUser().id == currentPlayer.id).DeselectPlayer();
                if(FindPlayerToKick() != null)
                {
                    MessageProcessing.chatManager.SystemMessage("SYSTEM", "Выбран игрок " + FindPlayerToKick().Nickname, "FF0000");
                    currentPlayer = players[0];
                    currentStage = CurrentStage.AfterVotingDelay;
                    playerTimer.SetAction(SwitchTurn);
                    playerTimer.SetTime(timeToDelay);

                    playerInfoList.Find(x=>x.GetUser().id == FindPlayerToKick().id).transform.SetParent(kickedPanel.panel.transform);
                    playerInfoList.Find(x=>x.GetUser().id == FindPlayerToKick().id).transform.localScale = Vector3.one;
                    playerInfoList.Find(x=>x.GetUser().id == FindPlayerToKick().id).transform.position = Vector3.zero;
                    kickedPanel.Appear();
                }
                else
                {
                    MessageProcessing.chatManager.SystemMessage("SYSTEM", "Мнения разделились, голосование проводится повторно!", "FF0000");
                    currentPlayer = players[0];
                    currentStage = CurrentStage.Voting;
                    playerTimer.SetAction(MakeRandomChoise);
                    playerTimer.SetTime(timeToVote);
                }
            }
            break;

        case CurrentStage.AfterVotingDelay:
            Kick();
            kickedPanel.Disapear();
            if(players.Count <= countForEndGame) 
            {
                playerTimer.SetTime(timeToDelay);
                playerTimer.SetAction(EndGameSwitch);
            }
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
            while(players[randomIndex].id == user.id) randomIndex = random.Next(players.Count);
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

    public void VotingForPlayer(int id, int authorId)
    {
        MessageProcessing.chatManager.SystemMessage("VOTING_MANAGER", "Игрок " + 
        players.Find(x=>x.id == authorId).Nickname + " голосует за игрока " + 
        players.Find(x=>x.id == id).Nickname, "FF0000");

        players.Find(x=>x.id == id).votesFor++;
        SwitchTurn();
    }

    public void MyVoteFor(User user)
    {
        VotingForPlayer(user.id, this.user.id);
        MessageProcessing.VoteFor(this.user.id, user.id);
    }

    private User FindPlayerToKick()
    {
        int max = 0;
        foreach (User p in players)
            if (p.votesFor > max) max = p.votesFor;
        List<User> list = players.FindAll(x=>x.votesFor == max);

        if(list.Count == 1) return list[0];
        else return null;
    }

    public void Kick()
    {
        PlayerInfo playerToKick = kickedPanel.GetComponentInChildren<PlayerInfo>();
        
        if(playerToKick!=null)
        {
            chat.SystemMessage("KICK_MANAGER","Кикаем игрока "+ playerToKick.GetUser().Nickname, "FF0000");
            players.Remove(playerToKick.GetUser());

            Destroy(playerToKick.gameObject);
            playerInfoList.Remove(playerInfoList.Find(x=>x.GetUser().id == playerToKick.GetUser().id));
            
            UserInfo newObserver = Instantiate(userInfoPref) as UserInfo;
            
            newObserver.setNickname(playerToKick.GetUser().Nickname);
            newObserver.id = playerToKick.GetUser().id;
            newObserver.toggleHost(playerToKick.GetUser().isHost);
            newObserver.setNum(observersGrid.transform.childCount+1);

            newObserver.setPanelToList(observersGrid.gameObject);

            observersList.Add(newObserver);

            if(playerToKick.GetUser().id == user.id) // if this user kicked
            {
                foreach(Transform card in handPanel.transform)
                {
                    Destroy(card.gameObject);
                }
                foreach(Transform card in openedCardsPanel.transform)
                {
                    Destroy(card.gameObject);
                }
            
                isKicked = true;
            }

            if(!ObserversButton.isSelected) Buttons.OnTabSelected(ObserversButton);
        }
        NullVotes();
    }

    public void OnUserLeave(int id)
    {
        if(endgameShown) return;
        User disconnectedUser = users.Find(x=>x.id == id);
        if(disconnectedUser!=null)
        {
            MessageProcessing.chatManager.SystemMessage("SYSTEM", "Игрок " + disconnectedUser.Nickname + " покидает игру.", "03FF00");
            
            users.Remove(disconnectedUser);
            if(players.Find(x=>x.id == id) != null)
            {
                players.Remove(players.Find(x=>x.id == id));
                Destroy(playerInfoList.Find(x=>x.GetUser().id == id).gameObject);
                playerInfoList.Remove(playerInfoList.Find(x=>x.GetUser().id == id));
            }

            if(observersList.Find(x=>x.id == id) != null)
            {
                Debug.Log("Found observer");
                Destroy(observersList.Find(x=>x.id == id).gameObject);
                observersList.Remove(observersList.Find(x=>x.id == id));
            }
            if(players.Count <= countForEndGame) EndGameSwitch();
            else if(currentPlayer!=null && currentPlayer.id == id) SwitchTurn();
        }
        if(FindObjectOfType<EndgamePanel>() == null && disconnectedUser.id == currentPlayer.id) SwitchTurn();
    }

    public void EndGameSwitch()
    {
        endgamePanel.Appear();
        endgameShown = true;
        if(!isKicked) foreach(DeckCard card in myCards) 
        {
            DeckCardSerializable dcs;
            dcs.name = card.name;
            dcs.category = card.category;
            dcs.description = card.description;
            dcs.iconName = card.iconName;
            dcs.r = card.color.r;
            dcs.g = card.color.g;
            dcs.b = card.color.b;

            MessageProcessing.EndgameSend(dcs);
            SetPlayersEndgamePanel(user.id, dcs);
        }

        myCards.Clear();
        playerTimer.isRunning = false;
        playerTimer.SetTime(0);
    }

    public void SetPlayersEndgamePanel(int id, DeckCardSerializable dcs)
    {
        User playerToAdd = players.Find(x=> x.id == id);
        if(playerToAdd != null) endgamePanel.AddUserToEndgame(playerToAdd, dcs);
        else Debug.Log("Пользователь c id:" +id+ " не найден!");
    }

    public void Exit() 
    {
        Disconnect(null, EventArgs.Empty);
    }

    public void Disconnect(object s, EventArgs e)
    {
        MessageProcessing.Exit();
        SceneManager.LoadScene("MenuInterface");
    }
}