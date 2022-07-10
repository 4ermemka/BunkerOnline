using System;
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
    public event EventHandler<OnClickConnectEventArgs> OnClickConnect;
    public event EventHandler OnChooseClient;
    public event EventHandler OnChooseServer;
    public event EventHandler OnReturnToMenu;

    public class OnClickConnectEventArgs:EventArgs
    {
        public string server_ip;
    }

    private User user;
    [SerializeField] private GameManager gm;
    [SerializeField] private Text userNickDisplay;
    [SerializeField] private GameObject userInfo;
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private InputField NicknameField;
    [SerializeField] private InputField ipAdressField;
    [SerializeField] private Text errMsg;
    [SerializeField] public Text connectionStatusText;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    public void Start()
    {
        user = new User(0,"PLAYER");
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        resetErrorMsg();
        SwitchCanvas(CanvasType.MainMenu);
    }


    public void RefreshUserDisplay()
    {
        userNickDisplay.text = user.name;
        UpdateUserFromList(user.id,user.name);
    }

    public void SetInfo()
    {
        if(NicknameField.text != "") user.name = NicknameField.text;
        else
        {
            errMsg.text = "Nickname is empty!";
        }
        RefreshUserDisplay();
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
        OnReturnToMenu?.Invoke(this, EventArgs.Empty);
        OnLobbyClosed();
        SwitchCanvas(CanvasType.MainMenu);
    }

    public void SwitchToLobby()
    {
        if(user.name == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else 
        {
            resetErrorMsg();
            OnChooseServer?.Invoke(this, EventArgs.Empty);
            SwitchCanvas(CanvasType.LobbyMenu);
        }
    }

    public void SwitchToConnectionMenu()
    {
        if(user.name == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else
        {
            resetErrorMsg();
            OnChooseClient?.Invoke(this, EventArgs.Empty);
            SwitchCanvas(CanvasType.ConnectionMenu);
        }
    }
    
    public void resetErrorMsg()
    {
        if(errMsg != null && user.name != "") errMsg.text = "";
        connectionStatusText.text = "";
    }

    public void AddUserToList(string Nickname, int num, bool host)
    {
        GameObject connectedUser = Instantiate(userInfo) as GameObject;
        UserInfo temp = connectedUser.GetComponent<UserInfo>();

        temp.setNickname(Nickname);
        temp.toggleHost(host);
        temp.setNum(num);
        temp.setPanelToList(lobbyList);
    }

    public void RemoveUserFromList(int id)
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }

        GameObject deletingUser = info.Find(x => x.GetComponent<UserInfo>().num == id);
        
        if(deletingUser!=null) Destroy(deletingUser);
        else Debug.Log("Err during deletion!");
    }
    public void UpdateUserFromList(int id, string newNickname)
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }

        GameObject updatingUser = info.Find(x => x.GetComponent<UserInfo>().num == id);
        
        if(updatingUser!=null) 
        {
            updatingUser.GetComponent<UserInfo>().nickname = newNickname;
            updatingUser.GetComponent<UserInfo>().setNickname(newNickname);
        }
        else Debug.Log("Err during Updating!");
    }

    public void ClickConnect() 
    {
        if(ipAdressField.text != "")
        {
            string ip = ipAdressField.text;
            connectionStatusText.text = "Connecting to " + ip.ToString() + "...";
            OnClickConnect?.Invoke(this, new OnClickConnectEventArgs {server_ip = ip});
        }
        else connectionStatusText.text = "Ip field is empty!";
    }

    public void OnHost()
    {
        if(user.name!="") AddUserToList(user.name, user.id, true);
        SwitchToLobby();
    }

    public void OnLobbyClosed() 
    {
        List<GameObject> info = new List<GameObject>();
        foreach (Transform child in lobbyList.transform) 
        {
            info.Add(child.gameObject);
        }
        
        foreach(GameObject deletingUser in info) 
        {
            Debug.Log(deletingUser.GetComponent<UserInfo>().num);
            Destroy(deletingUser);
        }
    }
    
    public void OnExit() 
    {
        Application.Quit();
    }
}
