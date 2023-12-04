using UnityEngine;

public class UnityCardGameLogger : ICardGameLogger
{
    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
}