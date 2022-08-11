[System.Serializable]
public class Net_KickPlayer : NetMsg
{
    public Net_KickPlayer()
    {
        OP = NetOP.KickPlayer;
    }

    public int id;
}