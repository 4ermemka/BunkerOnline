using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum Events {
    Turn,
    Voting,
    Kick
}

struct Player
{
    public bool IsActive;
    public int Id;
    public string name;
    public string[] cards;

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
    private List<Player> players;

    [SerializeField] private InterfaceMG interfaceMG;

    public GameManager()
    {
        players = new List<Player>();
    }

    public void AddNewPlayer(string name, int conID)
    {
        int countPlayers = players.Count;
        countPlayers++;
        Player newPlayer = new Player(countPlayers, name);
        players.Add(newPlayer);
        interfaceMG.AddPlayerToList(name, conID, false);
    }

    public void AddNewPlayer(string name, int conID, string cardsOld)
    {
        string[] cards;
        cards = Decryption(cardsOld);
        Player newPlayer = new Player(conID, name, cards);
        interfaceMG.AddPlayerToList(name, conID, false);
        players.Add(newPlayer);
    }

    public void PausePlayer(string name, int conID)
    {
        //pause code
    }

    private bool IsEmpty(int id)
    {
        bool flag = false; int i;
        for (i = 0; i < players[id].cards.Length; i++)
            if (players[id].cards[i] == string.Empty) flag = true;
        if (!flag && players[id].name == string.Empty)
            return true;
        else return false;
    }

    public bool UpdateInformation(string name, int conId, string cardsNew)
    {
        string[] cards;
        cards = Decryption(cardsNew);
        if (conId < players.Count && !IsEmpty(conId))
        {
            players[conId].SetName(name);
            players[conId].SetCards(cards);
            return true;
        }
        else return false;
    }

    public string Encryption(int id)
    {
        string en_cards = ""; int i;
        for (i = 0; i < players[id].cards.Length; i++)
            en_cards = en_cards + players[id].cards[i] + ";";
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
}