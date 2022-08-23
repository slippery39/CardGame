using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUICastingSpellState : GameUIActionState, IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _spellToCast;
    public GameUICastingSpellState(GameUIStateMachine stateMachine, CardInstance spellToCast)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _spellToCast = spellToCast;

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
        }
        else if (NeedsCostChoices)
        {
            ChangeToCostChoosingState();
        }
        else
        {
            DoAction();
        }
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
        var effectsWithTargets = _cardGame.TargetSystem.GetEffectsThatNeedTargets(_spellToCast.Effects);
        if (effectsWithTargets.Any())
        {
            ChangeState(new GameUISelectTargetState(_stateMachine, this, effectsWithTargets));
        }
        else
        {
            _cardGame.Log("An error occured... expecting effects with targets for GameUICastingSpellState::ChangeToSelectTargetState()");
        }
    }

    public override void ChangeToCostChoosingState()
    {
        ChangeState(new GameUIChooseCostsState(_stateMachine, this, _spellToCast,_spellToCast.AdditionalCost));
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
            Spell = _spellToCast,
            AdditionalChoices = SelectedChoices
        };

        if (!spellAction.IsValidAction(_cardGame))
        {
            Debug.Log($"Could not play {_spellToCast.Name}");
            return;
        }

        _cardGame.ProcessAction(spellAction);
        _stateMachine.ToIdle();
    }
}


