using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

using System.Collections.Generic;

public class Server : MonoBehaviour
{
    public GameManager GM;
    public MessageProcessing messageProcessing;

    private List<int> ConnectedUsersId = new List<int>();

    private const int BYTE_SIZE = 1024;

    private const int MAX_USER = 100;
    private const int PORT = 28120;
    private const int WEB_PORT = 28121;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;

    private bool isStarted;
    private byte error;

    private void Start() // При старте выполнить код ниже
    {
        messageProcessing = new MessageProcessing();
        DontDestroyOnLoad(gameObject); // гарантия перехода между сценами
        Init();
    }

    public void SetGamemanager(GameManager gm)
    {
        GM = gm;
        messageProcessing = new MessageProcessing(GM);
    }

    private void Update() //каждый кадр
    {
        UpdateMessagePump();
    }

    private void Init() // стартануть сервер
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        //SERVER ONLY FROM HERE
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));

        isStarted = true;
    }

    public void  Shutdown() 
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump() //ожидание и принятие сообщений
    {
        if(!isStarted) return;

        int recHostId; // is this Web/SA host?
        int connectionId; //which user is sending me this?
        int channelId; // Which lane is he sending me this from

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, recBuffer.Length, out dataSize, out error);

        switch(type)
        {
            case NetworkEventType.Nothing:
            break;

            case NetworkEventType.DataEvent:
            //Here we get data

            OnData(connectionId, channelId, recHostId, recBuffer);
            break;

            case NetworkEventType.ConnectEvent:
            Debug.Log(string.Format("User {0} connected through port {1}!", connectionId, recHostId));
            ConnectedUsersId.Add(connectionId);
            break;

            case NetworkEventType.DisconnectEvent:
            Debug.Log(string.Format("User {0} disconnected!", connectionId));
            ConnectedUsersId.Remove(connectionId);
            break;

            default:
            case NetworkEventType.BroadcastEvent:
            Debug.Log("Unexpected msg type!");
            break;
        }
    }

      #region Send
    public void SendClient(int recHostId, int connectionId, byte[] buffer) 
    {
        if(recHostId == 0)
            NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
        else
            NetworkTransport.Send(webHostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
    }
    #endregion

    #region OnData
    private void OnData(int conId, int channel, int host, byte[] buffer) 
    {

        //Here write what to do
        messageProcessing.OnData(conId, channel, host, buffer);
        SendOther(conId, host, buffer);
    }

    public void SendOther(int conId, int host, byte[] buffer)
    {
        
        foreach (var i in ConnectedUsersId)
        {
            if (i != conId)
                SendClient(host, i, buffer);
            Debug.Log(string.Format("Sending msg about this to user {0}", i));
        }
    }
    #endregion
}
