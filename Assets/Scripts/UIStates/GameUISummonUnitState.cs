using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUISummonUnitState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _unitToSummon;


    public GameUISummonUnitState(GameUIStateMachine stateMachine, CardInstance unitToSummon)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _unitToSummon = unitToSummon;
    }


    public void OnApply()
    {
        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _unitToSummon).Select(ent => ent.EntityId);
        foreach (UIGameEntity entity in _stateMachine.GameController.GetUIEntities())
        {
            if (validLaneTargets.Contains(entity.EntityId))
            {
                entity.Highlight();
            }
        }
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
        foreach (UILane uiLane in _stateMachine.GameController.GetUILanes())
        {
            uiLane.StopHighlight();
        }
    }

    public void HandleSelection(int entityId)
    {
        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _unitToSummon).Select(ent => ent.EntityId);

        if (!validLaneTargets.Contains(entityId))
        {
            return;
        }
        _cardGame.PlayCardFromHand(_cardGame.ActivePlayer, _unitToSummon, entityId);
        _stateMachine.ToIdle();
    }
}

