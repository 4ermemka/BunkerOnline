[System.Serializable]
public class NetUser_Add : NetMsg
{
    public NetUser_Add()
    {
        OP = NetOP.AddUser;
    }

    public string Username { set; get; }
    public int id { set; get; }
}