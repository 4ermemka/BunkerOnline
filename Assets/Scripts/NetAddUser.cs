[System.Serializable]
public  class Net_AddUser:NetMsg
{
    public Net_AddUser()
    {
        OP = NetOP.AddUser;
    }

    public string Username { set; get; }
}