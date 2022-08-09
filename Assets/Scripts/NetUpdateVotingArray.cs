[System.Serializable]
public  class Net_UpdateVotingArray : NetMsg
{
    public Net_UpdateVotingArray()
    {
        OP = NetOP.UpdateVotingArray;
    }
    public int[] votingArray { get; set; }
}
