using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenedCardsPanel : MonoBehaviour, IDropHandler
{
    [SerializeField] private Attribute atrPref;
    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        if(card)
        {
              Attribute newAtr = Instantiate(atrPref) as Attribute;
              newAtr.SetIcon(card.GetIcon());
              newAtr.SetColor(card.GetColor());
              newAtr.SetDescription(card.GetDescription());
              newAtr.SetAttributeName(card.GetAttributeName());
              newAtr.SetCategory(card.GetCategory());
              newAtr.transform.SetParent(transform);
              newAtr.transform.localScale = new Vector3(1,1,1);
              Destroy(eventData.pointerDrag.gameObject);
              Destroy(card);
        }
    }
}