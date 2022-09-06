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
    private Server server;
    private Client client;

    private void Update() 
    {
        if(transform.childCount > 10)
            Destroy(transform.GetChild(0).gameObject);
    }

    public void Start() 
    {
        server = FindObjectOfType<Server>();
        client = FindObjectOfType<Client>();
        MessageProcessing.SetChatManager(this); 
    }

    public void SetNickname(string nick) 
    {
        myNickname = nick;
    }

    public void AddMessage(string author, string msg) 
    {
        ChatMessage chatMsg = Instantiate(msgPref) as ChatMessage;

        chatMsg.SetColor("FFBC00");
        chatMsg.SetMessage(msg);
        chatMsg.SetNickname(author);

        chatMsg.transform.SetParent(chatList.transform);
        chatMsg.transform.localScale = new Vector3(1,1,1);
    }

    public void SystemMessage(string author, string msg, string hexColor)
    {
        ChatMessage chatMsg = Instantiate(msgPref) as ChatMessage;

        chatMsg.SetColor(hexColor);
        chatMsg.SetMessage(msg);
        chatMsg.SetNickname(author);

        chatMsg.transform.SetParent(chatList.transform);
        chatMsg.transform.localScale = new Vector3(1,1,1);
    }

    public void WriteMessage()
    {
        if(msgField.text!=string.Empty) 
            {
                AddMessage(this.myNickname, msgField.text);
                if(server != null)
                {
                    server.SendOther(MessageProcessing.WriteChatMsg(myNickname, msgField.text));
                }
                if(client != null)
                {
                    client.SendServer(MessageProcessing.WriteChatMsg(myNickname, msgField.text));
                }
            }
        msgField.text = string.Empty;
    }
}
