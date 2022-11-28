using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameService : MonoBehaviour
{
    public IObservable<GameEventLog> GetGameEventLogsObservable()
    {
        return UIGameController.Instance.CardGame.EventLogSystem.GetGameEventLogsAsObservable();
    }
}
