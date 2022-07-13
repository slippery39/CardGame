using System;
using System.Collections.Generic;
using System.Linq;


//TODO - This needs to be changed to a more generic choose AdditionalCostState to be handle to handle additional costs on creatures and spells.
/*
 * We should be able to do this by passing in the additional cost instead of the entire card, and by changing the _parentState to 
 * use some sort of interface like "Apply()" or "OnFinish()" instead of forcing it to be a GameUIActivatedAbilityState.
 */
public class GameUIChooseCostsState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private AdditionalCost _additionalCost;
    private CardGameEntity _sourceEntity;

    private GameUIActionState _parentState;

    public GameUIChooseCostsState(GameUIStateMachine stateMachine, GameUIActionState parentState, CardGameEntity sourceEntity, AdditionalCost additionalCost)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _additionalCost = additionalCost;
        _parentState = parentState;
        _sourceEntity = sourceEntity;
    }

    public string GetMessage()
    {
        return $@"Choose your cost for {_additionalCost.RulesText}";
    }

    public void HandleInput()
    {
        throw new NotImplementedException(); //we should never get here, our handle input is handled by our parent state.
    }

    public void HandleSelection(int entityId)
    {
        var validChoices = _additionalCost.GetValidChoices(_cardGame, _actingPlayer,_sourceEntity);

        if (!validChoices.Select(e => e.EntityId).Contains(entityId))
        {
            return;
        }

        var choiceAsEntity = validChoices.FirstOrDefault(tar => tar.EntityId == entityId);

        if (choiceAsEntity == null)
        {
            return;
        }

        //save the choices in the parent state
        _parentState.SelectedChoices = new List<CardGameEntity> { choiceAsEntity };

        //Choosing costs is the last step at the moment, so we should always do the action;
        _parentState.DoAction();
    }

    public void OnApply()
    {
        var validChoices = _additionalCost.GetValidChoices(_cardGame, _actingPlayer, _sourceEntity);

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
