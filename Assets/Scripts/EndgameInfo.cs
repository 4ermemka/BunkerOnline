using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndgameInfo : MonoBehaviour
{
    public int id;
    public string Nickname;

    [SerializeField] public TextMeshProUGUI userName;
    [SerializeField] public FlexibleLayoutGroup attributesPanel;

    [SerializeField] public Attribute attributePref;

    void Start()
    {
        
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void SetName(string name) 
    {
        userName.text = name;
        Nickname = name;
    }

    public void AddAttribute(DeckCardSerializable card)
    {
        Attribute newAtr = Instantiate(attributePref) as Attribute;
        newAtr.transform.SetParent(attributesPanel.transform);
        newAtr.transform.localScale = Vector3.one;

        newAtr.DeckCardSerializableToAttribute(card);
    }
}
