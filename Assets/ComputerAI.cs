using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;

public class ComputerAI : MonoBehaviour
{
    [SerializeField]
    private int playerId;

    [SerializeField]
    private string previousDebugMessage = "";

    // Start is called before the first frame update
    void Start()
    {
        //The AI will attempt to select his action every 1 second.
        Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe((_) => TryChooseAction());
    }

    private void TryChooseAction()
    {

        Profiler.BeginSample("AI.ChoosingAction");

        var gameController = UIGameController.Instance;
        var gameService = UIGameController.Instance.GameService;
        var cardGame = gameController.CardGame;

        if (!gameService.HasGameStarted)
        {
            return;
        }

        if (cardGame.ActivePlayer.PlayerId == playerId)
        {
            if (cardGame.CurrentGameState == GameState.WaitingForAction)
            {
                ChooseAction(cardGame);
            }
            else if (cardGame.CurrentGameState == GameState.WaitingForChoice)
            {
                MakeChoice(gameService, cardGame);
            }
        }
        Profiler.EndSample();
    }

    private void MakeChoice(GameService gameService, CardGame cardGame)
    {
        var choiceInfo = cardGame.ChoiceInfoNeeded;
        var validChoices = choiceInfo.GetValidChoices(cardGame, cardGame.ActivePlayer);
        var choices = validChoices.Randomize().Take(choiceInfo.NumberOfChoices);
        gameService.MakeChoice(choices.ToList());
    }

    private int EvaluateBoard(Player player, CardGame board)
    {
        /*
         * Things to check
         * 
         * Life Totals
         * 
         * Units in play -|
         * Items in play -|- Total Remaining Resources?
         * Cards in hand -|
         * Total Mana
         * 
         */

        var myLifeTotal = board.Players.Where(p => p.EntityId == player.EntityId).FirstOrDefault().Health;
        var oppLifeTotal = board.Players.Where(p => p.EntityId != player.EntityId).FirstOrDefault().Health;

        var score = 0;

        score += myLifeTotal * 20;
        score -= oppLifeTotal * 20;

        //Power of units in play
        if (board.Player1.GetUnitsInPlay().Any())
        {
            var totalPower = board.Player1.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score += totalPower * 30;
        }

        //Power of opponent units in play
        if (board.Player2.GetUnitsInPlay().Any())
        {
            var totalPowerOpp = board.Player2.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score -= totalPowerOpp * 30;
        }

        //Total number of resources available.

        return score;
    }



    private void ChooseAction(CardGame cardGame)
    {
        var gameController = UIGameController.Instance;
        var gameService = gameController.GameService;

        var availableActions = cardGame.ActivePlayer.GetAvailableActions(cardGame);
        var validActions = availableActions.Where(a => a.IsValidAction(cardGame)).ToList();

        var newDebugMsg = $"Number of valid actions {validActions.Count()}";

        if (newDebugMsg != previousDebugMessage)
        {
            Debug.Log(newDebugMsg);
            previousDebugMessage = newDebugMsg;
        }

        if (validActions.Count() == 0)
        {
            return;
        }


        var gameState = cardGame.Copy(true);

        var actionScores = validActions.Select(act =>
        {
            var gameStateCopy = gameState.Copy(true);
            gameStateCopy.ProcessAction(act);
            var score = EvaluateBoard(cardGame.ActivePlayer, gameStateCopy);

            return new StateActionNode
            {
                Action = act,
                Score = score,
                OriginalState = gameState,
                ResultingState = gameStateCopy,
                Depth = 1
            };

            //TODO - Recursively or Iteratively Add Depth.
        });

        var actionScoresAsList = actionScores.Randomize().ToList();
        actionScoresAsList.Sort((a, b) =>
        {
            return b.Score - a.Score;
        });

        var hasValidActions = validActions.Any();

        if (hasValidActions)
        {
            //TODO - check if the action will have a negative impact on the current board state.---
            var actionToChoose = actionScoresAsList.First().Action;
            //MiniMax Algorithm
            Debug.Log("Trying to process action");
            gameService.ProcessAction(actionToChoose);
        }
    }
}


public class StateActionNode
{
    public CardGameAction Action { get; set; }
    public CardGame OriginalState { get; set; }
    public CardGame ResultingState { get; set; }
    public int Score { get; set; }
    public int Depth { get; set; }
    public StateActionNode Children { get; set; }
    public StateActionNode Parent { get; set; }
}
