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

    private void HandleCardSelectedFromHand(CardInstance card)
    {
        if (card.HasMultipleOptions())
        {
            _stateMachine.ChangeState(new GameUIChooseCardActionState(_stateMachine, card));
        }
        else if (card.CurrentCardData is UnitCardData)
        {
            _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, card));
        }
        else if (card.CurrentCardData is ManaCardData)
        {
            var playManaAction = new PlayManaAction
            {
                Player = ActingPlayer,
                Card = card
            };

            if (!playManaAction.IsValidAction(_cardGame))
            {
                return;
            }
            _cardGame.ProcessAction(playManaAction);
        }
        else if (card.CurrentCardData is SpellCardData || card.CurrentCardData is ItemCardData)
        {
            _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, card));
        }
        else
        {
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

        
        if (card.HasMultipleOptions())
        {

        }

        //Temporary Code to Handle Lotus Bloom for trsting purposes. Remove before pushing.
        //TODO - Lotus Bloom - be able to handle cards with more than one activated ability.
        //For now we will assume the activated abilities are in different zones.
        if (card.Name == "Lotus Bloom")
        {
            Debug.Log("Test : Handling Lotus Bloom");
            if (_cardGame.GetZoneOfCard(card).ZoneType == ZoneType.Hand)
            {
                Debug.Log("Activating Ability for Lotus Bloom");
                _cardGame.ActivatedAbilitySystem.ActivateAbililty(ActingPlayer, card, new ActivateAbilityInfo
                {

                });
            }
        }
        //TOOD Lotus Bloom - handling activated abilities for cards not in play.
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

