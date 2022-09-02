using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class MessageProcessing
{
    #region MessageProcessingFields
    public static Server server;
    public static Client client;
    public static NetManager netManager;
    public static GameManager gameManager;
    public static ChatManager chatManager;
    private static byte error;
    private const int BYTE_SIZE = 1024;

    #endregion

    #region MessageProcessingConstructors

    static MessageProcessing()
    {
        server = null;
        client = null;
        netManager = null;
        gameManager = null;
        chatManager = null;
        error = 0;
    }
    public static void SetNetManager(NetManager nm)
    {
        netManager = nm;
    }

    public static void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    public static void SetChatManager(ChatManager cm)
    {
        chatManager = cm;
    }

    #endregion

    #region MessageTransformation
    public static byte[] MakeBuffer(NetMsg msg)
    {
        //Place to hold data
        byte[] buffer = new byte[BYTE_SIZE];

        //Here you make byte array from your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        formatter.Serialize(ms, msg);

        return buffer;
    }
    public static NetMsg MakeMessage(byte[] buffer)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        NetMsg msg = (NetMsg)formatter.Deserialize(ms);

        return msg;
    }

    #endregion

    #region OnConnection

    public static void ClientOnServerConnection(object sender, Client.OnConnectEventArgs e)
    {
        if(netManager != null)
        {
            netManager.MenuInterfaceManager.NewConnectionStatus("Connected!");
            netManager.hostId = e.hostId;
            client.SendServer(MessageProcessing.AddUser(netManager.user));
            netManager.MenuInterfaceManager.SwitchToLobby();
        }
    }

    public static void ServerOnClientConnected(object sender, Server.OnConnectEventArgs e)
    {
        //Debug.Log("New player connected! Sending him list...");
        if(netManager != null) server.SendClient(netManager.hostId, e.conId, MessageProcessing.ServerSetGlobalId(e.conId));
    }

    #endregion

    #region OnDisonnection

    public static void ClientOnServerDisonnection(object sender, EventArgs e)
    {
        //Debug.Log("We have been disconnected from server!");
        if (gameManager != null)
            SceneManager.LoadScene("MenuInterface");

        if (netManager != null) 
        {
            netManager.ClearLobbyList();
            netManager.MenuInterfaceManager.SwitchToMainMenu();
        }
    }

    public static void ServerOnClientDisconnection(object sender, Server.OnDisconnectEventArgs e)
    {
        //Debug.Log(string.Format("Player {0} was disconnected! ", e.conId));
        if(netManager != null)
            netManager.LeaveUser(e.conId);
        if (gameManager != null)
            gameManager.OnUserLeave(e.conId);

        SendLeaveUser(e.conId);
    }

    #endregion

    #region ServerReadMsg

    /////////////////////////////////////////////////////////////////////////////
    /*                                SERVER                                   */
    /////////////////////////////////////////////////////////////////////////////

    public static void ServerOnData(object sender, Server.OnDataEventArgs e)
    {
        NetMsg msg = MakeMessage(e.buffer);
        //Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", e.conId, e.channel, e.host, msg.OP));
        switch (msg.OP)
        {
            case NetOP.None:
                break;

            case NetOP.AddUser:
                OnNewUser(e.conId, e.host, (NetUser_Add)msg);
                break;

            case NetOP.UpdateUser:
                OnUpdateUser(e.conId, e.host, (NetUser_UpdateInfo)msg);
                break;

            case NetOP.UpdateChat:
                OnUpdateChat(e.conId, e.host, (Net_UpdateChat)msg);
                break;
            case NetOP.PlayerVote:
                OnPlayerVote(e.conId, e.host, (Net_PlayerVote)msg);
                break;

            case NetOP.CastCard:
                OnPlayerCastCard(e.conId, e.host, (Net_CastCard)msg);
                break;

            case NetOP.ReadyForGame:
                OnPlayerReadyForGame(e.conId, e.host, (Net_ReadyForGame)msg);
                break;

            case NetOP.SwitchTurn:
                OnSwitchTurn(e.conId, e.host, (Net_SwitchTurn)msg);
                break;

            default:
                //Debug.Log("Unexpected msg type!");
                break;
        }
    }

    private static void OnNewUser(int conId, int host, NetUser_Add msg)
    {
        if(netManager != null) 
            netManager.AddNewUser(msg.Username, conId);
        msg.id = conId;
        server.SendOther(conId, host, MakeBuffer(msg));
        server.SendClient(host, conId, ServerUsersListMsg(netManager.GetUsersList()));
    }

    private static void OnUpdateUser(int conId, int host, NetUser_UpdateInfo msg)
    {
        //Debug.Log(string.Format("Player {1} changed nick to {0} ", msg.Username, conId));
        netManager.UpdateUser(conId, msg.Nickname);
        server.SendOther(conId, host, MakeBuffer(msg));
    }

    private static void OnPlayerCastCard(int conId, int host, Net_CastCard msg)
    {
        //Debug.Log("User" + msg.user.Nickname + " casted "+ msg.card.name);
        gameManager.AddCardToPlayerPanel(msg.user, msg.card);
        gameManager.server.SendOther(conId, host, MakeBuffer(msg));
    }

    private static void OnUpdateChat(int conId, int host, Net_UpdateChat msg)
    {
        //Debug.Log("New message in chat!");
        chatManager.AddMessage(msg.Nickname, msg.message);
        gameManager.server.SendOther(conId, host, MakeBuffer(msg));
    }

    private static void OnPlayerVote(int conId, int host, Net_PlayerVote msg)
    {
        //Debug.Log("Player" + msg.user.id + "voted for player " + msg.id);
        gameManager.server.SendOther(conId, host, MakeBuffer(msg));
        gameManager.VotingForPlayer(msg.id);
    }

    private static void OnPlayerReadyForGame(int conId, int host, Net_ReadyForGame msg)
    {
        //Debug.Log(msg.userId +" connected well!");
        gameManager.SetUserActivity(conId, true);
    }

    private static void OnSwitchTurn(int conId, int host, Net_SwitchTurn msg)
    {
        gameManager.SwitchTurn();
        gameManager.server.SendOther(conId, host, MakeBuffer(msg));
    }

    #endregion

    #region ClientReadMsg

    /////////////////////////////////////////////////////////////////////////////
    /*                                CLIENT                                   */
    /////////////////////////////////////////////////////////////////////////////

    public static void ClientOnData(object sender, Client.OnDataEventArgs e)
    {
        NetMsg msg = MakeMessage(e.buffer);
        ////Debug.Log(string.Format("Received msg. Msg type: {0}", msg.OP));
        //Here write what to do
        switch (msg.OP)
        {
            case NetOP.None:
                break;

            case NetOP.SetGlobalId:
                OnSetGlobalId((NetUser_SetGlobalId)msg);
                break;

            case NetOP.AddUser:
                OnNewUser((NetUser_Add)msg);
                break;

            case NetOP.UpdateUser:
                OnUpdateUser((NetUser_UpdateInfo)msg);
                break;

            case NetOP.LeaveUser:
                OnLeaveUser((NetUser_Leave)msg);
                break;

            case NetOP.AllUsersInfo:
                SetListOfUsers((NetUser_AllUserList)msg);
                break;

            case NetOP.CastCard:
                OnPlayerCastCard((Net_CastCard)msg);
                break;

            case NetOP.UpdateChat:
                OnUpdateChat((Net_UpdateChat)msg);
                break;

            case NetOP.VotingForPlayer:
                OnUpdateVotingArray((Net_VotingForPlayer)msg);
                break;

            case NetOP.PlayerVote:
                OnPlayerVote((Net_PlayerVote)msg);
                break;

            case NetOP.LobbyStarted:
                OnLobbyStarted();
                break;

            case NetOP.GameStarted:
                OnGameStarted();
                break;

            case NetOP.PlayerKit:
                OnPlayerKit((Net_PlayerKit)msg);
                break;

            case NetOP.SwitchTurn:
                OnSwitchTurn((Net_SwitchTurn)msg);
                break;

            case NetOP.ServerReady:
                OnServerReady();
                break;

            default:
                ////Debug.Log("Unexpected msg type!");
                break;
        }
    }

    private static void OnNewUser(NetUser_Add msg)
    {
        ////Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
        netManager.AddNewUser(msg.Username, msg.id);
    }

    private static void OnUpdateUser(NetUser_UpdateInfo msg)
    {
        ////Debug.Log(string.Format("Other player changed nick to {0}", msg.Username));
        netManager.UpdateUser(msg.id, msg.Nickname);
    }

    private static void OnLeaveUser(NetUser_Leave msg)
    {
        Debug.Log(string.Format("Player {0} disconnected.", msg.id));
        if (netManager != null) netManager.LeaveUser(msg.id);
        if (gameManager != null) gameManager.OnUserLeave(msg.id);
    }

    private static void OnSetGlobalId(NetUser_SetGlobalId msg)
    {
        ////Debug.Log("Global id set to " + msg.globalConId);
        netManager.SetGlobalId(msg.globalConId);
    }

    private static void OnUpdateChat(Net_UpdateChat msg)
    {
        //Debug.Log("New message in chat!");
        chatManager.AddMessage(msg.Nickname, msg.message);
    }

    private static void SetListOfUsers(NetUser_AllUserList msg)
    {
        Debug.Log("Got users, count: " + msg.users.ToList<User>().Count);
        List<User> newList = new List<User>();
        foreach (User p in msg.users) newList.Add(p);
        netManager.UpdateUsersList(newList);
    }

    private static void OnUpdateVotingArray(Net_VotingForPlayer msg)
    {
        gameManager.VotingForPlayer(msg.id);
    }

    private static void OnPlayerVote(Net_PlayerVote msg)
    {
        //Debug.Log("Player" + msg.user.id + "voted for player " + msg.id);
        gameManager.VotingForPlayer(msg.id);
    }

    private static void OnPlayerKit(Net_PlayerKit msg)
    {
        //Debug.Log("Got card!");
        //Debug.Log("GM EMPTY = "+gameManager==null);
        DeckCard dc = new DeckCard();
        dc.name = msg.card.name;
        dc.category = msg.card.category;
        dc.description = msg.card.description;
        dc.iconName = msg.card.iconName;
        Color color = new Color(msg.card.r, msg.card.g, msg.card.b, 1.0f);
        dc.color = color;

        gameManager.SetCardToList(dc);
    }

    private static void OnLobbyStarted()
    {
        netManager.ChangeScene(null, EventArgs.Empty);
    }

    private static void OnGameStarted()
    {
        gameManager.StartGame();
    }

    private static void OnServerReady()
    {
        //gameManager.client.SendServer(ClientReadyForGame(gameManager.user.id));
    }

    private static void OnPlayerCastCard(Net_CastCard msg)
    {
        //Debug.Log("User" + msg.user.Nickname + " casted "+ msg.card.name);
        gameManager.AddCardToPlayerPanel(msg.user, msg.card);
    }

    private static void OnSwitchTurn(Net_SwitchTurn msg)
    {
        gameManager.SwitchTurn();
    }

    #endregion

    #region ServerWriteMsg
    public static byte[] ServerUsersListMsg(List<User> lobbyList)
    {
        Debug.Log("Sending List");
        NetUser_AllUserList msg = new NetUser_AllUserList();
        msg.users = lobbyList.ToArray();

        return MakeBuffer(msg);
    }

    public static byte[] ServerSetGlobalId(int globalConId)
    {
        NetUser_SetGlobalId msg = new NetUser_SetGlobalId();
        msg.globalConId = globalConId;

        return MakeBuffer(msg);
    }

    public static byte[] ServerUpdateChat(User user, string message)
    {
        Net_UpdateChat msg = new Net_UpdateChat();
        msg.Nickname = user.Nickname;
        msg.message = message;

        return MakeBuffer(msg);
    }

    public static byte[] ServerPlayerVoteMsg(User user, int id)
    {
        Net_PlayerVote msg = new Net_PlayerVote();
        msg.user = user;
        msg.id = id;

        return MakeBuffer(msg);
    }

    public static byte[] ServerPlayerKitMsg(DeckCard card)
    {
        Net_PlayerKit msg = new Net_PlayerKit();
        DeckCardSerializable dcs;

        dcs.name = card.name;
        dcs.category = card.category;
        dcs.description = card.description;
        dcs.iconName = card.iconName;
        dcs.r = card.color.r;
        dcs.g = card.color.g;
        dcs.b = card.color.b;

        msg.card = dcs;
        return MakeBuffer(msg);
    }

    public static byte[] ServerLobbyStartedMsg()
    {
        Net_LobbyStarted msg = new Net_LobbyStarted();

        return MakeBuffer(msg);
    }

    public static byte[] ServerGameStartedMsg()
    {
        Net_GameStarted msg = new Net_GameStarted();

        return MakeBuffer(msg);
    }

    public static byte[] ServerReadyForGame()
    {
        Net_ServerReady msg = new Net_ServerReady();

        return MakeBuffer(msg);
    }

    #endregion

    #region ClientWriteMsg
    public static byte[] ClientNewUserMsg(User user)
    {
        NetUser_Add msg = new NetUser_Add();
        msg.Username = user.Nickname;

        return MakeBuffer(msg);
    }
    public static byte[] UpdateUserMsg(User user)
    {
        NetUser_UpdateInfo msg = new NetUser_UpdateInfo();
        msg.Nickname = user.Nickname;
        msg.id = user.id;

        return MakeBuffer(msg);
    }

    public static byte[] ClientUpdateChat(User user, string message)
    {
        Net_UpdateChat msg = new Net_UpdateChat();
        msg.Nickname = user.Nickname;
        msg.message = message;

        return MakeBuffer(msg);
    }

    public static byte[] ClientReadyForGame(int id)
    {
        Net_ReadyForGame msg = new Net_ReadyForGame();
        msg.userId = id;

        return MakeBuffer(msg);
    }

    #endregion

    #region Client/Server Operations

    public static byte[] AddUser(User user)
    {
        NetUser_Add msg = new NetUser_Add();
        msg.id = user.id;
        msg.Username = user.Nickname;

        return MakeBuffer(msg);
    }

    public static byte[] LeaveUser(int id)
    {
        NetUser_Leave msg = new NetUser_Leave();
        msg.id = id;

        return MakeBuffer(msg);
    }

    public static byte[] CastCardMsg(User user, Card card)
    {
        Net_CastCard msg = new Net_CastCard();
        msg.user = user;
        msg.card = card.AttributeToDeckCardSerializable();

        return MakeBuffer(msg);
    }

    public static byte[] WriteChatMsg(string user, string message)
    {
        Net_UpdateChat msg = new Net_UpdateChat();
        msg.Nickname = user;
        msg.message = message;

        return MakeBuffer(msg);
    }

    public static byte[] PlayerVote(User author, int num_user)
    {
        Net_PlayerVote msg = new Net_PlayerVote();
        msg.user = author;
        msg.id = num_user;

        return MakeBuffer(msg);
    }

    public static byte[] SwitchTurn()
    {
        Net_SwitchTurn msg = new Net_SwitchTurn();

        return MakeBuffer(msg);
    }

    public static void UpdateUser(User user)
    {
        if (client != null)
        {
            client.SendServer(UpdateUserMsg(user));
        }

        if (server != null)
        {
            server.SendOther(UpdateUserMsg(user));
        }
    }

    public static void SendLeaveUser(int id)
    {
        server.SendOther(LeaveUser(id));
    }

    public static void ReadyForGame(User user)
    {
        if (gameManager.client != null)
        {
            gameManager.client.SendServer(ClientReadyForGame(user.id));
        }

        if (gameManager.server != null)
        {
            gameManager.server.SendOther(ServerReadyForGame());
        }
    }

    public static void AddCardToPanel(User user, OpenedCardsPanel.OnCastCardEventArgs e)
    {
        if (gameManager.client != null) gameManager.client.SendServer(CastCardMsg(user, e.card));
        if (gameManager.server != null) gameManager.server.SendOther(CastCardMsg(user, e.card));
    }

    public static void VoteFor(User user)
    {
        if (gameManager.client != null) gameManager.client.SendServer(PlayerVote(user, user.id));
        if (gameManager.server != null) gameManager.server.SendOther(PlayerVote(user, user.id));
    }

    public static void ChangeScene()
    {
        if (server != null) server.SendOther(ServerLobbyStartedMsg());
    }

    public static void Exit()
    {
        if (gameManager.server != null)
        {
            gameManager.server.KickAll();
            gameManager.server.Shutdown();
        }
        if (gameManager.client != null)
        {
            gameManager.client.Disconnect();
        }
    }

    #endregion
}