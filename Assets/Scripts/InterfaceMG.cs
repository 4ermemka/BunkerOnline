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
    [SerializeField] private GameObject lobbyList;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    public void Start()
    {
        //for (var i = 0; i < 10; i++)
        //{
        //    Instantiate(playerInfo, new Vector3(0, i * 2.0f, 0), Quaternion.identity);
        //}

        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.MainMenu);
    }

    public void SwitchCanvas(CanvasType _type)
    {
        if(lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }
        CanvasController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if(desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else Debug.Log("Err. Desired Canvas does not exist!");

    }

    public void SwitchToMainMenu()
    {
        SwitchCanvas(CanvasType.MainMenu);
    }

    public void SwitchToLobby()
    {
        SwitchCanvas(CanvasType.LobbyMenu);
    }

    public void SwitchToConnectionMenu()
    {
        SwitchCanvas(CanvasType.ConnectionMenu);
    }

    public void AddPlayerToList(string Nickname, int num)
    {
        GameObject connectedUser = Instantiate(playerInfo) as GameObject;
        List<Text> info = connectedUser.GetComponentsInChildren<Text>().ToList();
        info[0].text = Nickname;
        info[1].text = num.ToString();

        connectedUser.transform.parent = lobbyList.transform;
    }
}
