[System.Serializable]
public class Net_GameStarted : NetMsg
{
    public Net_GameStarted()
    {
        OP = NetOP.GameStarted;
    }
}