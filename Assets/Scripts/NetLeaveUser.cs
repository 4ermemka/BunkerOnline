[System.Serializable]
public  class Net_LeaveUser : NetMsg
{
    public Net_LeaveUser()
    {
        OP = NetOP.LeaveUser;
    }
    public string Username { set; get; } 
}