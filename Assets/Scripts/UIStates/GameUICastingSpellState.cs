using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUICastingSpellState : IGameUIState
{

    private CardGame _cardGame;
    private Player _actingPlayer=>_cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _spellToCast;

    public GameUICastingSpellState(GameUIStateMachine stateMachine,CardInstance spellToCast)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _spellToCast = spellToCast;
    }
    public string GetMessage()
    {
        return $@"Choose a target for spell {_spellToCast.Name}";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle();
            return;
        }
    }

    public void OnApply()
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _spellToCast).Select(e => e.EntityId);

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

    public void HandleSelection(int entityId)
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_actingPlayer, _spellToCast).Select(e => e.EntityId);

        if (!validTargets.Contains(entityId))
        {
            return;
        }

        _cardGame.PlayCardFromHand(_actingPlayer, _spellToCast, entityId);
        _stateMachine.ToIdle();
    }
}

