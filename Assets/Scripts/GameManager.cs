using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum Events 
{
    Turn,
    Voting,
    Kick
}

public class Player
{
    public bool IsActive;
    public int Id;
    public string name;
    public string[] cards;

    public Player()
    {
        this.IsActive = true;
        this.Id = 0;
        this.name = "PLAYER";
        this.cards = null;
    }

    public Player(int id, string name)
    {
        this.IsActive = true;
        this.Id = id;
        this.name = name;
        this.cards = null;
    }
    public Player(int id, string name, string[] cards)
    {
        this.IsActive = true;
        this.Id = id;
        this.name = name;
        this.cards = cards;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public void SetCards(string[] cards)
    {
        this.cards = cards;
    }
    public void SetStatus(bool IsActive)
    {
        this.IsActive = IsActive;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Server serverPref;
    [SerializeField] private Client clientPref;

    private Server server;
    private Client client;

    private Player player; 
    private List<Player> connectedList;

    [SerializeField] private InterfaceMG interfaceMG;

    public void Start()
    {
        player = new Player();
        connectedList = new List<Player>();
        interfaceMG.OnClickConnect += GameManager_ConnectPressed;
        interfaceMG.OnChooseClient += GameManager_StartClient;
        interfaceMG.OnChooseServer += GameManager_StartServer;
        interfaceMG.OnReturnToMenu += GameManager_DeleteNetworkObjects;
    }

    public void AddNewPlayer(string name, int conID)
    {
        int countconnectedList = connectedList.Count;
        countconnectedList++;
        Player newPlayer = new Player(countconnectedList, name);
        connectedList.Add(newPlayer);
        interfaceMG.AddPlayerToList(name, conID, false);
    }

    public void AddNewPlayer(string name, int conID, string cardsOld)
    {
        string[] cards;
        cards = Decryption(cardsOld);
        Player newPlayer = new Player(conID, name, cards);
        interfaceMG.AddPlayerToList(name, conID, false);
        connectedList.Add(newPlayer);
    }

    public void PausePlayer(string name, int conID)
    {
        //pause code
    }

    private bool IsEmpty(int id)
    {
        bool flag = false; int i;
        for (i = 0; i < connectedList[id].cards.Length; i++)
            if (connectedList[id].cards[i] == string.Empty) flag = true;
        if (!flag && connectedList[id].name == string.Empty)
            return true;
        else return false;
    }

    public bool UpdateInformation(string name, int conId, string cardsNew)
    {
        string[] cards;
        cards = Decryption(cardsNew);
        if (conId < connectedList.Count && !IsEmpty(conId))
        {
            connectedList[conId].SetName(name);
            connectedList[conId].SetCards(cards);
            return true;
        }
        else return false;
    }

    public string Encryption(int id)
    {
        string en_cards = ""; int i;
        for (i = 0; i < connectedList[id].cards.Length; i++)
            en_cards = en_cards + connectedList[id].cards[i] + ";";
        return en_cards;
    }

    public string[] Decryption(string en_cards)
    {
        int i, count_separator = 0, k = 0;
        for (i = 0; i < en_cards.Length; i++)
            if (en_cards[i] == ';') count_separator++;
        string[] dc_cards = new string[count_separator];
        for (i = 0; i < count_separator; i++)
        {
            while (en_cards[k] != ';')
            {
                dc_cards[i] += en_cards[k];
                k++;
            }
            k++;
        }
        return dc_cards;
    }

    //////////////////////////////////////////////////////////////////
    /////////////////// Reversed Ladder of events ////////////////////
    //////////////////////////////////////////////////////////////////

    public void GameManager_ConnectPressed(object sender, InterfaceMG.OnClickConnectEventArgs e)
    {
        if(client!=null)
        {
            client.Connect(e.server_ip);
        }
        else Debug.Log("Err! No Client O_o");
    }

    public void GameManager_StartServer(object sender, EventArgs e)
    {
        server = Instantiate(serverPref);
    }

    public void GameManager_StartClient(object sender, EventArgs e)
    {
        client = Instantiate(clientPref);
    }

    public void GameManager_DeleteNetworkObjects(object sender, EventArgs e)
    {
        if(server!=null) 
        {
            server.Shutdown();
            Destroy(server.gameObject);
        }
        if(client!=null) 
        {
            client.Shutdown();
            Destroy(client.gameObject);
        }
    }

}