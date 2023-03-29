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
    /**
     * Returns a CardGame where each card that is not currently revealed for the player
     * is set as an unknown card (entity id -1)
     */
    public CardGame GetGameViewForPlayer(CardGame cardGame, int playerId)
    {
        //Old Logic from UICard2D
        //if revealed to all && zonetype is in the hand then show it

        var gameCopy = cardGame.Copy();

        foreach (var cardInstance in gameCopy.GetEntities<CardInstance>())
        {
            //We check each card individually with the logic below,
            //If shouldSeeCard = false, we set it as an unknown card (entity id = -1)
            //The UI will process unknown cards by 

            //Fixes issue regarding dual faced cards
            if (cardInstance.GetZone() == null)
            {
                continue;
            }

            var isOwnTurn = cardGame.ActivePlayerId == cardInstance.OwnerId;
            var isInDeck = cardInstance.GetZone().ZoneType == ZoneType.Deck;
            var isInHand = cardInstance.GetZone().ZoneType == ZoneType.Hand;
            var isVisible = new List<ZoneType> { ZoneType.InPlay, ZoneType.Stack, ZoneType.Discard, ZoneType.Exile }.Contains(cardInstance.GetZone().ZoneType);
            var shouldSeeCard = isVisible || cardInstance.RevealedToAll || (isInDeck && cardInstance.RevealedToOwner && isOwnTurn) || (isInHand && isOwnTurn);
            
            
            if (isInHand)
            {
                var debug = 0;
            }
            //Cards that are revealed to owner
            if (!shouldSeeCard)
            {
                cardInstance.SetUnknown();             
            }
        }

        return gameCopy;
    }
    #endregion
}
