using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleLayoutGroup : LayoutGroup
{
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool squareElems;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        int count = transform.childCount;

        int b = 1;
        int a = count;

        if (count > 0) 
        {
            while (b < a || a * b < count)// ...все влезло и при этом +-квадратная матрица
            {
                b++;
                a = count / b;
                if (count % b != 0) a++;
            }
        }

        rows = a;
        columns = b;

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)columns) - 
        ((spacing.x / (float)columns) * (columns - 1)) - 
        (padding.left/(float)columns) - (padding.right/(float)columns);

        float cellHeight = (parentHeight/(float)rows) - 
        ((spacing.y / (float)rows) * (rows - 1)) -
        (padding.top/(float)rows) - (padding.bottom/(float)rows);

        cellSize.x = cellWidth;
        cellSize.y = cellHeight;

        int columnCount = 0;
        int rowCount = 0;

        for(int i = 0; i < rectChildren.Count; i++) 
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x + spacing.x) * columnCount + padding.left;
            var yPos = (cellSize.y + spacing.y) * rowCount + padding.top;

            float offsetX = Mathf.Min(cellSize.x,cellSize.y) - cellSize.x;
            float offsetY = Mathf.Min(cellSize.x,cellSize.y) - cellSize.y;

            SetChildAlongAxis(item, 0, xPos - offsetX/2, Mathf.Min(cellSize.x,cellSize.y));
            SetChildAlongAxis(item, 1, yPos - offsetY/2, Mathf.Min(cellSize.x,cellSize.y));
        }
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
