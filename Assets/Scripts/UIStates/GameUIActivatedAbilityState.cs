using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUIActivatedAbilityState : IGameUIState
{

    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithAbility;

    public GameUIActivatedAbilityState(GameUIStateMachine stateMachine, CardInstance cardWithAbility)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _cardWithAbility = cardWithAbility;
    }
    public string GetMessage()
    {
        return $@"Choose a target for {_cardWithAbility.Name}'s ability";
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
        var validTargets = _cardGame.TargetSystem.GetValidAbilityTargets(
            _cardGame,
            _actingPlayer,
            _cardWithAbility)
            .Select(e => e.EntityId);

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
        var validTargets = _cardGame.TargetSystem.GetValidAbilityTargets(_cardGame, _actingPlayer, _cardWithAbility); ;

        if (!validTargets.Select(e => e.EntityId).Contains(entityId))
        {
            return;
        }

        var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == entityId);

        if (targetAsEntity == null)
        {
            return;
        }
        //We still need a card here?.
        _cardGame.ActivatedAbilitySystem.ActivateAbilityWithTargets(_cardGame, _actingPlayer, _cardWithAbility, new List<CardGameEntity> { targetAsEntity });
        _stateMachine.ToIdle();
    }
}

