using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUIIdleState : IGameUIState
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private readonly GameUIStateMachine _stateMachine;

    public GameUIIdleState(GameUIStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public string GetMessage()
    {
        return "Play a card or activate an ability";
    }
    private void HandleCardSelected(CardInstance card)
    {
        if (card.HasMultipleOptions())
        {
            _stateMachine.ChangeState(new GameUIChooseCardActionState(_stateMachine, card));
            return;
        }

        if (card.GetAvailableActions().IsNullOrEmpty())
        {
            throw new System.Exception("Something weird happening in HandleCardSelected(). Should never see this message");
        }

        var action = card.GetAvailableActions().First();
        _stateMachine.HandleAction(action);
    }

    public void HandleInput()
    {
    }

    public void OnUpdate()
    {
        foreach (var uiEntity in _stateMachine.GameController.GetUIEntities())
        {
            var card = _stateMachine.GameController.CurrentUICardGame.GetCardById(uiEntity.EntityId);
            
            if (card == null)
            {
                continue;
            }

            //Only allow actions to be highlighted if they are of the current player. Computer opponents should not show highlights.
            if (card.OwnerId != _stateMachine.PlayerId)
            {
                uiEntity.StopHighlight();
                continue;
            }
            
            if (card.GetAvailableActions().Any())
            {
                uiEntity.Highlight();
            }
            else
            {
                uiEntity.StopHighlight();
            }
        };
    }

    public void OnApply()
    {
        _stateMachine.GameController.ShowEndTurnButton();
        _stateMachine.GameController.SetStateLabel("");
    }
    public void OnDestroy()
    {
        //Need to highlight all castable cards.
        foreach (var uiEntity in _stateMachine.GameController.GetUIEntities())
        {
            uiEntity.StopHighlight();
        };
    }

    public void HandleSelection(int entityId)
    {
        var card = _cardGame.GetCardById(entityId);


        if (card == null)
        {
            _cardGame.Log($"Could not find card with entity id {entityId}");
            return;
        }

        if (card.OwnerId != _stateMachine.PlayerId)
        {
            return; //should not do anything in this case.
        }

        if (card.HasMultipleOptions())
        {
            _stateMachine.ChangeState(new GameUIChooseCardActionState(_stateMachine, card));
            return;
        }

        if (card.GetAvailableActions().IsNullOrEmpty())
        {
            _cardGame.Log("No available actions for that card");
            return;
        }

        HandleCardSelected(card);
    }
}

