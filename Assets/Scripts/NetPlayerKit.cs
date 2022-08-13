[System.Serializable]
public class Net_PlayerKit : NetMsg
{
    public Net_PlayerKit()
    {
        OP = NetOP.PlayerKit;
    }
    public DeckCard[] cards { get; set; }
}