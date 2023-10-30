using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Targeting State for Effects
internal class GameUISelectTargetState : IGameUIState
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private List<Effect> _effects;
    private GameUIActionState _parentState;

    public GameUISelectTargetState(GameUIStateMachine stateMachine, GameUIActionState parentState, List<Effect> effects)
    {
        _stateMachine = stateMachine;
        _effects = effects;
        _parentState = parentState;
    }

    public string GetMessage()
    {
        return $@"Please choose a target for {GetEffectWithTarget().RulesText}";
    }

    //Assumes we only have spells with one target, will need to change this for when we have spells with more than one target.
    public Effect GetEffectWithTarget()
    {
        return _effects.Find(e => e.NeedsTargets());
    }

    public void HandleInput()
    {
        throw new NotImplementedException(); //we should never get here, our handle input is handled by our parent state.
    }

    public void HandleSelection(int entityId)
    {
        var validTargets = _cardGame.TargetSystem.GetValidEffectTargets(_actingPlayer, _effects); ;

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
            _parentState.DoAction();
        }
    }

    public void OnApply()
    {
        var validTargets = _cardGame.TargetSystem.GetValidEffectTargets(
        _actingPlayer,
        _effects)
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
        _stateMachine.GameController.SetStateLabel(GetMessage());
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

