using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUISummonUnitState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _unitToSummon;


    public GameUISummonUnitState(GameUIStateMachine stateMachine, Player actingPlayer, CardInstance unitToSummon)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _actingPlayer = actingPlayer;
        _unitToSummon = unitToSummon;
    }


    public void OnApply()
    {

        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _actingPlayer, _unitToSummon).Select(ent => ent.EntityId);
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
            _stateMachine.ToIdle(_actingPlayer);
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
        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _actingPlayer, _unitToSummon).Select(ent => ent.EntityId);

        if (!validLaneTargets.Contains(entityId))
        {
            return;
        }
        _cardGame.PlayCardFromHand(_cardGame.Player1, _unitToSummon, entityId);
        _stateMachine.ToIdle(_actingPlayer);
    }
}

