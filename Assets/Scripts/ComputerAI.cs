using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(GameService))]
public class ComputerAI : MonoBehaviour
{
    [SerializeField]
    private int playerId;

    [SerializeField]
    private string previousDebugMessage = "";

    [SerializeField]
    private int _calculations;
    [SerializeField]
    private bool _disabled;

    [SerializeField]
    private GameService _gameService;

    // Start is called before the first frame update
    void Start()
    {
        _gameService = this.GetComponent<GameService>();
        //The AI will attempt to select his action every 0.5 seconds.
        Observable.Interval(TimeSpan.FromSeconds(0.1)).Subscribe((_) =>
        {
                TryChooseAction();
        });
    }

    private void TryChooseAction()
    {
        if (!_gameService.HasGameStarted)
        {
            return;
        }

        if (_gameService.GameOver)
        {
            return;
        }

        Profiler.BeginSample("AI.ChoosingAction");
        var cardGame = _gameService.CardGame;

        if (cardGame.ActivePlayer.PlayerId == playerId)
        {
            ChooseActionBase(cardGame);
        }

        Profiler.EndSample();
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
        var totalMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 40;
        score += totalMana;

        var anyMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 40;
        score += anyMana;


        //Temporary Power is what we should calculate (ie haze of rage


        //Other things to check

        //Favorable matchup in a lane.

        //If they are winning the game.

        return score;
    }

    private List<StateActionNode> CalculateStateActionScoresForState(CardGame cardGame, StateActionNode parent, int currentDepth)
    {        
        int maxDepth = 3;

        if (currentDepth > maxDepth)
        {
            return null; //end this branch;
        }

        if (cardGame.CurrentGameState == GameState.GameOver)
        {
            //game is already over, no need to continue;
            return null;
        }

        List<CardGameAction> validActions = new List<CardGameAction>();

        if (cardGame.CurrentGameState == GameState.WaitingForAction)
        {
            var availableActions = cardGame.ActivePlayer.GetAvailableActions(cardGame);
            validActions = availableActions.Where(a => a.IsValidAction(cardGame)).ToList();

            //Always only consider playing the lands first
            if (validActions.Where(a=>a is PlayManaAction).Any())
            {
                validActions = validActions.Where(a => a is PlayManaAction).ToList();
            }

        }
        else if (cardGame.CurrentGameState == GameState.WaitingForChoice)
        {
            var resolveChoiceAction = new ResolveChoiceAction()
            {
                Player = cardGame.ActivePlayer
            };
            resolveChoiceAction.Choices = resolveChoiceAction.GetValidChoices(cardGame).Take(cardGame.ChoiceInfoNeeded.NumberOfChoices).ToList();
            validActions = new List<CardGameAction>
            {
                resolveChoiceAction
            };
        }

        if (validActions.IsNullOrEmpty())
        {
            return null;
        }

        //Only add to the calculations on number of valid choices.
        _calculations++;

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
                Score = score,
                BestScoreIncludingChildren = 0, //calculated later on,
                OriginalScore = originalScore,
                ResultingState = gameStateCopy,
                Depth = currentDepth,
                Parent = parent
            };
            //TODO - Recursively or Iteratively Add Depth.
        });

        //Some optimzation needed here, if an action results in a really large increase in score, then automatically use that action, and do not go any further.
        //If an action results in a really low decrease in score, then do not go any furhter.




        foreach (var node in actionScores)
        {
            //Neutral Actions or only slightly beneficial actions can be pruned for performance reasons
            if (node.Score - node.OriginalScore <= 10)
            {
                continue;
            }

            node.Children = CalculateStateActionScoresForState(node.ResultingState, node, currentDepth + 1);
        }

        return actionScores.Where(a => a != null).ToList();
    }


    private int FindBestScoreForNode(StateActionNode node, int currentBest)
    {
        if (node == null)
        {
            return -999999;
        }

        var bestScore = Math.Max(currentBest, node.Score);

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                var childScore = FindBestScoreForNode(child, bestScore);
                bestScore = Math.Max(bestScore, childScore);
            }
        }

        return bestScore;
    }


    private void ChooseActionBase(CardGame cardGame)
    {
        if (_disabled)
        {
            return;
        }


        var gameService = _gameService;

        //We need an edge case for choices.

        /*
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
        */

        Profiler.BeginSample("AI Calculating Scores");
        var actionScores = CalculateStateActionScoresForState(cardGame, null, 1);
        Profiler.EndSample();

        //Now we need to sort them based on the best score achieved.

        Profiler.BeginSample("AI sorting scores");
        if (actionScores == null)
        {
            actionScores = new List<StateActionNode>();
        }
        actionScores.Sort((a, b) =>
        {
            a.BestScoreIncludingChildren = FindBestScoreForNode(a, -999999);
            b.BestScoreIncludingChildren = FindBestScoreForNode(b, -999999);
            return b.BestScoreIncludingChildren - a.BestScoreIncludingChildren;
        });
        Profiler.EndSample();


        var originalScore = EvaluateBoard(cardGame.ActivePlayer, cardGame);
        var positiveActions = actionScores.Where(a => a.Score >= originalScore).ToList();

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
            Debug.Log("No positive actions found, ending turn");
            gameService.ProcessAction(new NextTurnAction());
            //end turn.
        }
    }
}


public class StateActionNode
{
    public CardGameAction Action { get; set; }
    public CardGame OriginalState { get; set; }
    public CardGame ResultingState { get; set; }
    public int BestScoreIncludingChildren { get; set; }
    public int Score { get; set; }
    public int OriginalScore { get; set; }
    public int Depth { get; set; }

    public List<StateActionNode> Children { get; set; }
    public StateActionNode Parent { get; set; }
}
