[System.Serializable]
public  class Net_LeavePlayer:NetMsg
{
    public Net_LeavePlayer()
    {
        OP = NetOP.LeavePlayer;
    }
    public string Username { set; get; } 
}