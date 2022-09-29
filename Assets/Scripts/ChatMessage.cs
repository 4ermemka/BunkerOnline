using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Nickname;
    [SerializeField] private TextMeshProUGUI MessageText;
    [SerializeField] private Color AuthorColor;
    [SerializeField] private Color MessageColor;
    [SerializeField] [TextArea] private string author;
    [SerializeField] [TextArea] private string message;

    
    public void Update()
    {
        Nickname.text = author;
        MessageText.text = message;
        Nickname.color = AuthorColor;
        MessageText.color = MessageColor;
    }

    public void SetNickname(string nick) 
    {
        author = nick;
        Nickname.text = nick;    
    }

    public void SetMessage(string msg) 
    {
        message = msg;
        MessageText.text = msg;
    }

    public void SetColor(string hexCodeAuthor, string hexCodeMessage)
    {
        AuthorColor = ColorHandler.GetColorFromString(hexCodeAuthor);
        MessageColor = ColorHandler.GetColorFromString(hexCodeMessage);
    }
}
