using UnityEngine;
using System.Collections.Generic;

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

    public List<Vector2> GetCoords()
    {
        if(vertexCount == 0) return null;

        List<Vector2> list = new List<Vector2>();

        float deltaTheta = 2f * Mathf.PI / (float)(vertexCount);
        float theta = offset;

        for(int i = 0; i < vertexCount; i++) 
        {
            Vector2 vertex = new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
            Vector2 point;
            point = vertex;

            list.Add(point);

            theta += deltaTheta;
        }

        return list;
    }
}