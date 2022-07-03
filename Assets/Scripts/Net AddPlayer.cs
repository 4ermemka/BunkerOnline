[System.Serializable]
public  class Net_AddPlayer:NetMsg
{
    public Net_AddPlayer()
    {
        OP = NetOP.AddPlayer;
    }

    public string Username;
    public string OpenedCards; //that must be ONE string
}