[System.Serializable]
public class Net_ReadyForGame : NetMsg
{
    public Net_ReadyForGame()
    {
        OP = NetOP.ReadyForGame;
    }
    public int userId;
}