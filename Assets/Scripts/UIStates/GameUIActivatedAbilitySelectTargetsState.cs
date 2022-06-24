using System;
using System.Collections.Generic;
using System.Linq;

internal class GameUIActivatedAbilitySelectTargetsState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithAbility;

    private GameUIActivatedAbilityState _parentState;

    public GameUIActivatedAbilitySelectTargetsState(GameUIStateMachine stateMachine, GameUIActivatedAbilityState parentState, CardInstance cardWithAbility)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _cardWithAbility = cardWithAbility;
        _parentState = parentState;
    }

    public string GetMessage()
    {
        return $@"Choose a target for {_cardWithAbility.Name}'s ability";
    }

    public void HandleInput()
    {
        throw new NotImplementedException(); //we should never get here, our handle input is handled by our parent state.
    }

    private ActivatedAbility GetActivatedAbility()
    {
        return _cardWithAbility.GetAbilities<ActivatedAbility>().First();
    }

    public void HandleSelection(int entityId)
    {
        var validTargets = _cardGame.TargetSystem.GetValidAbilityTargets(_actingPlayer, _cardWithAbility); ;

        if (!validTargets.Select(e => e.EntityId).Contains(entityId))
        {
            return;
        }

        var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == entityId);

        if (targetAsEntity == null)
        {
            return;
        }

        //save the targets in the parent state
        _parentState.SelectedTargets = new List<CardGameEntity> { targetAsEntity };

        //Logic Needed
        // If we still have a cost choice to make, then move to the GameUIActivatedAbilityChooseCostState
        if (_parentState.NeedsCostChoices)
        {
            //move to the cost choosing state
            _parentState.ChangeToCostChoosingState();
        }
        else
        {
            //We should have everything we need, lets go activate the ability.
            _parentState.ActivateAbility();
        }
    }

    public void OnApply()
    {
        var validTargets = _cardGame.TargetSystem.GetValidAbilityTargets(
        _actingPlayer,
        _cardWithAbility)
        .Select(e => e.EntityId);

        if (validTargets.Count() == 0)
        {
            return;
        }

        var uiEntities = _stateMachine.GameController.GetUIEntities();
        //Highlight all entities that share an entity id with the valid targets of the spell        

        var entitiesToHighlight = uiEntities.Where(e => validTargets.Contains(e.EntityId));

        foreach (var entity in entitiesToHighlight)
        {
            entity.Highlight();
        }
    }

    public void OnDestroy()
    {
        var uiEntities = _stateMachine.GameController.GetUIEntities();
        foreach (var entity in uiEntities)
        {
            entity.StopHighlight();
        }
    }
}
