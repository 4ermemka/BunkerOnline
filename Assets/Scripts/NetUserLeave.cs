[System.Serializable]
public  class NetUser_Leave : NetMsg
{
    public NetUser_Leave()
    {
        OP = NetOP.LeaveUser;
    }
    public string Username { set; get; } 
}