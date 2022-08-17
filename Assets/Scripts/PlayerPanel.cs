using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    
    private float offset = Mathf.PI/2;
    private List<Attribute> attributesList;

    private void Start() 
    {
        SetChildrenOnCircle();
        //gridRenderer = GetComponent<LineRenderer>();
    }

     public void Update()
    {
        SetChildrenOnCircle();
    }


    public void SetChildrenOnCircle() 
    {
        List<GameObject> children = new List<GameObject>();

        float maxContainerSize = Mathf.Min(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
        float maxWidth = 0f;

        foreach (Transform p in transform)
            {
                children.Add(p.gameObject);
                if (p.gameObject.GetComponent<RectTransform>().rect.width > maxWidth)
                    maxWidth = p.gameObject.GetComponent<RectTransform>().rect.width;
                if (p.gameObject.GetComponent<RectTransform>().rect.height > maxWidth)
                    maxWidth = p.gameObject.GetComponent<RectTransform>().rect.height;
            }

        if(children.Count == 0) return;

        Circle ch = new Circle(children.Count, maxContainerSize/2 - maxWidth/2);

        ch.SetOffset(offset);

        List<Vector2> coords = ch.GetCoords();
        for(int i = 0; i < ch.GetVertexCount(); i++) 
        {
            children[i].transform.parent = transform;
            children[i].transform.localScale = new Vector3(1,1,1); 
            children[i].transform.localPosition = coords[i];
        }
    }
}
