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

    public IObservable<CardGame> GetOnGameStateUpdatedObservable()
    {

        var gameStateObs = UIGameController.Instance.CardGame.GameStateChangedObservable;
        return gameStateObs;
    }
}
