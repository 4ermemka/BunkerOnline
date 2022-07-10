using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

using System.Collections.Generic;

public class MessageProcessing
{
    private GameManager gameManager;
    private byte error;
    private const int BYTE_SIZE = 1024;

    public MessageProcessing()
    {
        this.gameManager = null;
    }

    public MessageProcessing(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

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

            case NetOP.AddPlayer:
                OnNewPlayer(e.conId, e.host, (Net_AddPlayer)msg);
                break;

            case NetOP.LeavePlayer:
                OnLeavePlayer(e.conId, e.host, (Net_LeavePlayer)msg);
                break;

            case NetOP.UpdateCardPlayer:
                OnUpdatePlayer(e.conId, e.host, (Net_UpdateCardPlayer)msg);
                break;

            case NetOP.CastCardPlayer:
                //Soon          
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
      
    private void OnNewPlayer(int conId, int host, Net_AddPlayer msg)
    {
        gameManager.AddNewPlayer(msg.Username, conId, false, string.Empty);
        Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
    }

    private void OnLeavePlayer(int conId, int host, Net_LeavePlayer msg)
    {
        gameManager.PausePlayer(msg.Username, conId);
        Debug.Log(string.Format("Player {0}, id: {1} is now inactive.", msg.Username, conId));
    }

    private void OnUpdatePlayer(int conId, int host, Net_UpdateCardPlayer msg)
    {
        gameManager.UpdateInformation(msg.Username, conId, msg.NewCardsOnTable);
        Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
    }

    /////////////////////////////////////////////////////////////////////////////
    /*                                CLIENT                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnData (object sender, Client.OnDataEventArgs e)
    {   
        NetMsg msg = MakeMessage(e.buffer);
        Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", msg.OP));
        //Here write what to do
        switch (msg.OP)
        {
            case NetOP.None:
                break;

            case NetOP.AddPlayer:
                OnNewPlayer((Net_AddPlayer)msg);
                break;

            case NetOP.LeavePlayer:
                OnLeavePlayer((Net_LeavePlayer)msg);
                break;

            case NetOP.UpdateCardPlayer:
                OnUpdatePlayer((Net_UpdateCardPlayer)msg);
                //make interface changes
                break;

            case NetOP.AllPlayersInfo:
                SetListOfPlayers((Net_AllPlayerList)msg);
                break;

            case NetOP.CastCardPlayer:
                //Soon          
                break;

            default:
                Debug.Log("Unexpected msg type!");
                break;
        }
    }
    
    private void OnNewPlayer(Net_AddPlayer msg)
    {
        Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
    }

    private void OnLeavePlayer(Net_LeavePlayer msg)
    {
        Debug.Log(string.Format("Player {0} is now paused.", msg.Username));
    }

    private void OnUpdatePlayer(Net_UpdateCardPlayer msg)
    {
        Debug.Log(string.Format("Player {0} opened new card.", msg.Username));
    }

    private void SetListOfPlayers(Net_AllPlayerList msg)
    {
        List<Player> newList = new List<Player>();
        foreach (Player p in msg.players) newList.Add(p);
        gameManager.UpdatePlayersList(newList);
    }

}
