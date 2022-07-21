using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    [SerializeField] private int vertexCount = 5;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float offset = 0f;

    public Circle()
    {
        vertexCount = 5;
        radius = 5f;
    }
    public Circle(int v, float r)
    {
        vertexCount = v;
        radius = r;
    }

    public void SetRadius (float r) 
    {
        radius = r;
    }

    public int GetVertexCount() 
    {
        return vertexCount;
    }

    public float GetRadius()
    {
        return radius;
    }

    public float GetCircleLength()
    {
        return 2f * Mathf.PI * radius;
    }

    public void SetVertexCount(int c) 
    {
        vertexCount = c;    
    }

    public void SetOffset(float o)
    {
        offset = o;
    }

    public List<Vector3> GetCoords()
    {
        if(vertexCount == 0) return null;

        List<Vector3> list = new List<Vector3>();

        float deltaTheta = 2f * Mathf.PI / (float)(vertexCount);
        float theta = offset;

        for(int i = 0; i < vertexCount; i++) 
        {
            Vector3 vertex = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 1f);
            Vector3 point;
            point = vertex;

            list.Add(point);

            theta += deltaTheta;
        }

        return list;
    }
}

public class PlayerPanel : MonoBehaviour
{
    private float offset = Mathf.PI/2;
    [SerializeField] private LineRenderer gridRenderer;

    public void Start()
    {
        SetChildrenOnCircle();
        //gridRenderer = GetComponent<LineRenderer>();
        gridRenderer.gameObject.transform.localScale = new Vector3(1,1,1);
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

        float newScale = 1 + 1/(float)ch.GetVertexCount();
        ch.SetOffset(offset);

        List<Vector3> coords = new List<Vector3>();
        coords = ch.GetCoords();
        gridRenderer.positionCount = ch.GetVertexCount();
        for(int i = 0; i < ch.GetVertexCount(); i++) 
        {
            children[i].transform.parent = transform;
            children[i].transform.localScale = new Vector3(newScale,newScale,1); 
            children[i].transform.localPosition = coords[i];
            gridRenderer.SetPosition(i, coords[i]);
        }
    }
}
