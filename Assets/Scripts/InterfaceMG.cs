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
    #region Events
    public event EventHandler<OnClickConnectEventArgs> OnClickConnect;
    public event EventHandler OnChooseClient;
    public event EventHandler OnChooseServer;
    public event EventHandler OnReturnToMenu;

    public class OnClickConnectEventArgs:EventArgs
    {
        public string server_ip;
    }
    #endregion

    #region InterfaceFields
    [SerializeField] private GameManager gm;
    [SerializeField] private Text userNickDisplay;
    [SerializeField] private GameObject userInfo;
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private InputField NicknameField;
    [SerializeField] private InputField ipAdressField;
    [SerializeField] private Text errMsg;
    [SerializeField] private Text connectionStatusText;

    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    #endregion

    public void Start()
    {
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.MainMenu);

        gm.OnUserUpdated += WhenNicknameChanged;
    }

    public void Update()
    {
        lobbyList.GetComponent<GridLayoutGroup>().cellSize = new Vector3((lobbyList.GetComponent<RectTransform>().rect.width - 15)/2, 15, 1);
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
        if(gm.user.name == "") 
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
        if(gm.user.name == "") 
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

    public void ClientClick() 
    {
        OnChooseClient?.Invoke(this, EventArgs.Empty);
        SwitchToConnectionMenu();
    }

    public void HostClick() 
    {
        OnChooseServer?.Invoke(this, EventArgs.Empty);
        SwitchToLobby();
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
            gm.UpdateUsername(NicknameField.text);
        else errMsg.text = "Empty nickname not allowed!";
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
        ClearLobby();
        int i = 0;
        foreach (User u in users)
        {
            GameObject panel = Instantiate(userInfo) as GameObject;
            UserInfo panelInfo = panel.GetComponent<UserInfo>();

            panelInfo.setId(u.id);
            panelInfo.setNickname(u.name);
            panelInfo.toggleHost(u.isHost);
            panelInfo.setNum(++i);

            panelInfo.setPanelToList(lobbyList);
        }
    }

    public void ClearLobby()
    {
        foreach (Transform p in lobbyList.transform) Destroy(p.gameObject);
    }

    public void NewConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    public void ResetErrors()
    {
        connectionStatusText.text = "";
        errMsg.text = "";
    }

    public void WhenNicknameChanged(object sender, GameManager.OnUserUpdatedEventArgs e)
    {
        userNickDisplay.text = e.newName;
    }

    #endregion
}
