using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUIActivatedAbilityState : GameUIActionState, IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithAbility;

    public GameUIActivatedAbilityState(GameUIStateMachine stateMachine, CardInstance cardWithAbility)
    {
        SelectedTargets = new List<CardGameEntity>();

        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _cardWithAbility = cardWithAbility;

        //Determine whether the ability has targets
        NeedsTargets = GetActivatedAbility().HasTargets();
        //Determine whether the ability needs cost choices
        NeedsCostChoices = GetActivatedAbility().HasAdditionalCostChoices();
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
        return _cardWithAbility.GetAbilitiesAndComponents<ActivatedAbility>().First();
    }

    public override void OnDestroy()
    {
        _internalState?.OnDestroy();
    }

    public override void HandleSelection(int entityId)
    {
        _internalState?.HandleSelection(entityId);
    }


    public override void ChangeToCostChoosingState()
    {
        ChangeState(new GameUIChooseCostsState(_stateMachine, this, _cardWithAbility, GetActivatedAbility().AdditionalCost));
    }

    public override void ChangeToSelectTargetState()
    {
        ChangeState(new GameUISelectTargetState(_stateMachine, this, GetActivatedAbility().Effects));
    }

    public void ActivateAbility()
    {
        _cardGame.ActivatedAbilitySystem.ActivateAbililty(_actingPlayer, _cardWithAbility, new ActivateAbilityInfo
        {
            Targets = SelectedTargets,
            Choices = SelectedChoices
        });

        _stateMachine.ToIdle();
    }

    public override void DoAction()
    {
        ActivateAbility();
    }
}
