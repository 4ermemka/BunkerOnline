using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitConfirm : MonoBehaviour
{
    private GameManager gm;
    void Start()
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        gm = MessageProcessing.gameManager;
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
        if(gm != null)gm.Disconnect();
        else Debug.Log("GM EMPTY!");
    }

    public void OnClickNo()
    {
        Disapear();
    }
}
