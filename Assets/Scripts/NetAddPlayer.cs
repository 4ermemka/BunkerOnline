[System.Serializable]
public  class Net_AddPlayer:NetMsg
{
    public Net_AddPlayer()
    {
        OP = NetOP.AddPlayer;
    }
    public string Username;
}