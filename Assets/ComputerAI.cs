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
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe((_) => TryChooseAction());
    }

    private void TryChooseAction()
    {
        var gameController = UIGameController.Instance;
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
                    cardGame.MakeChoice(choices.ToList());
                }
                else if (choiceInfo.NumberOfChoices == 1)
                {
                    var choice = validChoices.Randomize().ToList()[0];
                    cardGame.MakeChoice(new List<CardInstance> { choice });
                }
            }

            //Choose an action here.
            //var availableActions = gameController.CardGame.Get
        }
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

        if (validActions.Any())
        {
            var actionToChoose = validActions.Randomize().First();
            gameService.ProcessAction(actionToChoose);
        }
    }
}
