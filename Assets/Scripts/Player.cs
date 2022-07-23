using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class Player : User
{
    [SerializeField] private Image avatar;
    [SerializeField] private PlayerPanel attributePanel;
    [SerializeField] private Text nicknameText;

    public bool isActive;
    public List<string> cards;

    public Player() : base() 
    {
        isActive = true;
        cards = new List<string>();
    }

    public Player(int id, string name) : base(id, name)
    {
        this.isActive = true;
        //this.cards = new string;
    }

    public Player(int id, string name, bool host, string[] cards) : base(id, name, host)
    {
        this.isActive = true;
        this.cards.Clear();

        foreach(var c in cards) 
        {
        this.cards.Add(c);    
        }
    }

    //public static implicit operator Player(User user) => new Player (user.id, user.name);

    public void SetCards(string[] cards)
    {
        this.cards.Clear();

        foreach(var c in cards) 
        {
        this.cards.Add(c);    
        }
    }
    public void SetActiveStatus(bool isActive)
    {
        this.isActive = isActive;
    }

    public bool IsActive()
    {
        return this.isActive;
    }

    public void AddAtribute() 
    {
        
    }
    public void UpdateAtribute() 
    {
        
    }

    public void DeleteAtribute() 
    {
        
    }
}
