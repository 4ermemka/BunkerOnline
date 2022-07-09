using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

using System.Collections.Generic;

public class MessageProcessing
{
    private GameManager GM;
    private byte error;
    private const int BYTE_SIZE = 1024;

    public MessageProcessing()
    {
        this.GM = null;
    }

    public MessageProcessing(GameManager GM)
    {
        this.GM = GM;
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

    public void OnData (int conId, int channel, int host, byte[] buffer)
    {
        NetMsg msg = MakeMessage(buffer);
        Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", conId, channel, host, msg.OP));
        switch (msg.OP)
        {
            case NetOP.None:
                break;

            case NetOP.AddPlayer:
                OnNewPlayer(conId, host, (Net_AddPlayer)msg);
                break;

            case NetOP.LeavePlayer:
                OnLeavePlayer(conId, host, (Net_LeavePlayer)msg);
                break;

            case NetOP.UpdateCardPlayer:
                OnUpdatePlayer(conId, host, (Net_UpdateCardPlayer)msg);
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
        GM.AddNewPlayer(msg.Username, conId);
        Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
    }

    private void OnLeavePlayer(int conId, int host, Net_LeavePlayer msg)
    {
        GM.PausePlayer(msg.Username, conId);
        Debug.Log(string.Format("Player {0}, id: {1} is now inactive.", msg.Username, conId));
    }

    private void OnUpdatePlayer(int conId, int host, Net_UpdateCardPlayer msg)
    {
        GM.UpdateInformation(msg.Username, conId, msg.NewCardsOnTable);
        Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
    }

    /////////////////////////////////////////////////////////////////////////////
    /*                                CLIENT                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnData (byte[] buffer)
    {

        NetMsg msg = MakeMessage(buffer);
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

}
