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
        if (card.CurrentCardData is UnitCardData)
        {
            _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, card));
        }
        else if (card.CurrentCardData is ManaCardData)
        {
            if (_cardGame.ManaSystem.CanPlayManaCard(ActingPlayer, card))
            {
                _cardGame.ManaSystem.PlayManaCard(ActingPlayer, card);
            }
        }
        else if (card.CurrentCardData is SpellCardData)
        {
            //TODO - need to update this.
            if (_cardGame.TargetSystem.SpellNeedsTargets(ActingPlayer, card))
            {
                _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, card));
            }
            else
            {
                _cardGame.PlayCardFromHand(ActingPlayer, card, 0);
            }
        }
    }

    private void HandleCardActivatedAbility(CardInstance card)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility == null)
        {
            _cardGame.Log("The card does not have an activated ability");
        }

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

    public void OnApply()
    {
    }
    public void OnDestroy()
    {
    }

    public void HandleSelection(int entityId)
    {
        //check if its a card in play

        var cardInPlay = ActingPlayer.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Where(card => card.EntityId == entityId).FirstOrDefault();

        if (cardInPlay != null)
        {
            HandleCardActivatedAbility(cardInPlay);
            return;
        }

        //we need to get the card from hand?
        var cardFromHand = ActingPlayer.Hand.Cards.Where(card => card.EntityId == entityId).FirstOrDefault();

        if (cardFromHand == null)
        {
            Debug.Log("Invalid Selection");
            return;
        }
        HandleCardSelectedFromHand(cardFromHand);
    }
}

