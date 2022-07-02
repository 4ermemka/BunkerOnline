public static class NetOP
{
    public const int None = 0;
    public const int AddPlayer = 1;
    public const int LeavePlayer = 2;
    public const int UpdateCardPlayer = 3;
    public const int CastCardPlayer = 4;
}

[System.Serializable]
public abstract class NetMsg
{
    public byte OP { set; get; } 

    public NetMsg()
    {
        OP = NetOP.None;
    }
}