using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum CurrentStage 
{
    Turn,
    Voting,
    Kick
}

public enum CurrentNetState
{
    None,
    Server,
    Client
}


[Serializable]
public class User
{
    public bool isHost;
    public int id;
    public string name;
    public bool isReady;

    public User()
    {
        isHost = false;
        id = 0;
        name = "PLAYER";
        isReady = false;
    }

    public User(int id, string name)
    {
        this.isHost = false;
        this.id = id;
        this.name = name;
        isReady = false;
    }

    public User(int id, string name, bool host)
    {
        this.isHost = host;
        this.id = id;
        this.name = name;
        isReady = false;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void ToggleHost(bool host)
    {
        this.isHost = host;
    }
}

[Serializable]
public class Player : User
{
    public bool isActive;
    public string[] cards;

    public Player() : base() 
    {
        isActive = true;
        cards = new string[10];
    }

    public Player(int id, string name) : base(id, name)
    {
        this.isActive = true;
        this.cards = new string[10];
    }

    public Player(int id, string name, bool host,string[] cards) : base(id, name, host)
    {
        this.isActive = true;
        this.cards = cards;
    }

    //public static implicit operator Player(User user) => new Player (user.id, user.name);

    public void SetCards(string[] cards)
    {
        this.cards = cards;
    }
    public void SetStatus(bool isActive)
    {
        this.isActive = isActive;
    }
}

public class GameManager : MonoBehaviour
{
    private CurrentNetState netState = CurrentNetState.None;
    [SerializeField] private Server serverPref;
    [SerializeField] private Client clientPref;

    private MessageProcessing messageProcessing;

    private Server server;
    private Client client;

    private User user; 
    private List<User> connectedList;

    [SerializeField] private InterfaceMG interfaceMG;

    public void Start()
    {
        user = new User();
        connectedList = new List<User>();
        messageProcessing = new MessageProcessing(this);

        interfaceMG.OnClickConnect += GameManager_ConnectPressed;
        interfaceMG.OnChooseClient += GameManager_StartClient;
        interfaceMG.OnChooseServer += GameManager_StartServer;
        interfaceMG.OnReturnToMenu += GameManager_DeleteNetworkObjects;
    }

    public void OnServerConnection(object sender, Client.OnConnectEventArgs e)
    {
        interfaceMG.connectionStatusText.text = "Connected!";
        Net_AddUser msg = new Net_AddUser();
        msg.Username = user.name;

        byte[] buffer = messageProcessing.MakeBuffer(msg);

        client.SendServer(buffer);
    }

    public void OnClientConnection(object sender, Server.OnConnectEventArgs e)
    {
        Net_AllUserList msg = new Net_AllUserList();
        msg.users = connectedList.ToArray();

        byte[] buffer = messageProcessing.MakeBuffer(msg);

        server.SendClient(e.host, e.conId, buffer);
    }

    public void AddNewUser(string name, int conId)
    {
        User newUser = new User(conId, name);
        connectedList.Add(newUser);

        UpdateLobby();
    }

    public void AddNewUser(string name, int conId, bool host)
    {
        User newUser = new User(conId, name, host);
        connectedList.Add(newUser);

        UpdateLobby();
    }

    public void CloseLobby()
    {
        connectedList.Clear();
        UpdateLobby();
    }

    public void PauseUser(string name, int conId)
    {
        //pause code
    }

    public void UpdateUsersList(List<User> newList)
    {
        connectedList = newList;
        UpdateLobby();
        interfaceMG.SwitchToLobby();
        Debug.Log("List was updated!");
    }

    private void UpdateLobby()
    {
        for(int i = 0; i < connectedList.Count; i++) 
            interfaceMG.AddUserToList(connectedList[i].name, i+1, connectedList[i].isHost);
    }

    private bool IsEmpty(string[] cards)
    {
        bool flag = false; int i;
        for (i = 0; i < cards.Length; i++)
            if (cards[i] == string.Empty) flag = true;
        if (!flag && name == string.Empty)
            return true;
        else return false;
    }

    /*public bool UpdatePlayerInformation(string name, int conId, string cardsNew)
    {
        string[] cards;
        cards = Decryption(cardsNew);
        if (conId < connectedList.Count && !IsEmpty(connectedList[conId].cards))
        {
            connectedList[conId].SetName(name);
            connectedList[conId].SetCards(cards);
            return true;
        }
        else return false;
    }*/

    public string Encryption(string[] cards)
    {
        string en_cards = ""; 
        int i;
        for (i = 0; i < cards.Length; i++)
            en_cards = en_cards + cards[i] + ";";
        return en_cards;
    }

    public string[] Decryption(string en_cards)
    {
        int i, count_separator = 0, k = 0;
        for (i = 0; i < en_cards.Length; i++)
            if (en_cards[i] == ';') count_separator++;
        string[] dc_cards = new string[count_separator];
        for (i = 0; i < count_separator; i++)
        {
            while (en_cards[k] != ';')
            {
                dc_cards[i] += en_cards[k];
                k++;
            }
            k++;
        }
        return dc_cards;
    }

    //////////////////////////////////////////////////////////////////
    /////////////////// Reversed Ladder of events ////////////////////
    //////////////////////////////////////////////////////////////////

    public void GameManager_ConnectPressed(object sender, InterfaceMG.OnClickConnectEventArgs e)
    {
        if(client!=null)
        {
            client.Connect(e.server_ip);
        }
        else Debug.Log("Err! No Client O_o");
    }

    public void GameManager_StartServer(object sender, EventArgs e)
    {
        server = Instantiate(serverPref);
        server.OnData += messageProcessing.OnData;
        server.OnConnect += OnClientConnection;
        netState = CurrentNetState.Server;
    }

    public void GameManager_StartClient(object sender, EventArgs e)
    {
        client = Instantiate(clientPref);
        client.OnData += messageProcessing.OnData;
        client.OnConnect += OnServerConnection;
        netState = CurrentNetState.Client;
    }

    public void GameManager_DeleteNetworkObjects(object sender, EventArgs e)
    {
        if(server!=null) 
        {
            server.Shutdown();
            server.OnData -= messageProcessing.OnData;
            Destroy(server.gameObject);
        }
        if(client!=null) 
        {
            client.Shutdown();
            client.OnData -= messageProcessing.OnData;
            Destroy(client.gameObject);
        }
        netState = CurrentNetState.None;
    }

}