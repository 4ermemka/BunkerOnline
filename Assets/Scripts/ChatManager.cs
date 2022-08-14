using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour
{
    private int messagesCount;
    private string myNickname = "4ermemka";
    [SerializeField] private GameObject chatList;

    [SerializeField] private ChatMessage msgPref;

    [SerializeField] private TMP_InputField msgField;

    public void Start() 
    {
        
    }

    public void SetNickname(string nick) 
    {
        myNickname = nick;
    }

    public void AddMessage(string author, string msg) 
    {
        ChatMessage chatMsg = Instantiate(msgPref) as ChatMessage;

        chatMsg.SetMessage(msg);
        chatMsg.SetNickname(author);

        chatMsg.transform.SetParent(chatList.transform);
        chatMsg.transform.localScale = new Vector3(1,1,1);
    }

    public void WriteMessage()
    {
        if(msgField.text!=string.Empty) AddMessage(this.myNickname, msgField.text);
        msgField.text = string.Empty;
    }
}
