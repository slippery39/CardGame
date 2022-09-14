using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUIIdleState : IGameUIState
{

    private CardGame _cardGame;
    private Player ActingPlayer { get => _cardGame.ActivePlayer; }
    private GameUIStateMachine _stateMachine;

    public GameUIIdleState(GameUIStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
    }

    public string GetMessage()
    {
        return "Play a card or activate an ability";
    }

    //Grabbed from ChooseCardActionState, should probably put this in the base class?



    //TODO - I don't think we need to differentiate between cards in hand and cards not in hand anymore, they should all work the same
    //now, theo only thing that matters is that they have actions available for them.
    private void HandleCardSelectedFromHand(CardInstance card)
    {
        if (card.HasMultipleOptions())
        {
            _stateMachine.ChangeState(new GameUIChooseCardActionState(_stateMachine, card));
            return;
        }
        if (card.GetAvailableActions().IsNullOrEmpty())
        {
            throw new System.Exception("Something weird happening in HandleCardSelectedFromHand(). Should never see this message");
        }

        var action = card.GetAvailableActions().First();
        _stateMachine.HandleAction(action);
    }

    private void HandleCardActivatedAbility(CardInstance card)
    {
        var activatedAbility = card.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility == null)
        {
            _cardGame.Log("The card does not have an activated ability");
        }

        //TODO, this should also be handled by the game logic as well... it currently is not.
        var canActivateAbility = _cardGame.ActivatedAbilitySystem.CanActivateAbility(ActingPlayer, card);

        if (canActivateAbility)
        {
            this._stateMachine.ChangeState(new GameUIActivatedAbilityState(_stateMachine, card));
        }
        else
        {
            _cardGame.Log("You cannot pay the mana or costs to activate that ability");
        }
    }

    public void HandleInput()
    {
    }

    public void OnUpdate()
    {

        //Change to grab all available actions, distinct it by the entity id associated with the action , then highlight those entityIds.

        //Need to highlight all castable cards.      
        foreach (var uiEntity in _stateMachine.GameController.GetUIEntities())
        {

            var card = _cardGame.GetCardById(uiEntity.EntityId);
            if (card == null)
            {
                continue;
            }
            
            if (card.GetAvailableActions().Any())
            {
                uiEntity.Highlight();
            }
            else
            {
                uiEntity.StopHighlight();
            }
        };
    }

    public void OnApply()
    {
    }
    public void OnDestroy()
    {
        //Need to highlight all castable cards.
        foreach (var uiEntity in _stateMachine.GameController.GetUIEntities())
        {
            uiEntity.StopHighlight();
        };
    }

    public void HandleSelection(int entityId)
    {
        var card = _cardGame.GetCardById(entityId);

        if (card == null)
        {
            _cardGame.Log($"Could not find card with entity id {entityId}");
            return;
        }


        if (card.HasMultipleOptions())
        {
            _stateMachine.ChangeState(new GameUIChooseCardActionState(_stateMachine, card));
            return;
        }

        //TODO - We can automatically move it to the proper state based on the type of action that is associated with it (since at this point there would only be one action).
        var isCardInPlay = _cardGame.IsInPlay(card);
        if (isCardInPlay)
        {
            HandleCardActivatedAbility(card);
            return;
        }

        if (card.GetAvailableActions().IsNullOrEmpty())
        {
            return;
        }

        HandleCardSelectedFromHand(card);
    }
}

