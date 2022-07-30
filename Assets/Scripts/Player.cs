using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

[Serializable]
public class Player : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private PlayerPanel attributePanel;
    [SerializeField] private Text nicknameText;

    public bool isHost;
    public int id;
    public string Nickname;
    public bool isActive;
    public List<string> cards;

    public Player() 
    {
        isActive = true;
        cards = new List<string>();
    }

    public Player(int id, string name)
    {
        this.id = id;
        this.Nickname = name;
        this.isActive = true;
        cards = new List<string>();
    }

    public Player(User user, string[] cards)
    {
        this.id = user.id;
        this.Nickname = user.Nickname;
        this.isHost = user.isHost;
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
