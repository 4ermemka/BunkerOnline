[System.Serializable]
public class Net_EndgameOpen : NetMsg
{
    public Net_EndgameOpen()
    {
        OP = NetOP.EndgameOpen;
    }    
    public int id;
    public DeckCardSerializable card;
}