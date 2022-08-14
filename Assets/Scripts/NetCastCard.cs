[System.Serializable]
public class Net_CastCard : NetMsg
{
    public Net_CastCard()
    {
        OP = NetOP.CastCard;
    }
    public User user { get; set; }
    public DeckCardSerializable card { get; set; }
}