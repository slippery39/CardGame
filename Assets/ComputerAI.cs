using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    }

    // Update is called once per frame
    void Update()
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
                var choices = choiceInfo.GetValidChoices(cardGame,cardGame.ActivePlayer);


                //I think careful study should also be in here as well.
                //If its a single choice vs a multi choice.
                if (choiceInfo is IMultiChoiceEffect)
                {
                    Debug.Log("TODO - Multi Choice Effect (Telling time at the moment)");
                    
                }
                else if (choiceInfo is IEffectWithChoice)
                {
                    var choice = choices.Randomize().ToList()[0];
                    cardGame.MakeChoice(new List<CardInstance> { choice });
                }               
            }

            //Choose an action here.
            //var availableActions = gameController.CardGame.Get
        }
        //Poll the game to check if its our turn to act

    }

    private void ChooseAction(CardGame cardGame)
    {
        var availableActions = cardGame.ActivePlayer.GetAvailableActions();

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
            cardGame.ProcessAction(actionToChoose);
        }
    }
}
