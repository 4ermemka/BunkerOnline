[System.Serializable]
public  class Net_UpdateVotingList : NetMsg
{
    public Net_UpdateVotingList()
    {
        OP = NetOP.UpdateVotingList;
    }
    public int[] votingArray { get; set; }
}