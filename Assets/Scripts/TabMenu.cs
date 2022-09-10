using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TabMenu : MonoBehaviour
{
    public bool isActive;
    [SerializeField] TabGroup panel;
    [SerializeField] float animationSpeed;
    [SerializeField] AnimationCurve curve;

    private Vector3 panelRectPos;
    private RectTransform panelRect;
    private RectTransform myRect;
    private Vector3 offset;
    void Start()
    {
        panelRect = panel.GetComponent<RectTransform>();
        myRect = transform.GetChild(0).GetComponent<RectTransform>();

        panelRectPos = panelRect.localPosition;
        offset = new Vector3(myRect.rect.size.x / 2 + panelRect.rect.size.x / 2, 0, 0);

        isActive = false;
        SetPosition(panelRectPos);
        SetOffset(offset);
    }
    void Update()
    {
        panelRectPos = panelRect.localPosition;
        SetPosition(panelRectPos);
        offset = new Vector3(myRect.rect.size.x / 2 + panelRect.rect.size.x / 2, 0, 0);
    }

    public void Appear()
    {
        isActive = true;
        //SetPosition(panelRectPos + offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, animationSpeed);
        ChangeOffset(-offset);
    }

    public void Disappear()
    {
        isActive = false;
        //SetPosition(panelRectPos - offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, animationSpeed);
        ChangeOffset(offset);
    }

    public void SetPosition(Vector3 pos)
    {
        gameObject.GetComponent<RectTransform>().localPosition = pos;
    }

    public void ChangeOffset(Vector3 pos)
    {
        LeanTween.moveLocal(myRect.gameObject, pos, animationSpeed).setEase(curve);
    }

    public void SetOffset(Vector3 pos)
    {
        myRect.transform.localPosition = pos;
    }


}
