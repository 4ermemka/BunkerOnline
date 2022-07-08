using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private GameManagerClass GM = new GameManagerClass();

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
    public void SendClient(int recHostId, int connectionId, NetMsg msg) 
    {
        //Place to hold data
        byte[] buffer = new byte[BYTE_SIZE];
        
        //Here you make byte array from your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);

        formatter.Serialize(ms, msg);

        if(recHostId == 0)
            NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
        else
            NetworkTransport.Send(webHostId, connectionId, reliableChannel, buffer, buffer.Length, out error);
    }
    #endregion

    #region OnData
    private void OnData(int conId, int channel, int host, NetMsg msg) 
    {
        Debug.Log(string.Format("Received msg from {0}, through channel {1}, host {2}. Msg type: {3}", conId, channel, host, msg.OP));

        //Here write what to do
       switch (msg.OP) {
        case NetOP.None:            
            break;

        case NetOP.AddPlayer:
            GM.AddNewPlayer(msg.Username, conId);
            Debug.Log(string.Format("Adding new player. Username: {0}, id: {1}", msg.Username, conId));
            SendOther(conId, host, msg);
            break;
            
        case NetOP.LeavePlayer:
            GM.PausePlayer(msg.Username, conId);
            Debug.Log(string.Format("Player {0}, id: {1} is now inactive.", msg.Username, conId));
            SendOther(conId, host, msg);
            break;

        case NetOP.UpdateCardPlayer:   
            GM.UpdateInformation(msg.Username, conId, msg.NewCardsOnTable);
            Debug.Log(string.Format("Player {0}, id: {1} opened new card.", msg.Username, conId));
            SendOther(conId, host, msg);    
            break;

        case NetOP.CastCardPlayer:
            //Soon          
            break;

        default :
            Debug.Log("Unexpected msg type!");
            break;
       }
    }

    private void SendOther(int conId, int host, NetMsg msg)
    {
        for(int i = 0; i < ConnectedUsersId.Length; i++) 
        {
            if (ConnectedUsersId[i] != conId) SendClient(host, ConnectedUsersId[i], msg);
            Debug.Log(string.Format("Sending msg about this to user {0}", ConnectedUsersId[i]));
        }    
    }
    #endregion
}
