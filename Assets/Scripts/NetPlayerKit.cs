[System.Serializable]
public class Net_PlayerKit : NetMsg
{
    public Net_PlayerKit()
    {
        OP = NetOP.PlayerKit;
    }
    public DeckCardSerializable card { get; set; }
}