[System.Serializable]
public  class Net_SetGlobalId : NetMsg
{
    public Net_SetGlobalId()
    {
        OP = NetOP.SetGlobalId;
    }
    public int globalConId { set; get; } 
}
