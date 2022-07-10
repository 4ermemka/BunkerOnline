public static class NetOP
{
    public const int None = 0;
    public const int AddUser = 1;
    public const int LeaveUser = 2;
    public const int UpdateCardPlayer = 3;
    public const int AllUsersInfo = 4;
    public const int CastCardPlayer = 5;
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