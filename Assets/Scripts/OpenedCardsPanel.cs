using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenedCardsPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        if(card)
            card.defaultParent = transform;
    }
}
