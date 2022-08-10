[System.Serializable]
public class Net_PlayerVote : NetMsg
{
    public Net_PlayerVote()
    {
        OP = NetOP.PlayerVote;
    }
    public User user;
    public int id { get; set; }
}