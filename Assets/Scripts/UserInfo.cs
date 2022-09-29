using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UserInfo:MonoBehaviour
{
    public int id;
    public int num;
    public bool isHost;
    public string nickname = "/*default nick*/";

    [SerializeField] float animationTime;
    [SerializeField] private TextMeshProUGUI nick;
    [SerializeField] private TextMeshProUGUI hostStatus;
    [SerializeField] private TextMeshProUGUI number;

    public void Start() 
    {
        id = 0;
        if(animationTime <= 0) animationTime*=-1 + 1;
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        if(Application.isPlaying) LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, animationTime);
    }

    public void Update() 
    {
        if(animationTime <= 0) animationTime*=-1 + 0.5f;
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
        transform.SetParent(list.transform);
        transform.localScale = new Vector3(1,1,1);
    }
}
