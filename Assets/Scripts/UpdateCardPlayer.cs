[System.Serializable]
public  class Net_UpdateCardPlayer:NetMsg
{
    public Net_UpdateCardPlayer()
    {
        OP = NetOP.UpdateCardPlayer;
    }
    public string Username { set; get; } 
    public string NewCardsOnTable { set; get; } 
}