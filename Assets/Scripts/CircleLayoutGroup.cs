using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleLayoutGroup : LayoutGroup
{
    [SerializeField] private float offset = -Mathf.PI/2;
    [SerializeField] private float scaleFactor = 2f;
    public Vector2 cellSize;

    public override void CalculateLayoutInputVertical()
    {
        if(scaleFactor==0) scaleFactor = 1;
        float maxContainerSize = Mathf.Min(GetComponent<RectTransform>().rect.width, 
        GetComponent<RectTransform>().rect.height);

        if(rectChildren.Count != 0)
        {
            cellSize.x = maxContainerSize/(float)scaleFactor;
            cellSize.y = maxContainerSize/(float)scaleFactor;

            Circle ch = new Circle(rectChildren.Count, 
            maxContainerSize/2 - cellSize.x/2);
            ch.SetOffset(offset);
            List<Vector2> coords = ch.GetCoords();

            for(int i = 0; i < rectChildren.Count; i++) 
            {
                var item = rectChildren[i];

                var xPos = coords[i].x + GetComponent<RectTransform>().rect.width/2f-cellSize.x/2;
                var yPos = coords[i].y + GetComponent<RectTransform>().rect.height/2f-cellSize.y/2;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }
    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
