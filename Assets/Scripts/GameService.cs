using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameService : MonoBehaviour
{
    #region Fields
    private CardGame _cardGame;
    [SerializeField]
    private bool _hasGameStarted = false;
    #endregion

    #region Properties
    public bool HasGameStarted { get => _hasGameStarted; set => _hasGameStarted = value; }
    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }
    public bool GameOver { get => _cardGame.CurrentGameState == GameState.GameOver; }
    #endregion

    private readonly ReplaySubject<GameEvent> _gameEventSubject = new(1);

    #region Public Methods
    public void SetupGame(GameSetupOptions options)
    {
        _cardGame = new CardGame();
        _cardGame.Setup(options);
   }
    public void StartGame()
    {
        if (_cardGame == null)
        {
            throw new Exception("StartGame() has failed. Game is null. Perhaps you forgot to Setup the game first?");
        }

        //Push any events happening from the card game to this 
        _cardGame.GameEventSystem.GameEventObservable.Subscribe((gameEvent) =>
        {
            this._gameEventSubject.OnNext(gameEvent);
        });

        _cardGame.StartGame();
        _hasGameStarted = true;
    }

    public void ProcessAction(CardGameAction action)
    {
        if (_cardGame == null)
        {
            throw new Exception("ProcessAction() has failed. Game is null. Perhaps you forgot to Setup the game first?");
        }

        _cardGame.ProcessAction(action);
    }
    
    public void EndTurn()
    {
        if (_cardGame == null)
        {
            throw new Exception("EndTurn() has failed. Game is null. Perhaps you forgot to Setup the game first?");
        }
        var endTurnAction = new NextTurnAction();
        _cardGame.ProcessAction(endTurnAction);
    }

    public void MakeChoice(List<CardInstance> choices)
    {
        _cardGame.MakeChoice(choices);
    }

    public IObservable<GameEventLog> GetGameEventLogsObservable()
    {
        return _cardGame.EventLogSystem.GetGameEventLogsAsObservable();
    }

    public IObservable<GameEvent> GetGameEventObservable()
    {
        return _gameEventSubject.AsObservable();
    }

    public IObservable<CardGame> GetOnGameStateUpdatedObservable()
    {
        var gameStateObs = _cardGame.GameStateChangedObservable;
        return gameStateObs;
    }
    #endregion
}
