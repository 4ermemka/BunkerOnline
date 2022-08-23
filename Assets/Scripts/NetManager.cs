using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public enum CurrentNetState
{
    None,
    Server,
    Client
}

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
    private GameManager gm;

    public Server server;
    private Client client;
    public int hostId;

    public User user; 
    private List<User> lobbyList;

    #endregion

    public void Start()
    {
        DontDestroyOnLoad(this);
        user = new User();
        lobbyList = new List<User>();
        MessageProcessing.SetNetManager(this);

        MenuInterfaceManager.OnClickConnect += NetManager_ConnectPressed;
        MenuInterfaceManager.OnChooseClient += NetManager_StartClient;
        MenuInterfaceManager.OnChooseServer += NetManager_StartServer;
        MenuInterfaceManager.OnReturnToMenu += NetManager_DeleteNetworkObjects;
        MenuInterfaceManager.OnStartGame += ChangeScene;
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
        server.SendClient(hostId, e.conId, MessageProcessing.ServerSetGlobalId(e.conId));
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

        server.SendOther(MessageProcessing.ServerUsersListMsg(lobbyList));
    }

    #endregion

    #region UserManager

    public void AddNewUser(string Nickname, int conId)
    {
        User newUser = new User(conId, Nickname);
        lobbyList.Add(newUser);

        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void AddNewUser(string Nickname, int conId, bool host, int recHost)
    {
        User newUser = new User(conId, Nickname, host);
        lobbyList.Add(newUser);

        server.SendClient(recHost, conId, MessageProcessing.ServerUsersListMsg(lobbyList));
        server.SendOther(conId, recHost, MessageProcessing.ServerUsersListMsg(lobbyList));

        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void UpdateUsername(string Nickname)
    {
        user.SetNickname(Nickname);
        OnUserUpdated?.Invoke(this, new OnUserUpdatedEventArgs{newName = Nickname});
        User updatedUser = lobbyList.Find(x=> x.id == user.id);
        if(updatedUser!=null) Debug.Log("ID USER: " + user.id + " " + updatedUser.Nickname);

        if(updatedUser!=null) updatedUser.SetNickname(Nickname);

        MenuInterfaceManager.ClearLobby();
        MenuInterfaceManager.UpdateLobby(lobbyList);

        if(netState == CurrentNetState.Server)
        {
            server.SendOther(MessageProcessing.ServerUsersListMsg(lobbyList));
        }

        if(netState == CurrentNetState.Client)
        {
            client.SendServer(MessageProcessing.ClientUpdateUserMsg(user));
        }
    }

    public void UpdateUser(int conId, int host, string newNickname) 
    {
        User updatedUser = lobbyList.Find(x=> x.id == conId);
        if(updatedUser!=null) 
        {
            updatedUser.SetNickname(newNickname);
            MenuInterfaceManager.ClearLobby();
            MenuInterfaceManager.UpdateLobby(lobbyList);
        }

        if(netState == CurrentNetState.Server)
        {
            Debug.Log("SENDING OTHER");
            server.SendOther(conId, host, MessageProcessing.ServerUsersListMsg(lobbyList));
        }
    }

    public void SetGlobalId(int globalConId)
    {
        user.id = globalConId;
        Debug.Log("Global id is now " + user.id);
        client.SendServer(MessageProcessing.ClientNewUserMsg(user));
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
        server.OnData += MessageProcessing.OnData;
        server.OnConnect += ServerOnClientConnected;
        server.OnDisconnect += ServerOnClientDisconnection;
        netState = CurrentNetState.Server;
        user.ToggleHost(true);
        if(user == null) Debug.Log("NULL");
        lobbyList.Add(user);
        MenuInterfaceManager.UpdateLobby(lobbyList);
    }

    public void NetManager_StartClient(object sender, EventArgs e)
    {
        client = Instantiate(clientPref);
        client.OnData += MessageProcessing.OnData;
        client.OnConnect += ClientOnServerConnection;
        client.OnDisconnect += ClientOnServerDisonnection;
        netState = CurrentNetState.Client;
        user.ToggleHost(false);
    }

    public void NetManager_DeleteNetworkObjects(object sender, EventArgs e)
    {
        if(server!=null) 
        {
            server.OnData -= MessageProcessing.OnData;
            server.OnConnect -= ServerOnClientConnected;
            server.OnDisconnect -= ServerOnClientDisconnection;
            server.Shutdown();
        }
        if(client!=null) 
        {
            client.OnData -= MessageProcessing.OnData;
            client.OnConnect -= ClientOnServerConnection;
            client.OnDisconnect -= ClientOnServerDisonnection;
            client.Shutdown();
        }
        netState = CurrentNetState.None;
        ClearLobbyList();
        user.ToggleHost(false);
    }

    #endregion

    #region GetInfo

    public List<User> GetUsersList()
    {
        return lobbyList;
    }

    public User GetUser()
    {
        return user;
    }

    #endregion

    public void ChangeScene(object sender, EventArgs e)
    {
        if(server!=null) server.SendOther(MessageProcessing.ServerLobbyStartedMsg());
        SceneManager.LoadScene("GameInterface");
    }
}
