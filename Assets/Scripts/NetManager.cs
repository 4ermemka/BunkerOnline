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

#region UserClass

[Serializable]
public class User : MonoBehaviour
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
#endregion

public class NetManager : MonoBehaviour
{
    #region Events
    public event EventHandler<OnUserUpdatedEventArgs> OnUserUpdated;

    public class OnUserUpdatedEventArgs:EventArgs
    {
        public string newName;
    }
    #endregion

    #region NetmanagerFields

    private CurrentNetState netState = CurrentNetState.None;
    [SerializeField] private Server serverPref;
    [SerializeField] private Client clientPref;
    [SerializeField] private MenuInterfaceManager MenuInterfaceManager;

    private MessageProcessing messageProcessing;

    private Server server;
    private Client client;
    public int hostId;

    public User user; 
    private List<User> lobbyList;

    #endregion

    public void Start()
    {
        user = new User();
        lobbyList = new List<User>();
        messageProcessing = new MessageProcessing(this);

        MenuInterfaceManager.OnClickConnect += NetManager_ConnectPressed;
        MenuInterfaceManager.OnChooseClient += NetManager_StartClient;
        MenuInterfaceManager.OnChooseServer += NetManager_StartServer;
        MenuInterfaceManager.OnReturnToMenu += NetManager_DeleteNetworkObjects;
    }

    #region OnConnection  

    public void ClientOnServerConnection(object sender, Client.OnConnectEventArgs e)
    {
        MenuInterfaceManager.NewConnectionStatus("Connected!");
        hostId = e.hostId;
    }

    public void ServerOnClientConnected(object sender, Server.OnConnectEventArgs e)
    {
        Debug.Log("New player connected! Sending him list...");
        server.SendClient(hostId, e.conId, messageProcessing.ServerSetGlobalId(e.conId));
    }

    #endregion

    #region OnDisonnection  

    public void ClientOnServerDisonnection(object sender, EventArgs e)
    {
        Debug.Log("We have been disconnected from server!");
        ClearLobbyList();
        MenuInterfaceManager.SwitchToMainMenu();
    }

    public void ServerOnClientDisconnection(object sender, Server.OnDisconnectEventArgs e)
    {
        Debug.Log(string.Format("Player {0} was disconnected! ", e.conId));
        LeaveUser(e.conId);

        server.SendOther(messageProcessing.ServerUsersListMsg(lobbyList));
    }

    #endregion

    #region UserManager

    public void AddNewUser(string name, int conId)
    {
        User newUser = new User(conId, name);
        lobbyList.Add(newUser);

        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void AddNewUser(string name, int conId, bool host, int recHost)
    {
        User newUser = new User(conId, name, host);
        lobbyList.Add(newUser);

        server.SendClient(recHost, conId, messageProcessing.ServerUsersListMsg(lobbyList));
        server.SendOther(conId, recHost, messageProcessing.ServerUsersListMsg(lobbyList));

        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void UpdateUsername(string name)
    {
        user.SetName(name);
        OnUserUpdated?.Invoke(this, new OnUserUpdatedEventArgs{newName = name});
        User updatedUser = lobbyList.Find(x=> x.id == user.id);
        if(updatedUser!=null) Debug.Log("ID USER: " + user.id + " " + updatedUser.name);

        if(updatedUser!=null) updatedUser.SetName(name);

        MenuInterfaceManager.ClearLobby();
        MenuInterfaceManager.UpdateLobby(lobbyList);

        if(netState == CurrentNetState.Server)
        {
            server.SendOther(messageProcessing.ServerUsersListMsg(lobbyList));
        }

        if(netState == CurrentNetState.Client)
        {
            client.SendServer(messageProcessing.ClientUpdateUserMsg(user));
        }
    }

    public void UpdateUser(int conId, int host, string newName) 
    {
        User updatedUser = lobbyList.Find(x=> x.id == conId);
        if(updatedUser!=null) 
        {
            updatedUser.SetName(newName);
            MenuInterfaceManager.ClearLobby();
            MenuInterfaceManager.UpdateLobby(lobbyList);
        }

        if(netState == CurrentNetState.Server)
        {
            Debug.Log("SENDING OTHER");
            server.SendOther(conId, host, messageProcessing.ServerUsersListMsg(lobbyList));
        }
    }

    public void SetGlobalId(int globalConId)
    {
        user.id = globalConId;
        Debug.Log("Global id is now " + user.id);
        client.SendServer(messageProcessing.ClientNewUserMsg(user));
    }

    public void ClearLobbyList()
    {
        lobbyList.Clear();
        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void LeaveUser(int conId)
    {
        User deletingUser = lobbyList.Find(x => x.id == conId);
        if(deletingUser!=null) 
        {
            lobbyList.Remove(deletingUser);
            MenuInterfaceManager.UpdateLobby(lobbyList);
        }
        else Debug.Log("Err during deleting!");

        
    }

    public void UpdateUsersList(List<User> newList)
    {
        lobbyList = newList;
        MenuInterfaceManager.UpdateLobby(lobbyList);
        MenuInterfaceManager.SwitchToLobby();
        Debug.Log("List was updated!");
    }

    #endregion

    #region GameManager
    private bool CardsAreEmpty(string[] cards)
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
        if (conId < lobbyList.Count && !IsEmpty(lobbyList[conId].cards))
        {
            lobbyList[conId].SetName(name);
            lobbyList[conId].SetCards(cards);
            return true;
        }
        else return false;
    }*/

    #endregion
    
    #region CardEncrypting

    public string CardsToString(string[] cards)
    {
        string en_cards = ""; 
        int i;
        for (i = 0; i < cards.Length; i++)
            en_cards = en_cards + cards[i] + ";";
        return en_cards;
    }

    public string[] StringToCards(string en_cards)
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

    #endregion

    #region TriggerredFunctions

    //////////////////////////////////////////////////////////////////
    /////////////////// Reversed Ladder of events ////////////////////
    //////////////////////////////////////////////////////////////////
    
    public void NetManager_ConnectPressed(object sender, MenuInterfaceManager.OnClickConnectEventArgs e)
    {
        if(client!=null)
        {
            client.Connect(e.server_ip);
        }
        else Debug.Log("Err! No Client O_o");
    }

    public void NetManager_StartServer(object sender, EventArgs e)
    {
        server = Instantiate(serverPref);
        server.OnData += messageProcessing.OnData;
        server.OnConnect += ServerOnClientConnected;
        server.OnDisconnect += ServerOnClientDisconnection;
        netState = CurrentNetState.Server;
        user.ToggleHost(true);
        lobbyList.Add(user);
        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void NetManager_StartClient(object sender, EventArgs e)
    {
        client = Instantiate(clientPref);
        client.OnData += messageProcessing.OnData;
        client.OnConnect += ClientOnServerConnection;
        client.OnDisconnect += ClientOnServerDisonnection;
        netState = CurrentNetState.Client;
        user.ToggleHost(false);
    }

    public void NetManager_DeleteNetworkObjects(object sender, EventArgs e)
    {
        if(server!=null) 
        {
            server.OnData -= messageProcessing.OnData;
            server.OnConnect -= ServerOnClientConnected;
            server.OnDisconnect -= ServerOnClientDisconnection;
            server.Shutdown();
        }
        if(client!=null) 
        {
            client.OnData -= messageProcessing.OnData;
            client.OnConnect -= ClientOnServerConnection;
            client.OnDisconnect -= ClientOnServerDisonnection;
            client.Shutdown();
        }
        netState = CurrentNetState.None;
        ClearLobbyList();
        user.ToggleHost(false);
    }

    #endregion
}
