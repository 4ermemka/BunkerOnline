[System.Serializable]
public  class Net_AllPlayerList:NetMsg
{
    public Net_AllPlayerList()
    {
        OP = NetOP.AllPlayersInfo;
    }
    public Player[] players { set; get; }   
}