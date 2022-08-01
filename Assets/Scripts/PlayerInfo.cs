using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

[Serializable]
public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private PlayerPanel attributePanel;
    [SerializeField] private Text nicknameText;

    public string Nickname;
    public List<string> cards;
    
    //public static implicit operator Player(User user) => new Player (user.id, user.name);

    public void SetCards(string[] cards)
    {
        this.cards.Clear();

        foreach(var c in cards) 
        {
            this.cards.Add(c);    
        }
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
