[System.Serializable]
public class Net_PlayerVote : NetMsg
{
    public Net_PlayerVote()
    {
        OP = NetOP.PlayerVote;
    }
    public int author { get; set; }
    public int id { get; set; }
}
