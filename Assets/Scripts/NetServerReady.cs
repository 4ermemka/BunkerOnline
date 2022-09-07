[System.Serializable]
public class Net_ServerReady : NetMsg
{
    public Net_ServerReady()
    {
        OP = NetOP.ServerReady;
    }
}