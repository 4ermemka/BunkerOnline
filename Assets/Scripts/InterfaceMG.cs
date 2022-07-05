using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameManager;

public enum CanvasType
{
    MainMenu,
    ConnectionMenu,
    LobbyMenu
}

public class InterfaceMG : MonoBehaviour
{
    [SerializeField] private GameObject playerInfo;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    public void Start()
    {
        //for (var i = 0; i < 10; i++)
        //{
        //    Instantiate(playerInfo, new Vector3(0, i * 2.0f, 0), Quaternion.identity);
        //}

        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        Debug.Log("A: " + canvasControllerList.Count);
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        Debug.Log("B: " + canvasControllerList.Count);
        SwitchCanvas(CanvasType.MainMenu);
    }

    public void SwitchCanvas(CanvasType _type)
    {
        Debug.Log("type: " + _type);
        if(lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }
        Debug.Log("C: " + canvasControllerList.Count);
        CanvasController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if(desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else Debug.Log("Err. Desired Canvas does not exist!");
        Debug.Log("D: " + canvasControllerList.Count);

    }

    public void SwitchToMainMenu()
    {
        Debug.Log("E: " + canvasControllerList.Count);
        SwitchCanvas(CanvasType.MainMenu);
    }

    public void SwitchToLobby()
    {
        Debug.Log("E: " + canvasControllerList.Count);
        SwitchCanvas(CanvasType.LobbyMenu);
    }

    public void SwitchToConnectionMenu()
    {
        Debug.Log("E: " + canvasControllerList.Count);
        SwitchCanvas(CanvasType.ConnectionMenu);
    }
}
