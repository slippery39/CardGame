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
        _stateMachine.HandleAction(_cardWithMultipleActions.GetAvailableActions()[castChoiceId]);
    }


}


