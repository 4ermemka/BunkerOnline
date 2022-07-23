using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
<<<<<<< HEAD
        //this.cards = new string;
=======
        cards = new List<string>();
>>>>>>> a6b27f0f9cab7fc27aacd6f7a430aa7f31993b28
    }

    public Player(int id, string name, bool host, string[] cards) : base(id, name, host)
    {
        this.isActive = true;
        SetCards(cards);
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

    public Attribute FindAttribute(int id)
    {
        List<Attribute> attributesList = attributePanel.GetComponentsInChildren<Attribute>().ToList();

        Attribute attribute = attributesList.Find(x => x.GetComponent<Attribute>().GetId() == id);
        return attribute;
    }

    public void UpdateAttribute(int id) 
    {
        
    }

    public void DeleteAttribute(int id) 
    {
        Attribute deletedAttribute = FindAttribute(id);
        if (deletedAttribute!=null) Destroy(deletedAttribute.gameObject);
    }
}
