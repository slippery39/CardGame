using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SimService
{
    //Returns the result of the game;
    public CardGame SimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        var cardGame = new CardGame();
        cardGame.SetupPlayers(player1Deck, player2Deck);
        //On Game Over needs to be an observable
        cardGame.OnGameOverObservable.Subscribe((c) => Debug.Log($"Game has ended. {c.WinLoseSystem.GetGameOverInfo().Winner.Name} is the winner"));
        cardGame.StartGame();

        while (cardGame.CurrentGameState != GameState.GameOver)
        {           
            new DefaultBrain().GetNextAction(cardGame);
        }

        return cardGame;
    }

    public void SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {

        int gameId = 1;
        Debug.Log($"Simulation has started for game {gameId}");
        SimulateGame(player1Deck, player2Deck);
    }
}
