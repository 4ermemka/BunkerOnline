using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LayoutType
{
    Flexible,
    FlexibleSquare,
    ConstantRows,
    ConstantColumns
}

public class FlexibleLayoutGroup : LayoutGroup
{
    [SerializeField] private LayoutType layoutType;

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;

    [SerializeField] private bool squareElems;
    [SerializeField] private bool centreLastRow;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

       switch (layoutType) 
       {
        case LayoutType.FlexibleSquare:
            int count = transform.childCount;
        
            columns = 1;
            rows = count;

            if (count > 0) 
            {
                while (columns < rows || rows * columns < count)// ...все влезло и при этом +-квадратная матрица
                {
                    columns++;
                    rows = count / columns;
                    if (count % columns != 0) rows++;
                }
            }
            break;
        
        case LayoutType.Flexible:
            float scr = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(scr);
            columns = Mathf.CeilToInt(scr);
        break;

        case LayoutType.ConstantRows:
            columns = transform.childCount/rows;
            if(columns==0||transform.childCount%rows!=0) columns++;
        break;

        default :
            rows = transform.childCount/columns;
            if(rows==0||transform.childCount%columns!=0) rows++;
            break;
       }
        

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
            if(centreLastRow && rowCount == rows-1 && rectChildren.Count%columns!=0) offsetX -= (cellSize.x + spacing.x) * (columns-rectChildren.Count%columns);

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
