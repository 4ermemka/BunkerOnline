using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitConfirm : MonoBehaviour
{
    void Start()
    {
        transform.position = Vector3.zero;
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void Appear()
    {
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.5f);
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Disapear()
    {
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, 0.5f);
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnClickYes()
    {
        if(MessageProcessing.gameManager != null) MessageProcessing.gameManager.Disconnect(null, EventArgs.Empty);
        else Debug.Log("GM EMPTY!");
    }

    public void OnClickNo()
    {
        Disapear();
    }
}
