using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum CanvasType
{
    MainMenu,
    ConnectionMenu,
    LobbyMenu
}

public class InterfaceMG : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameManager gm;
    [SerializeField] private Text playerNickDisplay;
    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private InputField NicknameField;
    [SerializeField] private Text errMsg;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    public void Start()
    {
        player = new Player(0,"PLAYER");
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        resetErrorMsg();
        SwitchCanvas(CanvasType.MainMenu);
    }


    public void RefreshPlayerDisplay()
    {
        playerNickDisplay.text = player.name;
        UpdatePlayerFromList(player.Id,player.name);
    }

    public void SetInfo()
    {
        if(NicknameField.text != "") player.name = NicknameField.text;
        else
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        RefreshPlayerDisplay();
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
        if(player.name == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else 
        {
            resetErrorMsg();
            SwitchCanvas(CanvasType.LobbyMenu);
        }
    }

    public void SwitchToConnectionMenu()
    {
        if(player.name == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else
        {
            resetErrorMsg();
            SwitchCanvas(CanvasType.ConnectionMenu);
        }
    }
    
    public void resetErrorMsg()
    {
        if(errMsg != null && player.name != "") errMsg.text = "";
    }

    public void AddPlayerToList(string Nickname, int num, bool host)
    {
        GameObject connectedUser = Instantiate(playerInfo) as GameObject;
        PlayerInfo temp = connectedUser.GetComponent<PlayerInfo>();

        temp.setNickname(Nickname);
        temp.toggleHost(host);
        temp.setNum(num);
        temp.setPanelToList(lobbyList);
    }

    public void RemovePlayerFromList(int id)
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }

        GameObject deletingUser = info.Find(x => x.GetComponent<PlayerInfo>().num == id);
        
        if(deletingUser!=null) Destroy(deletingUser);
        else Debug.Log("Err during deletion!");
    }
    public void UpdatePlayerFromList(int id, string newNickname)
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }

        GameObject updatingUser = info.Find(x => x.GetComponent<PlayerInfo>().num == id);
        
        if(updatingUser!=null) 
        {
            updatingUser.GetComponent<PlayerInfo>().nickname = newNickname;
            updatingUser.GetComponent<PlayerInfo>().setNickname(newNickname);
        }
        else Debug.Log("Err during Updating!");
    }

    public void OnHost()
    {
        if(player.name!="") AddPlayerToList(player.name, player.Id, true);
        SwitchToLobby();
    }

    public void OnLobbyClosed() 
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }
        
        foreach(GameObject deletingPlayer in info) 
        {
            Debug.Log(deletingPlayer.GetComponent<PlayerInfo>().num);
            Destroy(deletingPlayer);
        }
    }
}
