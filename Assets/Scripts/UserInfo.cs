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

    [SerializeField] private TextMeshProUGUI nick;
    [SerializeField] private TextMeshProUGUI hostStatus;
    [SerializeField] private TextMeshProUGUI number;

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
