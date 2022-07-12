using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MessageProcessing
{
    #region MessageProcessingFields

    private GameManager gameManager;
    private byte error;
    private const int BYTE_SIZE = 1024;

    #endregion

    #region MessageProcessingConstructors
    public MessageProcessing()
    {
        this.gameManager = null;
    }

    public MessageProcessing(GameManager gameManager)
    {
        this.gameManager = gameManager;
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
                OnNewUser(e.conId, e.host, (Net_AddUser)msg);
                break;

            case NetOP.UpdateUser:
                OnUpdateUser(e.conId, e.host, (Net_UpdateUser)msg);
                break;

            /*case NetOP.UpdateCardPlayer:
                OnUpdatePlayer(e.conId, e.host, (Net_UpdateCardPlayer)msg);
                break;*/

            case NetOP.CastCardPlayer:
                //Soon          
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
      
    private void OnNewUser(int conId, int host, Net_AddUser msg)
    {
        Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
        gameManager.AddNewUser(msg.Username, conId, false, host);
    }

    private void OnUpdateUser(int conId, int host, Net_UpdateUser msg)
    {
        Debug.Log(string.Format("Player {1} changed nick to {0} ", msg.Username, conId));
        gameManager.UpdateUser(conId, host, msg.Username);
    }

    /*private void OnUpdatePlayer(int conId, int host, Net_UpdateCardPlayer msg)
    {
        gameManager.UpdatePlayerInformation(msg.Username, conId, msg.NewCardsOnTable);
        Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
    }*/

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
                OnSetGlobalId((Net_SetGlobalId)msg);
                break;

            case NetOP.AddUser:
                OnNewUser((Net_AddUser)msg);
                break;

            case NetOP.UpdateUser:
                OnUpdateUser((Net_UpdateUser)msg);
                break;

            case NetOP.UpdateCardPlayer:
                OnUpdatePlayer((Net_UpdateCardPlayer)msg);
                //make interface changes
                break;

            case NetOP.LeaveUser:
                OnLeaveUser((Net_LeaveUser)msg);
                break;

            case NetOP.AllUsersInfo:
                Debug.Log("AllUsersInfo case!");
                SetListOfUsers((Net_AllUserList)msg);
                break;

            case NetOP.CastCardPlayer:
                //Soon          
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
    
    private void OnNewUser(Net_AddUser msg)
    {
        Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
    }

    private void OnUpdateUser(Net_UpdateUser msg)
    {
        Debug.Log(string.Format("Other player changed nick to {0}", msg.Username));
        gameManager.UpdateUser(msg.conId, msg.hostId, msg.Username);
    }

    private void OnLeaveUser(Net_LeaveUser msg)
    {
        Debug.Log(string.Format("Player {0} disconnected.", msg.Username));
    }

    private void OnSetGlobalId(Net_SetGlobalId msg)
    {
        Debug.Log("Global id set to " + msg.globalConId);
        gameManager.SetGlobalId(msg.globalConId);
    }

    private void OnUpdatePlayer(Net_UpdateCardPlayer msg)
    {
        Debug.Log(string.Format("Player {0} opened new card.", msg.Username));
    }

    private void SetListOfUsers(Net_AllUserList msg)
    {
        List<User> newList = new List<User>();
        foreach (User p in msg.users) newList.Add(p);
        gameManager.UpdateUsersList(newList);
    }

    #endregion

    #region ServerWriteMsg

    public byte[] ServerUsersListMsg(List<User> lobbyList)
    {
        Net_AllUserList msg = new Net_AllUserList();
        msg.users = lobbyList.ToArray();

        return MakeBuffer(msg);
    }

    public byte[] ServerUdateUser(User user, int hostId)
    {
        Net_UpdateUser msg = new Net_UpdateUser();
        msg.conId = user.id;
        msg.hostId = hostId;
        msg.Username = user.name;

        return MakeBuffer(msg);
    }

    public byte[] ServerSetGlobalId(int globalConId)
    {
        Net_SetGlobalId msg = new Net_SetGlobalId();
        msg.globalConId = globalConId;

        return MakeBuffer(msg);
    }

    #endregion

    #region ClientWriteMsg

    public byte[] ClientNewUserMsg(User user)
    {
        Net_AddUser msg = new Net_AddUser();
        msg.Username = user.name;

        return MakeBuffer(msg);
    }

    public byte[] ClientUpdateUserMsg(User user)
    {
        Net_UpdateUser msg = new Net_UpdateUser();
        msg.Username = user.name;
        msg.hostId = gameManager.hostId;
        msg.conId = user.id;

        return MakeBuffer(msg);
    }
    
    #endregion
}
