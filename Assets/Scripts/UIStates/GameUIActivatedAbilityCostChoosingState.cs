using System;
using System.Collections.Generic;
using System.Linq;

public class GameUIActivatedAbilityCostChoosingState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithAbility;

    private GameUIActivatedAbilityState _parentState;

    public GameUIActivatedAbilityCostChoosingState(GameUIStateMachine stateMachine, GameUIActivatedAbilityState parentState, CardInstance cardWithAbility)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _cardWithAbility = cardWithAbility;
        _parentState = parentState;
    }

    public string GetMessage()
    {
        return $@"Choose your cost for {_cardWithAbility.Name}'s ability";
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
        var activatedAbility = GetActivatedAbility();
        var validChoices = activatedAbility.AdditionalCost.GetValidChoices(_cardGame, _actingPlayer, _cardWithAbility);

        if (!validChoices.Select(e => e.EntityId).Contains(entityId))
        {
            return;
        }

        var choiceAsEntity = validChoices.FirstOrDefault(tar => tar.EntityId == entityId);

        if (choiceAsEntity == null)
        {
            return;
        }

        //save the targets in the parent state
        _parentState.SelectedChoices = new List<CardGameEntity> { choiceAsEntity };

        //Choosing costs is the last step, so we should always activate the ability. Note that if one day we have multiple costs, or need to select multiple 
        //we will need to update this to handle that.
        _parentState.ActivateAbility();
    }

    public void OnApply()
    {
        var activatedAbility = GetActivatedAbility();
        var validChoices = activatedAbility.AdditionalCost.GetValidChoices(_cardGame, _actingPlayer, _cardWithAbility);

        if (validChoices.Count() == 0)
        {
            return;
        }

        var uiEntities = _stateMachine.GameController.GetUIEntities();
        var choicesAsInts = validChoices.Select(c => c.EntityId).ToList();
        //Highlight all entities that share an entity id with the valid choices;   

        var entitiesToHighlight = uiEntities.Where(e => choicesAsInts.Contains(e.EntityId));

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
