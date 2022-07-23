using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : Attribute, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera cam;

    void Awake()
    {
        cam = Camera.allCameras[0];
    }

    void Update()
    {
        
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 newPos = cam.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
