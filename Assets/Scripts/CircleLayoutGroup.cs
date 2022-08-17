using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class CircleLayoutGroup : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private bool enableAnimation;
    [SerializeField] private float animationTime;
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private float maxContainerSize = 5f;
    [SerializeField] private int childrenLastCount;
    private Vector2 lastRectSize;
    public Vector2 cellSize;

    public void Start()
    {
        childrenLastCount = 0;
        offset = Mathf.PI/2;
        lastRectSize = gameObject.GetComponent<RectTransform>().rect.size;
    }

    public void SetChildrenInCircle()
    {
        if(gameObject.transform.childCount != 0)
        {
            cellSize.x = maxContainerSize/(float)scaleFactor;
            cellSize.y = maxContainerSize/(float)scaleFactor;

            Circle ch = new Circle(gameObject.transform.childCount, 
            maxContainerSize/2 - cellSize.x/2);
            ch.SetOffset(offset);
            List<Vector2> coords = ch.GetCoords();

            for(int i = 0; i < gameObject.transform.childCount; i++) 
            {
                var item = gameObject.transform.GetChild(i);
                var xPos = coords[i].x;
                var yPos = coords[i].y;

                if(enableAnimation && Application.isPlaying) LeanTween.moveLocal(item.gameObject, new Vector3(xPos, yPos, animationTime), 0.5f).setEase(curve);
                else item.transform.localPosition = new Vector3(xPos,yPos,0f);

                item.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x, cellSize.y);
            }
        }
    }

    public void Update()
    {
        if(animationTime <= 0) animationTime*=-1 + 1;
        
        if(scaleFactor==0) scaleFactor = 1;
        maxContainerSize = Mathf.Min(GetComponent<RectTransform>().rect.width, 
        GetComponent<RectTransform>().rect.height);

        if(lastRectSize!=gameObject.GetComponent<RectTransform>().rect.size || childrenLastCount!=transform.childCount)
        {
            offset += transform.childCount*Mathf.PI/3;
            SetChildrenInCircle();
            childrenLastCount = transform.childCount;
            lastRectSize = gameObject.GetComponent<RectTransform>().rect.size;
        }
    }
}
