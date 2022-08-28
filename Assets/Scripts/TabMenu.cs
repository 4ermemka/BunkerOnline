using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TabMenu : MonoBehaviour
{
    [SerializeField] TabGroup panel;
    [SerializeField] float animationSpeed;
    [SerializeField] AnimationCurve curve;

    private RectTransform panelRect;
    private RectTransform myRect;
    private Vector3 offset;
    void Start()
    {
        panelRect = panel.GetComponent<RectTransform>();
        myRect = gameObject.GetComponent<RectTransform>();
        offset = new Vector3(myRect.rect.width/2 + panelRect.rect.width/2, 0, 0);
        SetPosition(panel.transform.localPosition + offset);
    }
    void Update()
    {
        offset = new Vector3(myRect.rect.width/2 + panelRect.rect.width/2, 0, 0);
    }

    public void Appear()
    {
        SetPosition(panel.transform.localPosition + offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, animationSpeed);
        ChangePosition(panel.transform.localPosition - offset);
    }

    public void Disappear() 
    {
        SetPosition(panel.transform.localPosition - offset);
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, animationSpeed);
        ChangePosition(panel.transform.localPosition + offset);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void ChangePosition(Vector3 pos)
    {
        LeanTween.moveLocal(gameObject, pos, animationSpeed).setEase(curve);
    }


}
