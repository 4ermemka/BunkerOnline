public static class NetOP
{
    public const int None = 0;
    public const int SetGlobalId = 1;
    public const int AddUser = 2;
    public const int UpdateUser = 3;
    public const int LeaveUser = 4;
    public const int UpdateCardPlayer = 5;
    public const int AllUsersInfo = 6;
    public const int CastCardPlayer = 7;
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