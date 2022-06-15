using System;
using System.Collections.Generic;
using System.Text;

public enum Events {
    Turn,
    Voting,
    Kick
}

struct Cards
{
    public bool sex;
    public int age;
    public string job;
    public string hobby;
    public string luggage;

    public bool IsEmpty()
    {
        if (job == string.Empty && luggage == string.Empty && hobby == string.Empty)
            return true;
        else return false;
    }
}

struct Player
{
    public int Id;
    public string name;
    public Cards cards;

    public Player(int id, string name, Cards cards)
    {
        this.Id = id;
        this.name = name;
        this.cards = cards;
    }

    public void SetName (string name)
    {
        this.name = name;
    }

    public void SetCards(Cards cards)
    {
        this.cards = cards;
    }
}

namespace GameManagerClass
{
    class GameManagerClass
    { 
        private List<Player> players;

        public GameManagerClass()
        {
            players = new List<Player>();
        }

        public void AddNewPlayer(string name, Cards cards)
        {
            int countPlayers = players.Count;
            countPlayers++;
            Player newPlayer = new Player(countPlayers, name, cards);
            players.Add(newPlayer);
        }

        private bool IsEmpty(int id)
        {
            if (players[id].cards.IsEmpty() && players[id].name == string.Empty)
                return true;
            else return false;
        }

        public bool UpdateInformation(int id, string name, Cards cards)
        {
            if (id < players.Count && !IsEmpty(id))
            {
                players[id].SetName(name);
                players[id].SetCards(cards);
                return true;
            }
            else return false;
        }
    }
}
