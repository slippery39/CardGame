using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IBrain
{
    CardGameAction GetNextAction(CardGame cardGame);
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

public class DefaultBrain : IBrain
{
    private int _calculations;
    public CardGameAction GetNextAction(CardGame cardGame)
    {
        _calculations = 0;
        return ChooseActionBase(cardGame);
    }

    private void Log(string message)
    {
        Debug.Log($"AI Brain : {message}");
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

    private CardGameAction ChooseActionBase(CardGame cardGame)
    {
        var actionScores = CalculateStateActionScoresForState(cardGame, null, 1);
        if (actionScores == null)
        {
            actionScores = new List<StateActionNode>();
        }
        actionScores.ForEach(node =>
        {
            node.BestScoreIncludingChildren = FindBestScoreForNode(node, -999999);
        });
        actionScores.Sort((a, b) =>
        {
            return b.BestScoreIncludingChildren - a.BestScoreIncludingChildren;
        });

        var originalScore = EvaluateBoard(cardGame.ActivePlayer, cardGame);
        var positiveActions = actionScores.Where(a => (a.BestScoreIncludingChildren >= originalScore-20) || a.Action is ResolveChoiceAction).ToList();

        Log($"Original Score : {originalScore}");
        actionScores.ForEach(a =>
        {
           Log($"Score for Action : {a.Action.SourceCard?.Name} : {a.Action.ToUIString()} : Best Score : {a.BestScoreIncludingChildren}. Depth 1 Score : {a.Score}");
        });

        if (positiveActions.Any())
        {
            var actionToChoose = positiveActions[0].Action;
            Log($"Choosing Action : ${actionToChoose.ToUIString()}");
            return actionToChoose;
        }
        else
        {
            Log("No Positive Actions Found");
            return new NextTurnAction();
        }
    }
    private List<StateActionNode> CalculateStateActionScoresForState(CardGame cardGame, StateActionNode parent, int currentDepth)
    {
        int maxDepth = 3;

        if (currentDepth > maxDepth)
        {
            Log($"Entering a depth of {currentDepth}");
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
            if (validActions.Exists(a => a is PlayManaAction))
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
        }).ToList();

        //Some optimzation needed here, if an action results in a really large increase in score, then automatically use that action, and do not go any further.
        //If an action results in a really low decrease in score, then do not go any furhter.

        foreach (var node in actionScores)
        {
            //Neutral Actions or only slightly beneficial actions can be pruned for performance reasons
            //This needs to account for actions that have a choice in between.. whic hit is currently not doing so.
            //If next turn action is the best, then don't attempt to go anymore.
            if (node.OriginalScore - node.Score <=-500 || node.Action is NextTurnAction)
            {
                continue;
            }

            node.Children = CalculateStateActionScoresForState(node.ResultingState, node, currentDepth + 1);
        }

        return actionScores.Where(a => a != null).ToList();
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

        var me = board.Players.Find(p => p.EntityId == player.EntityId);
        var opp = board.Players.Find(p => p.EntityId != player.EntityId);

        var myLifeTotal = me.Health;
        var oppLifeTotal = opp.Health;

        var score = 0;

        if (myLifeTotal <=0 && oppLifeTotal <= 0)
        {
            return 55555;
        }

        if (myLifeTotal <= 0)
        {
            return -999999;
        }
        if (oppLifeTotal <= 0)
        {
            return 999999;
        }

        //We need to make life total more valuable the more it goes down (for ourselves)
        score += Convert.ToInt32(Math.Log(myLifeTotal))* 20;
        score -= Convert.ToInt32(Math.Log(oppLifeTotal)) * 20;

        //Power of units in play
        if (me.GetUnitsInPlay().Any())
        {
            var totalPower = me.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score += totalPower * 50;
        }

        //Toughness of units in play
        if (me.GetUnitsInPlay().Any())
        {
            var totalToughness = me.GetUnitsInPlay().Select(c => c.Toughness).Aggregate((p1, p2) => p1 + p2);
            score += totalToughness * 30;
        }

        //Power of opponent units in play
        if (opp.GetUnitsInPlay().Any())
        {
            var totalPowerOpp = opp.GetUnitsInPlay().Select(c => c.Power).Aggregate((p1, p2) => p1 + p2);
            score -= totalPowerOpp * 50;
        }
        //Toughness of opponent units in play
        if (opp.GetUnitsInPlay().Any())
        {
            var totalToughnessOpp= opp.GetUnitsInPlay().Select(c => c.Toughness).Aggregate((p1, p2) => p1 + p2);
            score -= totalToughnessOpp * 30;
        }

        //Total number of resources available.
        var totalNumberOfResources = me.GetCardsInPlay().Count + me.Hand.Count();
        score += totalNumberOfResources * 40;

        var totalNumberOfOppResources = opp.GetCardsInPlay().Count + opp.Hand.Count();
        score -= totalNumberOfOppResources * 40;

        //Permanent Mana should be considered a resource, but be sure not to count temporary mana.
        var totalMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 40;
        score += totalMana;

        var anyMana = me.ManaPool.TotalMana.TotalSumOfColoredMana * 40;
        score += anyMana;

        //Check if we have a lotus bloom suspended or in play.
        //Ideally we would just move this to a default action
        //i.e. check if lotus bloom is in our hand, and if so then activate it.
        if (me.Exile.Any(e => e.Name.ToLower() == "lotus bloom"))
        {
            score += 1000;
        }


        //Temporary Power is what we should calculate (ie haze of rage)

        //Other things to check

        //Favorable matchup in a lane.

        //If they are winning the game.

        return score;
    }
}

