using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Text Nickname;
    [SerializeField] private Text MessageText;

    public void SetNickname(string nick) 
    {
        Nickname.text = nick;    
    }

    public void SetMessage(string msg) 
    {
        MessageText.text = msg;
    }
}
