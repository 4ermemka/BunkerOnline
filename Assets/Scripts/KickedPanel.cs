using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickedPanel : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    private CanvasGroup cg;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }

    public void Appear()
    {
        LeanTween.alphaCanvas(cg, 1, 0.5f);
        cg.blocksRaycasts = true;
    }

    public void Disapear()
    {
        LeanTween.alphaCanvas(cg, 0, 0.5f);
        cg.blocksRaycasts = false;
        CardInfo info = FindObjectOfType<CardInfo>();
        if(info != null) Destroy(info.gameObject);
    }
}
