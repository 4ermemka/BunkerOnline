[System.Serializable]
public  class Net_UpdateUser : NetMsg
{
    public Net_UpdateUser()
    {
        OP = NetOP.UpdateUser;
    }
    public string Username { set; get; } 
    public int conId { set; get; } 
    public int hostId { set; get; } 
}