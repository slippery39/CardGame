using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUISummonUnitState : IGameUIState, IGameUICancellable
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private GameService _gameService => _stateMachine.GameService;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _unitToSummon;


    public GameUISummonUnitState(GameUIStateMachine stateMachine, CardInstance unitToSummon)
    {
        _stateMachine = stateMachine;
        _unitToSummon = unitToSummon;
    }


    public void OnApply()
    {
        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _unitToSummon).Select(ent => ent.EntityId);
        foreach (var entity in _stateMachine.GameController.GetUIEntities())
        {
            if (validLaneTargets.Contains(entity.EntityId))
            {
                entity.Highlight();
            }
        }
        OnApplyCancellable();
        _stateMachine.GameController.SetStateLabel(GetMessage());
    }

    public string GetMessage()
    {
        return $@"Choose a lane to summon {_unitToSummon.Name} to";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle();
            return;
        }
    }

    public void OnDestroy()
    {
        foreach (var entity in _stateMachine.GameController.GetUIEntities())
        {
            entity.StopHighlight();
        }
    }
    public void HandleSelection(int entityId)
    {
        var summonUnitAction = new PlayUnitAction
        {
            Player = _actingPlayer,
            SourceCard = _unitToSummon,
            CardToPlay = _unitToSummon,
            Lane = _cardGame.GetEntities<Lane>().Where(l => l.EntityId == entityId).FirstOrDefault()
        };

        Debug.Log($"Handling Selection for Entity:{entityId}");

        if (!summonUnitAction.IsValidAction(_cardGame))
        {
            Debug.Log("Invalid Action");
            return;
        }

        _gameService.ProcessAction(summonUnitAction);
        _stateMachine.ToIdle();
    }

    public void OnApplyCancellable()
    {
        this._stateMachine.GameController.ShowCancelButton();
    }

    public void HandleCancel()
    {
        this._stateMachine.ToIdle();
    }
}

