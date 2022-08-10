using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MessageProcessing
{
    #region MessageProcessingFields

    private NetManager netManager;
    private GameManager gameManager;
    private byte error;
    private const int BYTE_SIZE = 1024;

    #endregion

    #region MessageProcessingConstructors
    public MessageProcessing()
    {
        this.netManager = null;
    }

    public MessageProcessing(NetManager netManager)
    {
        this.netManager = netManager;
    }
    
    #endregion

    #region MessageTransformation
    public byte[] MakeBuffer(NetMsg msg)
    {
        //Place to hold data
        byte[] buffer = new byte[BYTE_SIZE];

        //Here you make byte array from your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        formatter.Serialize(ms, msg);

        return buffer;
    }

    public NetMsg MakeMessage(byte[] buffer)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        NetMsg msg = (NetMsg)formatter.Deserialize(ms);

        return msg;
    }

    #endregion

    #region ServerReadMsg
    
    /////////////////////////////////////////////////////////////////////////////
    /*                                SERVER                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnData (object sender, Server.OnDataEventArgs e)
    {
        NetMsg msg = MakeMessage(e.buffer);
        Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", e.conId, e.channel, e.host, msg.OP));
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
                OnUpdateChat((Net_UpdateChat)msg);
                break;
            case NetOP.PlayerVote:
                OnPlayerVote(e.conId, e.host, (Net_PlayerVote)msg);
                break;
            case NetOP.CastCardPlayer:
                //Soon          
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
      
    private void OnNewUser(int conId, int host, NetUser_Add msg)
    {
        Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
        netManager.AddNewUser(msg.Username, conId, false, host);
    }

    private void OnUpdateUser(int conId, int host, NetUser_UpdateInfo msg)
    {
        Debug.Log(string.Format("Player {1} changed nick to {0} ", msg.Username, conId));
        netManager.UpdateUser(conId, host, msg.Username);
    }

    /*private void OnUpdatePlayer(int conId, int host, Net_UpdateCardPlayer msg)
    {
        netManager.UpdatePlayerInformation(msg.Username, conId, msg.NewCardsOnTable);
        Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
    }*/

    private void OnUpdateChat(Net_UpdateChat msg)
    {
        Debug.Log("New message in chat!");
    }
    private void OnPlayerVote(int conId, int host, Net_PlayerVote msg)
    {
        Debug.Log("Player" + msg.user.id + "voted for player " + msg.id);
        gameManager.Voting(msg.id);
    }

    #endregion

    #region ClientReadMsg

    /////////////////////////////////////////////////////////////////////////////
    /*                                CLIENT                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnData (object sender, Client.OnDataEventArgs e)
    {   
        NetMsg msg = MakeMessage(e.buffer);
        Debug.Log(string.Format("Received msg. Msg type: {0}", msg.OP));
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

            case NetOP.UpdateCardPlayer:
                OnUpdatePlayer((Net_UpdateCardPlayer)msg);
                //make interface changes
                break;

            case NetOP.LeaveUser:
                OnLeaveUser((NetUser_Leave)msg);
                break;

            case NetOP.AllUsersInfo:
                Debug.Log("AllUsersInfo case!");
                SetListOfUsers((NetUser_AllUserList)msg);
                break;

            case NetOP.CastCardPlayer:
                //Soon          
                break;
            case NetOP.UpdateChat:
                OnUpdateChat((Net_UpdateChat)msg);
                break;
            case NetOP.UpdateVotingArray:
                OnUpdateVotingArray((Net_UpdateVotingArray)msg);
                break;
            case NetOP.PlayerVote:
                OnPlayerVote((Net_PlayerVote) msg);
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
    
    private void OnNewUser(NetUser_Add msg)
    {
        Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
    }

    private void OnUpdateUser(NetUser_UpdateInfo msg)
    {
        Debug.Log(string.Format("Other player changed nick to {0}", msg.Username));
        netManager.UpdateUser(msg.conId, msg.hostId, msg.Username);
    }

    private void OnLeaveUser(NetUser_Leave msg)
    {
        Debug.Log(string.Format("Player {0} disconnected.", msg.Username));
    }

    private void OnSetGlobalId(NetUser_SetGlobalId msg)
    {
        Debug.Log("Global id set to " + msg.globalConId);
        netManager.SetGlobalId(msg.globalConId);
    }

    private void OnUpdatePlayer(Net_UpdateCardPlayer msg)
    {
        Debug.Log(string.Format("Player {0} opened new card.", msg.Username));
    }

    private void SetListOfUsers(NetUser_AllUserList msg)
    {
        List<User> newList = new List<User>();
        foreach (User p in msg.users) newList.Add(p);
        netManager.UpdateUsersList(newList);
    }

    private void OnUpdateVotingArray(Net_UpdateVotingArray msg)
    {
        gameManager.SetVotingList(msg.votingArray.ToList());
    }

    private void OnPlayerVote (Net_PlayerVote msg) {
        Debug.Log("Player" + msg.user.id + "voted for player " + msg.id);
    }

    #endregion

    #region ServerWriteMsg

    public byte[] ServerUsersListMsg(List<User> lobbyList)
    {
        NetUser_AllUserList msg = new NetUser_AllUserList();
        msg.users = lobbyList.ToArray();

        return MakeBuffer(msg);
    }

    public byte[] ServerUpdateUser(User user, int hostId)
    {
        NetUser_UpdateInfo msg = new NetUser_UpdateInfo();
        msg.conId = user.id;
        msg.hostId = hostId;
        msg.Username = user.Nickname;

        return MakeBuffer(msg);
    }

    public byte[] ServerSetGlobalId(int globalConId)
    {
        NetUser_SetGlobalId msg = new NetUser_SetGlobalId();
        msg.globalConId = globalConId;

        return MakeBuffer(msg);
    }

    public byte[] ServerUpdateChat(User user, string message)
    {
        Net_UpdateChat msg = new Net_UpdateChat();
        msg.Nickname = user.Nickname;
        msg.message = message;

        return MakeBuffer(msg);
    }

    public byte[] ServerUpdateVotingArray(int[] votingArray)
    {
        Net_UpdateVotingArray msg = new Net_UpdateVotingArray();
        msg.votingArray = votingArray;

        return MakeBuffer(msg);
    }

    public byte[] ServerPlayerVoteMsg (User user, int id)
    {
        Net_PlayerVote msg = new Net_PlayerVote();
        msg.user = user;
        msg.id = id;

        return MakeBuffer(msg);
    }
    //need to add message about kick, player's vote (for Alina)
    #endregion

    #region ClientWriteMsg

    public byte[] ClientNewUserMsg(User user)
    {
        NetUser_Add msg = new NetUser_Add();
        msg.Username = user.Nickname;

        return MakeBuffer(msg);
    }

    public byte[] ClientUpdateUserMsg(User user)
    {
        NetUser_UpdateInfo msg = new NetUser_UpdateInfo();
        msg.Username = user.Nickname;
        msg.hostId = netManager.hostId;
        msg.conId = user.id;

        return MakeBuffer(msg);
    }

    public byte[] ClientUpdateChat(User user, string message)
    {
        Net_UpdateChat msg = new Net_UpdateChat();
        msg.Nickname = user.Nickname;
        msg.message = message;

        return MakeBuffer(msg);
    }

    public byte[] ClientPlayerVote(User user, int num_user)
    {
        Net_PlayerVote msg = new Net_PlayerVote();
        msg.id = num_user;

        return MakeBuffer(msg);
    }

    #endregion
}
