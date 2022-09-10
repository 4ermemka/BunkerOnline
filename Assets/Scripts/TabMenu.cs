using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TabMenu : MonoBehaviour
{
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
        myRect = gameObject.GetComponent<RectTransform>();

        panelRectPos = panelRect.localPosition;
        offset = new Vector3(myRect.rect.size.x / 2 + panelRect.rect.size.x / 2, 0, 0);

        SetPosition(panelRectPos + offset);
    }
    void Update()
    {
        offset = new Vector3(myRect.rect.size.x / 2 + panelRect.rect.size.x / 2, 0, 0);
    }

    public void Appear()
    {
        SetPosition(panelRectPos + offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, animationSpeed);
        ChangePosition(panelRectPos - offset);
    }

    public void Disappear()
    {
        SetPosition(panelRectPos - offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, animationSpeed);
        ChangePosition(panelRectPos + offset);
    }

    public void SetPosition(Vector3 pos)
    {
        myRect.localPosition = pos;
    }

    public void ChangePosition(Vector3 pos)
    {
        LeanTween.moveLocal(gameObject, pos, animationSpeed).setEase(curve);
    }


}
