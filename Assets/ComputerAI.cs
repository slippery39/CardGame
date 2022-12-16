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

        var me = board.Players.Where(p => p.EntityId == player.EntityId).FirstOrDefault();
        var opp = board.Players.Where(p => p.EntityId != player.EntityId).FirstOrDefault();

        var myLifeTotal = me.Health;
        var oppLifeTotal = opp.Health;

        var score = 0;

        score += myLifeTotal * 20;
        score -= oppLifeTotal * 20;

        //Power of units in play
        if (me.GetUnitsInPlay().Any())
        {
            var totalPower = me.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score += totalPower * 50;
        }

        //Power of opponent units in play
        if (opp.GetUnitsInPlay().Any())
        {
            var totalPowerOpp = opp.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score -= totalPowerOpp * 50;
        }

        //Total number of resources available.
        var totalNumberOfResources = me.GetCardsInPlay().Count() + me.Hand.Count();
        score += totalNumberOfResources * 40;

        var totalNumberOfOppResources = opp.GetCardsInPlay().Count() + opp.Hand.Count();
        score -= totalNumberOfOppResources * 40;

        //Permanent Mana should be considered a resource, but be sure not to count temporary mana.
        var totalMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 20;
        score += totalMana;

        var anyMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 50;
        score += anyMana;


        //Temporary Power is what we should calculate (ie haze of rage


        //Other things to check

        //Favorable matchup in a lane.

        //If they are winning the game.

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

        //TODO - this is the recursive method.
        var gameState = cardGame;
        var originalScore = EvaluateBoard(cardGame.ActivePlayer, gameState);


        var actionScores = validActions.Select(act =>
        {
            var gameStateCopy = gameState.Copy(true);
            gameStateCopy.ProcessAction(act);
            var score = EvaluateBoard(cardGame.ActivePlayer, gameStateCopy);

            return new StateActionNode
            {
                Action = act,
                Score = score - originalScore,
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

        var positiveActions = actionScoresAsList.Where(a => a.Score >= -4).ToList();

        if (positiveActions.Any())
        {
            //TODO - check if the action will have a negative impact on the current board state.---
            var actionToChoose = positiveActions.First().Action;
            //MiniMax Algorithm
            Debug.Log("Trying to process action");
            gameService.ProcessAction(actionToChoose);
        }
        else
        {
            //end turn.
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
