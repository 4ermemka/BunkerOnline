using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Card : Attribute, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform defaultParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = cam.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
