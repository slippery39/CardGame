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
        return "Play a card from your hand (using numeric keys) or press B to fight";
    }

    private void HandleCardSelectedFromHand(CardInstance card)
    {
        //Figure out what type of card it is, and move to the appropriate state.
        if (card.Name == "Gempalm Incinerator")
        {
            var test = 0;
        }

        if (card.HasMultipleCastModes())
        {
            _stateMachine.ChangeState(new GameUIChooseCastModeState(_stateMachine, card));
        }
        else if (card.CurrentCardData is UnitCardData)
        {
            _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, card));
        }
        else if (card.CurrentCardData is ManaCardData)
        {
            _cardGame.ManaSystem.PlayManaCard(ActingPlayer, card);
        }
        else if (card.CurrentCardData is SpellCardData || card.CurrentCardData is ItemCardData)
        {

            _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, card));
        }
        else
        {
            //TODO - handle items... 
            //handle items now... items might have targets?
            //Really we should just have a PlayingCardState... 
        }
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

        //Need to highlight all castable cards.
        foreach (var uiEntity in _stateMachine.GameController.GetUIEntities())
        {
            if (_cardGame.CanPlayCard(uiEntity.EntityId))
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

        var isCardInPlay = _cardGame.IsInPlay(card);

        if (isCardInPlay)
        {
            HandleCardActivatedAbility(card);
            return;
        }

        var isCardCastable = _cardGame.CanPlayCard(entityId); //should probably be part of a system.

        if (!isCardCastable)
        {
            return;
        }

        HandleCardSelectedFromHand(card);
    }
}

