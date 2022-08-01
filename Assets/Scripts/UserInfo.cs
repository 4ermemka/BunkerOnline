using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UserInfo:MonoBehaviour
{
    public int id = 0;
    public int num = 0;
    public bool isHost = false;
    public string nickname = "/*default nick*/";
    private TextMeshProUGUI nick;
    private TextMeshProUGUI hostStatus;
    private TextMeshProUGUI number;
    
    public void Awake() 
    {
        number = GetComponentsInChildren<TextMeshProUGUI>()[0];
        hostStatus = GetComponentsInChildren<TextMeshProUGUI>()[1];
        nick = GetComponentsInChildren<TextMeshProUGUI>()[2];
    }

    public void setId(int id)
    {
        this.id = id;
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
