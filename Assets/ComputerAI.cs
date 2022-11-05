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

        if (gameController.CardGame.ActivePlayer.PlayerId == playerId)
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

            //Choose an action here.
            //var availableActions = gameController.CardGame.Get
        }
        //Poll the game to check if its our turn to act

    }
}
