using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CanvasType
{
    MainMenu,
    ConnectionMenu,
    LobbyMenu
}

public class MenuInterfaceManager : MonoBehaviour
{
    #region Events
    public event EventHandler<OnClickConnectEventArgs> OnClickConnect;
    public event EventHandler OnChooseClient;
    public event EventHandler OnChooseServer;
    public event EventHandler OnReturnToMenu;
    public static event EventHandler OnStartGame;

    public class OnClickConnectEventArgs:EventArgs
    {
        public string server_ip;
    }
    #endregion

    #region InterfaceFields
    [SerializeField] private NetManager nm;
    [SerializeField] private TextMeshProUGUI userNickDisplay;
    [SerializeField] public UserInfo userInfo;
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private TMP_InputField NicknameField;
    [SerializeField] private TMP_InputField ipAdressField;
    [SerializeField] private TextMeshProUGUI errMsg;
    [SerializeField] private TextMeshProUGUI lobbyErrMsg;
    [SerializeField] private TextMeshProUGUI connectionStatusText;

    [SerializeField] private Button StartBtn;

    private int minPlayersCountToStart = 2;
    private int maxPlayersCountToStart;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    #endregion

    public void Start()
    {
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.MainMenu);

        nm.OnUserUpdated += WhenNicknameChanged;
    }

    public void Update()
    {
    }

    #region MultipleMenuNavigation

    /////////////////////////////////////////////////////////////////////////////////
    ////////////////         Switching between menu types            ////////////////
    /////////////////////////////////////////////////////////////////////////////////

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
        if(nm.user.Nickname == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else 
        {
            SwitchCanvas(CanvasType.LobbyMenu);
        }
    }

    public void SwitchToConnectionMenu()
    {
        if(nm.user.Nickname == "") 
        {
            SwitchToMainMenu();
            errMsg.text = "Nickname is empty!";
        }
        else
        {
            SwitchCanvas(CanvasType.ConnectionMenu);
        }
    }
    
    #endregion

    #region InterfaceInterraction

    /////////////////////////////////////////////////////////////////////////////////
    ////////////////          Reading info from interface            ////////////////
    /////////////////////////////////////////////////////////////////////////////////

    public void SwitchWindowMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.SetResolution(1920,1080,Screen.fullScreen);
    }

    public void ClientClick() 
    {
        OnChooseClient?.Invoke(this, EventArgs.Empty);
        SwitchToConnectionMenu();
        StartBtn.gameObject.SetActive(false);
    }

    public void HostClick() 
    {
        OnChooseServer?.Invoke(this, EventArgs.Empty);
        SwitchToLobby();
        StartBtn.gameObject.SetActive(true);
    }

    public void ConnectClick()
    {
        if(ipAdressField.text != string.Empty) 
            {
                NewConnectionStatus("Connecting to " + ipAdressField.text + "...");
                OnClickConnect?.Invoke(this, new OnClickConnectEventArgs {server_ip = ipAdressField.text});
            }
        else NewConnectionStatus("Enter adress first!");
    }

    public void BackClick() 
    {
        OnReturnToMenu?.Invoke(this, EventArgs.Empty);
        SwitchToMainMenu();
    }

    public void ApplyNickClick() 
    {
        if(NicknameField.text != "")
            nm.UpdateUsername(NicknameField.text);
        else errMsg.text = "Empty nickname not allowed!";
    }

    public void StartGameClick()
    {
        int connectedCount = lobbyList.transform.childCount;
        if(connectedCount >= minPlayersCountToStart)
        {
            OnStartGame?.Invoke(this, EventArgs.Empty);
        }
        else lobbyErrMsg.text = "Not enough players!";
    }

    public void ExitClick() 
    {
        Application.Quit();
    }

    #endregion

    #region UpdateInterface

    /////////////////////////////////////////////////////////////////////////////////
    ////////////////              Updating UI elements               ////////////////
    /////////////////////////////////////////////////////////////////////////////////

    public void UpdateLobby(List<User> users)
    {
        Debug.Log(users == null);
        ClearLobby();
        int i = 0;
        foreach (User u in users)
        {
            UserInfo panelInfo = Instantiate(userInfo) as UserInfo;

            panelInfo.setId(u.id);
            panelInfo.setNickname(u.Nickname);
            panelInfo.toggleHost(u.isHost);
            panelInfo.setNum(++i);

            panelInfo.setPanelToList(lobbyList);
        }
    }

    public void ClearLobby()
    {
        if(lobbyList != null)
            foreach (Transform p in lobbyList.transform) Destroy(p.gameObject);
    }

    public void NewConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    public void ResetErrors()
    {
        connectionStatusText.text = "";
        lobbyErrMsg.text = "";
        errMsg.text = "";
    }

    public void WhenNicknameChanged(object sender, NetManager.OnUserUpdatedEventArgs e)
    {
        userNickDisplay.text = e.newName;
    }

    #endregion
}
