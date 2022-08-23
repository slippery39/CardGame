using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameUIChooseCastModeState : IGameUIState, IGameUIStateHandleCastChoice
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _cardWithCastModes;

    public GameUIChooseCastModeState(GameUIStateMachine stateMachine, CardInstance cardWithCastModes)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _cardWithCastModes = cardWithCastModes;
    }

    public string GetMessage()
    {
        return $"Choose how you want to play {_cardWithCastModes.Name}";
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
        _stateMachine.GameController.ShowCastModePopup(_cardWithCastModes);
    }

    public void OnDestroy()
    {
        _stateMachine.GameController.CloseCastModePopup();
    }

    public void HandleSelection(int entityId)
    {
        return;
    }

    public void HandleCastChoiceSelection(int castChoiceId)
    {
        if (castChoiceId == 0)
        {
            HandleCastChoice(_cardWithCastModes);
            //we are playing the first card
        }
        else if (castChoiceId == 1)
        {
            HandleCastChoice(_cardWithCastModes.BackCard);
        }
    }

    private void HandleCastChoice(CardInstance instance)
    {
        if (instance.IsOfType<UnitCardData>())
        {
            _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, instance));
        }
        else if (instance.IsOfType<ManaCardData>())
        {
            var playManaAction = new PlayManaAction
            {
                Player = _actingPlayer,
                Card = instance
            };

            if (!playManaAction.IsValidAction(_cardGame))
            {
                return;
            }
            _cardGame.ProcessAction(playManaAction);
        }
        else if (instance.IsOfType<SpellCardData>() || instance.IsOfType<ItemCardData>())
        {
            _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, instance));
        }
    }
}


