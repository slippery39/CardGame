using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUIChoiceAsPartOfResolveState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private IEffectWithChoice  _sourceEffect;

    public List<CardInstance> _cardsChosen;

    private IGameUIState _internalState;

    public GameUIChoiceAsPartOfResolveState(GameUIStateMachine stateMachine, IEffectWithChoice sourceEffect)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _sourceEffect = sourceEffect;
        _cardsChosen = new List<CardInstance>();
    }

    public void HandleInput()
    {
        if (_internalState != null)
        {
            _internalState.HandleInput();
            return;
        }
    }

    public string GetMessage()
    {
        if (_internalState != null)
        {
            return _internalState.GetMessage();           
        }
        return _sourceEffect.ChoiceMessage;
    }

    public void OnApply()
    {
        //TODO
        //We will want to open a ZoneViewer with the top two cards of the players deck.

        switch (_sourceEffect) {
            case DiscardCardEffect dce:
                {
                    _internalState = new GameUIDiscardAsPartOfSpellState(_stateMachine, dce);
                    _internalState.OnApply();
                    break;
                }
            default:
                {
                    _stateMachine.GameController.ViewChoiceWindow(_sourceEffect.GetValidChoices(_cardGame, _actingPlayer), GetMessage());
                    break;
                }
        }
    }

    public void OnUpdate()
    {
        _internalState?.OnUpdate();
    }

    public void OnDestroy()
    {
        if (_internalState != null)
        {
            _internalState.OnDestroy();
            return;
        }

        _stateMachine.GameController.CloseChoiceWindow();
    }

    public void HandleSelection(int entityId)
    {
        if (_internalState != null)
        {
            _internalState.HandleSelection(entityId);
            return;
        }

        var entitySelected = _cardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();
        _cardsChosen.Add(entitySelected);
        _cardGame.MakeChoice(_cardsChosen);
        _stateMachine.ToIdle();
    }
}
