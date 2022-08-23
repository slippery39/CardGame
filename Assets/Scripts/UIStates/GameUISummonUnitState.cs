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
        foreach (UIGameEntity entity in _stateMachine.GameController.GetUIEntities())
        {
            entity.StopHighlight();
        }
    }
    public void HandleSelection(int entityId)
    {

        var summonUnitAction = new PlayUnitAction
        {
            Player = _actingPlayer,
            Card = _unitToSummon,
            Lane = _cardGame.GetEntities<Lane>().Where(l => l.EntityId == entityId).FirstOrDefault()
        };

        if (!summonUnitAction.IsValidAction(_cardGame))
        {
            return;
        }

        _cardGame.ProcessAction(summonUnitAction);
        _stateMachine.ToIdle();

        /*
        //Might want to move this into the action itself.
        var validLaneTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _unitToSummon).Select(ent => ent.EntityId);

        if (!validLaneTargets.Contains(entityId))
        {
            return;
        }

        var summonUnitAction = new PlayUnitAction
        {
            Card = _unitToSummon,
            Lane = _cardGame.GetEntities<Lane>().Where(l => l.EntityId == entityId).FirstOrDefault()
        };

        _cardGame.PlayCard(_cardGame.ActivePlayer, _unitToSummon, entityId, new List<CardGameEntity>());
        _stateMachine.ToIdle();
        */
    }
}

