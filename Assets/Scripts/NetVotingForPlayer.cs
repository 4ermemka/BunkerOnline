[System.Serializable]
public  class Net_VotingForPlayer : NetMsg
{
    public Net_VotingForPlayer()
    {
        OP = NetOP.VotingForPlayer;
    }
    public int id {get; set;}
}