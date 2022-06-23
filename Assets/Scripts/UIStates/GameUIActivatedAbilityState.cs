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

    //TODO - we will be using internal state to determine which messages to show and what not
    private IGameUIState _internalState;


    public bool NeedsTargets { get; set; } = false;
    public bool NeedsCostChoices { get; set; } = false;

    public bool AutomaticCostChoice { get; set; } = false;

    public List<CardGameEntity> SelectedTargets { get; set; }
    public List<CardGameEntity> SelectedChoices { get; set; }

    //How to pay additional costs?

    public GameUIActivatedAbilityState(GameUIStateMachine stateMachine, CardInstance cardWithAbility)
    {
        SelectedTargets = new List<CardGameEntity>();

        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _cardWithAbility = cardWithAbility;

        //Determine whether the ability has targets
        NeedsTargets = GetActivatedAbility().HasTargets();
        //Determine whether the ability needs cost choices
        NeedsCostChoices = GetActivatedAbility().HasChoices();
    }

    public string GetMessage()
    {
        return _internalState?.GetMessage();
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
        //TODO - I think the bug is because this is activating before the state machine above it gets to do anything.
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
            //no targets and no additional costs, just fire the ability as is
            ActivateAbility();
        }
        //Due to the logic of how abilities get activated, we may not have a state to apply.
        _internalState?.OnApply();
    }

    private ActivatedAbility GetActivatedAbility()
    {
        return _cardWithAbility.GetAbilities<ActivatedAbility>().First();
    }

    public void OnDestroy()
    {
        _internalState?.OnDestroy();
    }

    public void HandleSelection(int entityId)
    {
        _internalState?.HandleSelection(entityId);
    }

    public void ChangeState(IGameUIState stateTo)
    {
        _internalState?.OnDestroy();
        _internalState = stateTo;
        stateTo.OnApply();
    }

    public void ChangeToCostChoosingState()
    {
        ChangeState(new GameUIActivatedAbilityCostChoosingState(_stateMachine, this, _cardWithAbility));
    }

    public void ChangeToSelectTargetState()
    {
        ChangeState(new GameUIActivatedAbilitySelectTargetsState(_stateMachine, this, _cardWithAbility));
    }

    public void ActivateAbility()
    {
        //TODO - this needs to change.
        _cardGame.ActivatedAbilitySystem.ActivateAbililty(_cardGame, _actingPlayer, _cardWithAbility, new ActivateAbilityInfo
        {
            Targets = SelectedTargets,
            Choices = SelectedChoices
        });

        _stateMachine.ToIdle();
    }
}
