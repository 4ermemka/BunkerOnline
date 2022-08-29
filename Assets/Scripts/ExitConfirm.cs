using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitConfirm : MonoBehaviour
{
    private GameManager gm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnClickYes()
    {
    }

    private void OnClickNo()
    {
        Destroy(gameObject);
    }
}
