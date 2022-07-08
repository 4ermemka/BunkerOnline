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

    /////////////////////////////////////////////////////////////////////////////
    /*                                SERVER                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnNewPlayer(int conId, int host, Net_AddPlayer msg)
    {
        GM.AddNewPlayer(msg.Username, conId);
        Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
    }

    public void OnLeavePlayer(int conId, int host, Net_LeavePlayer msg)
    {
        GM.PausePlayer(msg.Username, conId);
        Debug.Log(string.Format("Player {0}, id: {1} is now inactive.", msg.Username, conId));
    }

    public void OnUpdatePlayer(int conId, int host, Net_UpdateCardPlayer msg)
    {
        GM.UpdateInformation(msg.Username, conId, msg.NewCardsOnTable);
        Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
    }

    /////////////////////////////////////////////////////////////////////////////
    /*                                CLIENT                                   */
    /////////////////////////////////////////////////////////////////////////////

    public void OnNewPlayer(Net_AddPlayer msg)
    {
        Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
    }

    public void OnLeavePlayer(Net_LeavePlayer msg)
    {
        Debug.Log(string.Format("Player {0} is now paused.", msg.Username));
    }

    public void OnUpdatePlayer(Net_UpdateCardPlayer msg)
    {
        Debug.Log(string.Format("Player {0} opened new card.", msg.Username));
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

}
