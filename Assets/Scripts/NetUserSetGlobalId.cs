[System.Serializable]
public  class NetUser_SetGlobalId : NetMsg
{
    public NetUser_SetGlobalId()
    {
        OP = NetOP.SetGlobalId;
    }
    public int globalConId { set; get; } 
}
