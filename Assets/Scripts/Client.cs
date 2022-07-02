using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private GameManagerClass GM = new GameManagerClass();

    private const int BYTE_SIZE = 1024;

    private const int MAX_USER = 100;
    private const int PORT = 28120;
    private const int WEB_PORT = 28121;

    private const string SERVER_IP = "127.0.0.1";

    private byte reliableChannel;
    private int connectionId; 
    private int hostId;
    private byte error;

    private bool isStarted;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Update()
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

    public void UpdateMessagePump() 
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
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(recBuffer);

            NetMsg msg = (NetMsg)formatter.Deserialize(ms);
            OnData(connectionId, channelId, recHostId, msg);
            break;

            case NetworkEventType.ConnectEvent:
            Debug.Log("Connected to the server!");
            break;

            case NetworkEventType.DisconnectEvent:
            Debug.Log("Disconnected from the server!");
            break;

            default:
            case NetworkEventType.BroadcastEvent:
            Debug.Log("Unexpected msg type!");
            break;
        }
    }

    #region OnData
    private void OnData(int conId, int channel, int host, NetMsg msg) 
    {
        Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", conId, channel, host, msg.OP));

        //Here write what to do
       switch (msg.OP) {
        case NetOP.None:            
            break;

        case NetOP.AddPlayer:
            Debug.Log(string.Format("Player connected!. Username: {0}", msg.Username));
            //make interface changes
            break;
            
        case NetOP.LeavePlayer:
            Debug.Log(string.Format("Player {0} is now paused.", msg.Username));
            //make interface changes
            break;

        case NetOP.UpdateCardPlayer:
            Debug.Log(string.Format("Player {0} opened new card.", msg.Username));
            //make interface changes
            break;

        case NetOP.CastCardPlayer:
            //Soon          
            break;

        default :
            Debug.Log("Unexpected msg type!");
            break;
       }
    }
    #endregion

    #region Send
    public void SendServer(NetMsg msg) 
    {
        //Place to hold data
        byte[] buffer = new byte[BYTE_SIZE];
        
        //Here you make byte array from your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
    }
    #endregion
}
