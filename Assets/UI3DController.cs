using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI3DController : MonoBehaviour
{

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var card in FindObjectsOfType<Card3D>())
            {
                var randomCard = new CardDatabase().GetAll().Randomize().First();
                card.SetCardInfo(randomCard);
            }
        }
    }
    public void HandleEvent(object evt)
    {
        //Run Appropriate Animations

        //Set the state of the game UI


        //SetGameState(evt.ResultingGameState);     
    }


    public void SetGameState(CardGame gameState)
    {
        //for each entity in the game State, go through and update the entity in the UI that corresponds to it.
        //cards that are not revealed (i.e. cards in your deck that are face down) or cards in the opponent hand should not correspond to a specific entity yet
        //we will use an entity id of -1 for this case.

        /*
         * 
         * foreach player ->
         * 
         * foreach(var card in hand){
         *  update the cards in the players hand
         * }
         * 
         * foreach(var card in graveyard){
         *  update the card in the players graveyard
         * }
         * 
         * foreach(var card in lane){
         *  update the cards in play
         * }
         * 
         * foreach (var card in items){
         *  update the cards in items
         * }
         * 
         * also deal with revealed cards
         * 
         */
    }
}
