﻿using System;
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
        NeedsTargets = _cardGame.TargetSystem.SpellNeedsTargets(_actingPlayer, _spellToCast);
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

    public override void HandleSelection(int entityId)
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _spellToCast).Select(e => e.EntityId);

        if (!validTargets.Contains(entityId))
        {
            return;
        }

        _cardGame.PlayCard(_actingPlayer, _spellToCast, entityId);
        _stateMachine.ToIdle();
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
        throw new NotImplementedException();
    }

    public override void DoAction()
    {
        //We need to change this to account for any additional costs and targets.
        int target;

        if (SelectedTargets != null && SelectedTargets.Any())
        {
            target = SelectedTargets[0].EntityId;
        }
        else
        {
            target = 0;
        }

        _cardGame.PlayCard(_actingPlayer, _spellToCast, target);
        _stateMachine.ToIdle();
    }
}


