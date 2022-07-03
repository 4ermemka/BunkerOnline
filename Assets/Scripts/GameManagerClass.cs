using System;
using System.Collections.Generic;
using System.Text;

public enum Events {
    Turn,
    Voting,
    Kick
}

struct Player
{
<<<<<<< HEAD
    public bool isActive;
    public string connectionIP;
=======
    public bool IsActive;
>>>>>>> 7e7682c2ea9fa2adf4f7de140fb99ecf5e74e4f2
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

    public void SetName (string name)
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

namespace GameManagerClass
{
    class GameManagerClass
    {
        private List<Player> players;

        public GameManagerClass()
        {
            players = new List<Player>();
        }
<<<<<<< HEAD

        public void AddNewPlayer(string name)
=======
        public void AddNewPlayer(int id, string name)
>>>>>>> 7e7682c2ea9fa2adf4f7de140fb99ecf5e74e4f2
        {
            Player newPlayer = new Player(id, name, null);
            players.Add(newPlayer);
        }

        public void AddNewPlayer(int id, string name, string[] cards)
        {
            Player newPlayer = new Player(id, name, cards);
            players.Add(newPlayer);
        }

        public void DeletePlayer(int id)
        {
            bool flag;
            if (!players[id].IsActive) flag = false; //запуск таймера
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

        public bool UpdateInformation(int id, string name, string cards)
        {
            string[] dc_cards;
            dc_cards = Decryption(cards);

            if (id < players.Count && !IsEmpty(id))
            {
                players[id].SetName(name);
                players[id].SetCards(dc_cards);
                return true;
            }
            else return false;
        }

        public string SendToServer(int id)
        {
            string en_cards = Encryption(id);
            return en_cards;

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
}
