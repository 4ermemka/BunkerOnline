using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInfo:MonoBehaviour
{
    public int num = 0;
    public bool isHost = false;
    public string nickname = "/*default nick*/";
    private Text nick;
    private Text hostStatus;
    private Text number;
    
    public void Awake() 
    {
        nick = GetComponentsInChildren<Text>()[0];
        number = GetComponentsInChildren<Text>()[1];
        hostStatus = GetComponentsInChildren<Text>()[2];
    }

    public void setNum(int n)
    {
        num = n;
        number.text = num.ToString() + ".";
    }

    public void toggleHost(bool h)
    {
        isHost = h;
        if(isHost) hostStatus.text = "HOST";
        else hostStatus.text = "";
    }

    public void setNickname(string n)
    {
        nickname = n;
        nick.text = nickname;
    }

    public void setPanelToList(GameObject list)
    {
        transform.parent = list.transform;
        transform.localScale = new Vector3(1,1,1);
    }
}
