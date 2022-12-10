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

        return (myLifeTotal * 15) + (oppLifeTotal * 20);
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
