using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

[Serializable]
public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private CircleLayoutGroup attributePanel;
    [SerializeField] private TextMeshProUGUI nicknameText;

    private List<Attribute> attributesList;

    private string Nickname;
    
    //public static implicit operator Player(User user) => new Player (user.id, user.name);

    public void SetNickname (string Nickname)
    {
        this.Nickname = Nickname;
    }

    public void SetCards(string[] cards) {
    }

    public void Start() 
    {
        attributesList = attributePanel.GetComponentsInChildren<Attribute>().ToList();
    }

    public void Update()
    {
        nicknameText.text = Nickname;
    }

    public Attribute FindAttribute(int id)
    {
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
