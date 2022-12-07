using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameService : MonoBehaviour
{
    #region Fields
    private CardGame _cardGame;
    private bool _hasGameStarted = false;
    #endregion

    #region Properties
    public bool HasGameStarted { get => _hasGameStarted; set => _hasGameStarted = value; }
    #endregion

    #region Public Methods
    public void SetupGame(Decklist player1Deck, Decklist player2Deck)
    {
        _cardGame = new CardGame();
        _cardGame.SetupDecks(player1Deck, player2Deck);
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
        _cardGame.ProcessAction(action);
    }

    public IObservable<GameEventLog> GetGameEventLogsObservable()
    {
        return _cardGame.EventLogSystem.GetGameEventLogsAsObservable();
    }

    public IObservable<CardGame> GetOnGameStateUpdatedObservable()
    {
        var gameStateObs = _cardGame.GameStateChangedObservable;
        gameStateObs.Subscribe(gameState =>
        {
            if (gameState?.Player1?.Lanes?[0].UnitInLane?.Name == "Arcbound Ravager")
            {
                var i = 0;
            }
            Debug.Log("Game State Changed has triggered in the Game Service");
        });
        return gameStateObs;
    }
    #endregion
}
