[System.Serializable]
public  class Net_UpdateChat : NetMsg
{
    public Net_UpdateChat()
    {
        OP = NetOP.UpdateChat;
    }
    public string Nickname { get; set; }
    public string message { get; set; }
}