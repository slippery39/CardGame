using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUICastingSpellState : GameUIActionState, IGameUIState, IGameUICancellable
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private GameService _gameService => _stateMachine.GameService;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _spellToCast;
    private List<ICastModifier> _modifiers;
    public GameUICastingSpellState(GameUIStateMachine stateMachine, CardInstance spellToCast, List<ICastModifier> modifiers = null)
    {
        _stateMachine = stateMachine;
        _spellToCast = spellToCast;

        if (modifiers != null)
        {
            _modifiers = modifiers;
        }
        else
        {
            _modifiers = new List<ICastModifier>();
        }

        //Determine whether the ability has targets
        NeedsTargets = _cardGame.TargetSystem.CardNeedsTargets(_actingPlayer, _spellToCast);
        //Determine whether the ability needs cost choices

        if (_spellToCast.AdditionalCost == null)
        {
            NeedsCostChoices = false;
        }
        else
        {
            NeedsCostChoices = _spellToCast.AdditionalCost.NeedsChoice;
        }
    }
    public override string GetMessage()
    {
        return _internalState?.GetMessage();
    }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle();
            return;
        }
    }

    public override void OnApply()
    {
        if (NeedsTargets)
        {
            ChangeToSelectTargetState();
            OnApplyCancellable();
           
        }
        else if (NeedsCostChoices)
        {
            ChangeToCostChoosingState();
            OnApplyCancellable();
        }
        else
        {
            DoAction();
        }
        _stateMachine.GameController.SetStateLabel(GetMessage());
    }

    public void OnApplyCancellable()
    {
        this._stateMachine.GameController.ShowCancelButton();
    }

    public void HandleCancel()
    {
        this._stateMachine.ToIdle();
    }

    public override void OnDestroy()
    {
        var uiEntities = _stateMachine.GameController.GetUIEntities();
        foreach (var entity in uiEntities)
        {
            entity.StopHighlight();
        }
    }

    public override void ChangeToSelectTargetState()
    {

        var effectsWithTargets = _spellToCast.Effects.Where(e => e.NeedsTargets()).ToList();
        if (effectsWithTargets.Any())
        {
            ChangeState(new GameUISelectTargetState(_stateMachine, this, _spellToCast, effectsWithTargets));
        }
        else
        {
            _cardGame.Log("An error occured... expecting effects with targets for GameUICastingSpellState::ChangeToSelectTargetState()");
        }
    }

    public override void ChangeToCostChoosingState()
    {
        ChangeState(new GameUIChooseCostsState(_stateMachine, this, _spellToCast, _spellToCast.AdditionalCost));
    }

    public override void HandleSelection(int entityId)
    {
        _internalState?.HandleSelection(entityId);
    }

    public override void DoAction()
    {
        var spellAction = new PlaySpellAction
        {
            Player = _actingPlayer,
            Targets = SelectedTargets,
            CardToPlay = _spellToCast,
            AdditionalChoices = SelectedChoices,
            CastModifiers = _modifiers
        };

        if (!spellAction.IsValidAction(_cardGame))
        {
            Debug.Log($"Could not play {_spellToCast.Name}");
            return;
        }

        _gameService.ProcessAction(spellAction);
        _stateMachine.ToIdle();
    }


}


