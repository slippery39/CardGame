using UnityEngine;

public interface ICardGameLogger
{
    void Log(string message);
}


public class UnityCardGameLogger : ICardGameLogger
{
    public void Log(string message)
    {
        Debug.Log(message);
    }
}