using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A UI State that deals with cards with multiple possible actions when you select it.
/// </summary>
public class GameUIChooseCardActionState : IGameUIState, IGameUIStateHandleCastChoice
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithMultipleActions;

    public GameUIChooseCardActionState(GameUIStateMachine stateMachine, CardInstance cardWithMultipleActions)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _cardWithMultipleActions = cardWithMultipleActions;
    }

    public string GetMessage()
    {
        return $"Choose how you want to play {_cardWithMultipleActions.Name}";
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
        //Show the cast modes window.
        _stateMachine.GameController.ShowActionChoicePopup(_cardWithMultipleActions.GetAvailableActions());
    }

    public void OnDestroy()
    {
        _stateMachine.GameController.CloseActionChoicePopup();
    }

    public void HandleSelection(int entityId)
    {
        return;
    }

    public void HandleCastChoiceSelection(int castChoiceId)
    {
        HandleActionChoice(_cardWithMultipleActions.GetAvailableActions()[castChoiceId]);
    }

    private void HandleActionChoice(CardGameAction actionSelected)
    {
        switch (actionSelected)
        {
            case PlayUnitAction unitAction:
                {
                    _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, unitAction.SourceCard));
                    break;
                }
            case PlayManaAction playManaAction:
                {
                    if (!playManaAction.IsValidAction(_cardGame))
                    {
                        return;
                    }
                    _cardGame.ProcessAction(playManaAction);
                    _stateMachine.ToIdle();
                    break;
                }
            case PlaySpellAction spellAction:
                {
                    _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, spellAction.SourceCard));
                    break;
                }
                //TODO - still need to make it so cards might have more than 1 ability.
            case ActivateAbilityAction activateAbilityAction:
                {
                    _stateMachine.ChangeState(new GameUIActivatedAbilityState(_stateMachine, activateAbilityAction.SourceCard));
                    break;
                }
            default:
                {
                    Debug.Log($"Unknown action type {actionSelected.GetType()} in GameUIChooseCardActionState");
                    break;
                }
        }
    }
}


