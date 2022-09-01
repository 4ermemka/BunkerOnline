[System.Serializable]
public  class NetUser_UpdateInfo : NetMsg
{
    public NetUser_UpdateInfo()
    {
        OP = NetOP.UpdateUser;
    }
    public string Nickname { set; get; } 
    public int id { set; get; } 
}