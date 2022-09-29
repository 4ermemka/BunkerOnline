public static class NetOP
{
    public const int None = 0;
    public const int SetGlobalId = 1;
    public const int AddUser = 2;
    public const int UpdateUser = 3;
    public const int LeaveUser = 4;
    public const int UpdateCardPlayer = 5;
    public const int AllUsersInfo = 6;
    public const int CastCard = 7;
    public const int LobbyStarted = 8;
    public const int GameStarted = 9;
    public const int ReadyForGame = 10;
    public const int PlayerKit = 11;
    public const int KickPlayer = 12;
    public const int PlayerVote = 13;
    public const int UpdateChat = 14;
    public const int VotingForPlayer = 15;
    public const int SwitchTurn = 16;
    public const int ServerReady = 17;
    public const int EndgameOpen = 18;
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