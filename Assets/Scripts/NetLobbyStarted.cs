[System.Serializable]
public class Net_LobbyStarted : NetMsg
{
    public Net_LobbyStarted()
    {
        OP = NetOP.LobbyStarted;
    }
}