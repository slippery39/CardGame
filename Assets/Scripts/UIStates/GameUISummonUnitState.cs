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

    private List<KeyCode> _inputKeys = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

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

    private Dictionary<KeyCode, Lane> GetKeyCodeToLaneMap()
    {

        var map = new Dictionary<KeyCode, Lane>();

        for (var i = 0; i < _inputKeys.Count; i++)
        {
            map.Add(_inputKeys[i], _actingPlayer.Lanes[i]);
        }

        return map;

    }

    public string GetMessage()
    {
        return $@"Choose a lane to summon {_unitToSummon.Name} to(use numeric keys)";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle(_actingPlayer);
            return;
        }

        var keyCodeToLaneMap = GetKeyCodeToLaneMap();

        foreach (var key in keyCodeToLaneMap.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                var lane = keyCodeToLaneMap[key];
                //Try to summon the unit.
                _cardGame.PlayCardFromHand(_cardGame.Player1, _unitToSummon, lane.EntityId);
                _stateMachine.ToIdle(_actingPlayer);
            }
        }
    }

    public void OnDestroy()
    {
        foreach (UILane uiLane in _stateMachine.GameController.GetUILanes())
        {
            uiLane.StopHighlight();
        }
    }
}

