[System.Serializable]
public  class Net_AllUserList:NetMsg
{
    public Net_AllUserList()
    {
        OP = NetOP.AllUsersInfo;
    }
    public User[] users { set; get; }   
}