using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUICastingSpellState : IGameUIState
{

    private CardGame _cardGame;
    private Player _actingPlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _spellToCast;

    public GameUICastingSpellState(GameUIStateMachine stateMachine, Player actingPlayer, CardInstance spellToCast)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _actingPlayer = actingPlayer;
        _spellToCast = spellToCast;
    }

    private List<KeyCode> inputKeys = new List<KeyCode>
    {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Q,
            KeyCode.W
    };

    public Dictionary<KeyCode, CardGameEntity> KeyCodeToEntityMap()
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _actingPlayer, _spellToCast);
        var keysToValidTargets = new Dictionary<KeyCode, CardGameEntity>();

        if (inputKeys.Count < validTargets.Count)
        {
            throw new Exception("Not enough keys for the amount of targets");
        }

        for (var i = 0; i < validTargets.Count; i++)
        {
            keysToValidTargets.Add(inputKeys[i], validTargets[i]);
        }

        return keysToValidTargets;
    }

    public string GetMessage()
    {
        var targetsForMessage = KeyCodeToEntityMap().ToList().Select(keyValuePair =>
        {
            return $@"{Environment.NewLine} {keyValuePair.Key.ToString()} : {keyValuePair.Value.Name} ";
        }).ToList();

        return $@"Choose a target for spell {_spellToCast.Name}  to(use numeric keys) {String.Join(",", targetsForMessage)} ";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle(_actingPlayer);
            return;
        }

        var keyValuePair = KeyCodeToEntityMap().ToList();

        for (var i = 0; i < keyValuePair.Count; i++)
        {
            var keyCode = keyValuePair[i].Key;
            if (Input.GetKeyDown(keyCode))
            {
                _cardGame.PlayCardFromHand(_actingPlayer, _spellToCast, keyValuePair[i].Value.EntityId);
                _stateMachine.ToIdle(_actingPlayer);
            }
        }
    }

    public void OnApply()
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _actingPlayer, _spellToCast).Select(e => e.EntityId);

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

