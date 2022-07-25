using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    private int messagesCount;
    [SerializeField] private Text Nickname;
    [SerializeField] private Text MessageText;
    [SerializeField] private GameObject chatList;

    [SerializeField] private ChatMessage msgPref;

    public void Start() 
    {
        
    }

    public void AddMessage(string author, string msg) 
    {
        ChatMessage chatMsg = Instantiate(msgPref) as ChatMessage;

        chatMsg.SetMessage(msg);
        chatMsg.SetNickname(author);

        chatMsg.transform.SetParent(chatList.transform);
    }
}
