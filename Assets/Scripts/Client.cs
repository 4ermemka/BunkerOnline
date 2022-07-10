using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public event EventHandler<OnConnectEventArgs> OnConnect;
    public event EventHandler OnDisconnect;
    public event EventHandler<OnDataEventArgs> OnData;
    public class OnConnectEventArgs:EventArgs
    {
        public int conId;
    }
    public class OnDataEventArgs:EventArgs
    {
        public  byte[] buffer;
    }

    private const int BYTE_SIZE = 1024;

    private const int MAX_USER = 100;
    private const int PORT = 28120;
    private const int WEB_PORT = 28121;

    private byte reliableChannel;
    private int connectionId; 
    private int hostId;
    private byte error;

    private bool isStarted;

    private void Start()// При старте выполнить код ниже
    {
        DontDestroyOnLoad(gameObject); // гарантия перехода между сценами
        Init();
    }

    private void Update() // каждый кадр
    {
        UpdateMessagePump();
    }

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        //CLIENT ONLY FROM HERE

        hostId = NetworkTransport.AddHost(topo, 0);
    }

    public void Connect(string SERVER_IP)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        //web client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Web connection");
        #else
        //standalone client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
        Debug.Log("Standalone connection");
        #endif

        Debug.Log(string.Format("Connecting to {0}...", SERVER_IP));

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

            OnData?.Invoke(this, new OnDataEventArgs {buffer = recBuffer});
            break;

            case NetworkEventType.ConnectEvent:
            OnConnect?.Invoke(this, new OnConnectEventArgs {conId = connectionId});
            Debug.Log("Connected to the server!");
            break;

            case NetworkEventType.DisconnectEvent:
            OnDisconnect?.Invoke(this, EventArgs.Empty);
            Debug.Log("Disconnected from the server!");
            break;

            default:
            case NetworkEventType.BroadcastEvent:
            Debug.Log("Unexpected msg type!");
            break;
        }
    }

    #region Send
    public void SendServer(byte[] buffer) 
    {
        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
        Debug.Log("Sending msg...");
    }
    #endregion
}
