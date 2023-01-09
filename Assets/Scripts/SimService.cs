using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class SimService
{
    //Returns the result of the game;
    public SimResultData SimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        var cardGame = new CardGame();
        //Want to make sure events are not firing;
        cardGame.EventLogSystem = new EmptyEventLogSystem();
        cardGame.SetupPlayers(player1Deck, player2Deck);
        //On Game Over needs to be an observable
        cardGame.OnGameOverObservable.Subscribe((c) => Debug.Log($"Game has ended. {c.WinLoseSystem?.GetGameOverInfo().Winner.Name} is the winner"));
        cardGame.StartGame();

        int numActions = 0;
        while (cardGame.CurrentGameState != GameState.GameOver)
        {
            var nextAction = new DefaultBrain().GetNextAction(cardGame);
            cardGame.ProcessAction(nextAction);
            numActions++;
            if (numActions > 1000)
            {
                var i = 0;
            }
        }

        var gameOverInfo = cardGame.WinLoseSystem.GetGameOverInfo();
        SimResult simResult;

        if (gameOverInfo.IsDraw)
        {
            simResult = SimResult.Draw;
        }
        else if (gameOverInfo.Winner.PlayerId == 1)
        {
            simResult = SimResult.Win;
        }
        else
        {
            simResult = SimResult.Loss;
        }

        var result = new SimResultData()
        {
            Deck1 = player1Deck.Name,
            Deck2 = player2Deck.Name,
            Result = simResult
        };

        return result;
    }

    public List<SimResultData> SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {
        var results = new List<SimResultData>();
        for (var i = 0; i < numberOfGames; i++)
        {
            int gameId = (i + 1);
            Debug.Log($"Simulation has started for game {gameId}");
            var result = SimulateGame(player1Deck, player2Deck);
            results.Add(result);
            Debug.Log($"Simulation has ended for game {gameId}");
        }

        Debug.Log("Final Results of Sim!");
        Debug.Log("------");

        //Formatting for our table

        Dictionary<string, SimResultDataForDeck> resultDataPerDeck = new Dictionary<string, SimResultDataForDeck>();
       
        foreach (var result in results)
        {            
            if (!resultDataPerDeck.ContainsKey(result.Deck1))
            {
                resultDataPerDeck.Add(result.Deck1, new SimResultDataForDeck());
            }
            if (!resultDataPerDeck.ContainsKey(result.Deck2))
            {
                resultDataPerDeck.Add(result.Deck2, new SimResultDataForDeck());
            }
            var deck1Result = resultDataPerDeck[result.Deck1];
            deck1Result.DeckName = result.Deck1;

            var deck2Result = resultDataPerDeck[result.Deck2];
            deck2Result.DeckName = result.Deck2;

            if (result.Result == SimResult.Win)
            {
                deck1Result.Wins++;
                deck2Result.Losses++;
            }
            else if (result.Result == SimResult.Loss)
            {
                deck1Result.Losses++;
                deck2Result.Wins++;
            }
            else
            {
                deck1Result.Draws++;
                deck2Result.Draws++;
            }
        }

        //Quick Formatting.
        //Sort by win percentage

        var deckResultsAsList = resultDataPerDeck.Values.ToList();
        deckResultsAsList.Sort((a, b) =>
        {
            if (a.WinPercentage > b.WinPercentage)
            {
                return 1;
            }
            return -1;
        });

        Debug.Log("Simulation Finished.... Results : ");


        //This would go inside the table.

        deckResultsAsList.ForEach(deckResult =>
        {
            Debug.Log($" Deck Name: {deckResult.DeckName}{Environment.NewLine} Wins: {deckResult.Wins}, Losses: {deckResult.Losses}, Draws: {deckResult.Draws}, Win Percentage: {deckResult.WinPercentage}");
        });
               
        


        return results;
    }
}

public class SimResultDataForDeck
{
    public string DeckName;
    public int GamesPlayed { get => Wins + Losses + Draws; }
    public int Wins;
    public int Losses;
    public int Draws;
    public float WinPercentage { get=> (float)(Wins + Losses) / (float)GamesPlayed;  }
}
