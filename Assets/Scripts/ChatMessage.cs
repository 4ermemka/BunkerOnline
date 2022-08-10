using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Nickname;
    [SerializeField] private TextMeshProUGUI MessageText;

    public void SetNickname(string nick) 
    {
        Nickname.text = nick;    
    }

    public void SetMessage(string msg) 
    {
        MessageText.text = msg;
    }
}
