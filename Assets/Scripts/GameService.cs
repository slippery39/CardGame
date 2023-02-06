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

    #region Public Methods
    public void SetupGame(Decklist player1Deck, Decklist player2Deck)
    {
        _cardGame = new CardGame();
        _cardGame.SetupPlayers(player1Deck, player2Deck);
    }
    public void StartGame()
    {
        if (_cardGame == null)
        {
            throw new Exception("StartGame() has failed. Game is null. Perhaps you forgot to Setup the game first?");
        }
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
        return _cardGame.GameEventSystem.GameEventObservable;
    }

    public IObservable<CardGame> GetOnGameStateUpdatedObservable()
    {
        var gameStateObs = _cardGame.GameStateChangedObservable;
        return gameStateObs;
    }
    #endregion
}
