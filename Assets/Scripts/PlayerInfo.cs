using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

[Serializable]
public class PlayerInfo : MonoBehaviour
{
    private User user;
    [SerializeField] private Image avatar;
    [SerializeField] private CircleLayoutGroup attributePanel;
    [SerializeField] private TextMeshProUGUI nicknameText;

    private List<Attribute> attributesList;

    private string Nickname = "Nickname";

    public User GetUser()
    {
        return user;
    }

    public void SetUser(User us)
    {
        this.user = us;
    }

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

    public void AddAttribute(Attribute attribute)
    {
        attribute.gameObject.GetComponent<CanvasGroup>().alpha=0;
        attribute.transform.SetParent(attributePanel.transform);
        attribute.transform.localScale = new Vector3(1,1,1);
        attribute.transform.localPosition = new Vector3(0,0,20);
        LeanTween.alphaCanvas(attribute.gameObject.GetComponent<CanvasGroup>(),1, 0.5f);
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
