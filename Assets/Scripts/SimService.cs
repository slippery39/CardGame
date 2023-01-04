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
           cardGame.ProcessAction(new DefaultBrain().GetNextAction(cardGame));
        }

        return cardGame;
    }

    public List<GameOverInfo> SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {
        var results = new List<GameOverInfo>();
        for (var i=0; i < numberOfGames; i++)
        {
            int gameId = (i + 1);
            Debug.Log($"Simulation has started for game {gameId}");
            var result = SimulateGame(player1Deck, player2Deck);
            results.Add(result.WinLoseSystem.GetGameOverInfo());
            Debug.Log($"Simulation has ended for game {gameId}");
        }

        Debug.Log("Final Results of Sim!");
        Debug.Log("------");
        foreach (var result in results)
        {
            Debug.Log($"Winning Player : {result.Winner.Name}");
        }

        return results;
    }
}
