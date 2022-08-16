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

[System.Serializable]
public struct Padding
{
    [SerializeField] public float left;
    [SerializeField] public float right;
    [SerializeField] public float top;
    [SerializeField] public float bottom;
}

[ExecuteAlways]
public class FlexibleLayoutGroup : MonoBehaviour
{
    [SerializeField] private LayoutType layoutType;

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;

    [SerializeField] private bool squareElems;
    [SerializeField] private bool centreLastRow;
    [SerializeField] private bool enableAnimation;
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private int childrenLastCount;
    private Vector2 lastRectSize;

    [SerializeField] private Padding padding;

    public void Start()
    {
        childrenLastCount = 0;
        lastRectSize = gameObject.GetComponent<RectTransform>().rect.size;
    }

    public void SetChildren()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

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

        for(int i = 0; i < transform.childCount; i++) 
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = transform.GetChild(i);

            var xPos = (cellSize.x + spacing.x) * columnCount + padding.left;
            var yPos = (cellSize.y + spacing.y) * rowCount + padding.top;

            float offsetX = parentWidth - cellSize.x;
            float offsetY = parentHeight - cellSize.y;

            float centreOffset = 0;
            if(centreLastRow && rowCount == rows-1 && transform.childCount%columns!=0) 
                centreOffset = (cellSize.x + spacing.x) * (columns-transform.childCount%columns);
            offsetX -= centreOffset;

            if(squareElems)
            {   
                if(enableAnimation && Application.isPlaying)
                    LeanTween.moveLocal(item.gameObject, new Vector3(xPos - offsetX/2, -(yPos - offsetY/2), 0f), 0.5f).setEase(curve);
                else
                    item.transform.localPosition = new Vector3(xPos - offsetX/2,-(yPos - offsetY/2),0f);
                
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Min(cellSize.x,cellSize.y), Mathf.Min(cellSize.x,cellSize.y));
            }
            else
            {
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x, cellSize.y);
        //#if UNITY_EDITOR
                if(!enableAnimation)
                item.transform.localPosition = new Vector3(xPos - offsetX/2, -(yPos - offsetY/2),0f);
        //#else
                if(enableAnimation) 
                LeanTween.moveLocal(item.gameObject, new Vector3(xPos - offsetX/2, -(yPos - offsetY/2),0f), 0.5f).setEase(curve);
        //#endif
            }
        }
    }

    public void Update()
    {
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

        if(lastRectSize!=gameObject.GetComponent<RectTransform>().rect.size || childrenLastCount!=transform.childCount)
        {
            SetChildren();
            childrenLastCount = transform.childCount;
            lastRectSize = gameObject.GetComponent<RectTransform>().rect.size;
        }
    }
}
