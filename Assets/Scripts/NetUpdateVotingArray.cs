[System.Serializable]
public  class Net_UpdateVotingArray : NetMsg
{
    public Net_UpdateVotingArray()
    {
        OP = NetOP.UpdateVotingList;
    }
    public int[] votingArray { get; set; }
}