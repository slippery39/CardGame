public interface ICardGameLogger
{
    void Log(string message);
}

public class EmptyLogger : ICardGameLogger
{
    public void Log(string message)
    {

    }
}

public class UnityCardGameLogger : ICardGameLogger
{
    public void Log(string message)
    {
        //Debug.Log(message);
    }
}