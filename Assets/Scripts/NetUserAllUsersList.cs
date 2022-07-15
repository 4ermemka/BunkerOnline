[System.Serializable]
public  class NetUser_AllUserList : NetMsg
{
    public NetUser_AllUserList()
    {
        OP = NetOP.AllUsersInfo;
    }
    public User[] users { set; get; }   
}