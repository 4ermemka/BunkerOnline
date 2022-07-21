[System.Serializable]
public  class NetUser_UpdateInfo : NetMsg
{
    public NetUser_UpdateInfo()
    {
        OP = NetOP.UpdateUser;
    }
    public string Username { set; get; } 
    public int conId { set; get; } 
    public int hostId { set; get; } 
}