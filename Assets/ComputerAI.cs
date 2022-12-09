using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

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
        var gameController = UIGameController.Instance;
        var gameService = UIGameController.Instance.GameService;
        var cardGame = gameController.CardGame;

        if (cardGame.ActivePlayer.PlayerId == playerId)
        {

            if (cardGame.CurrentGameState == GameState.WaitingForAction)
            {
                ChooseAction(cardGame);
            }
            else if (cardGame.CurrentGameState == GameState.WaitingForChoice)
            {
                var choiceInfo = cardGame.ChoiceInfoNeeded;
                var validChoices = choiceInfo.GetValidChoices(cardGame, cardGame.ActivePlayer);

                //I think careful study should also be in here as well.
                //If its a single choice vs a multi choice.
                if (choiceInfo.NumberOfChoices > 1)
                {
                    var choices = validChoices.Randomize().Take(choiceInfo.NumberOfChoices);
                    gameService.MakeChoice(choices.ToList());
                }
                else if (choiceInfo.NumberOfChoices == 1)
                {
                    var choice = validChoices.Randomize().ToList()[0];
                    gameService.MakeChoice(new List<CardInstance> { choice });
                }
            }

            //Choose an action here.
            //var availableActions = gameController.CardGame.Get
        }
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
        var validActions = availableActions.Where(a => a.IsValidAction(cardGame));

        var newDebugMsg = $"Number of valid actions {validActions.Count()}";

        if (newDebugMsg != previousDebugMessage)
        {
            Debug.Log(newDebugMsg);
            previousDebugMessage = newDebugMsg;
        }

        if (validActions.Count()  == 0)
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


        if (validActions.Any())
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
