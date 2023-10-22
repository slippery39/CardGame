using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUIChoiceAsPartOfResolveState : IGameUIState
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private GameService _gameService => _stateMachine.GameService;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private IEffectWithChoice _sourceEffect;

    public List<CardInstance> _cardsChosen;

    private IGameUIState _internalState;

    public GameUIChoiceAsPartOfResolveState(GameUIStateMachine stateMachine, IEffectWithChoice sourceEffect)
    {
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

        switch (_sourceEffect)
        {
            case DiscardCardEffect dce:
                {
                    _internalState = new GameUIDiscardAsPartOfSpellState(_stateMachine, dce);
                    _internalState.OnApply();
                    break;
                }
            default:
                {
                    if (_sourceEffect.NumberOfChoices > 1)
                    {
                        _internalState = new GameUIMultiChoiceState(_stateMachine, _sourceEffect);
                        _internalState.OnApply();
                    }
                    else
                    {
                        var cardsToShow = _sourceEffect.GetValidChoices(_cardGame, _actingPlayer);
                        var cardIds = cardsToShow.Select(c => c.EntityId);
                        _stateMachine.GameController.ViewChoiceWindow(cardsToShow, GetMessage(),false);
                        _stateMachine.GameController.GetUIEntities().Where(e => cardIds.Contains(e.EntityId))
                            .ToList()
                            .ForEach(e => e.Highlight());                            
                    }
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

        var entitySelected = _stateMachine.CardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();
        _cardsChosen.Add(entitySelected);

        var makeChoiceAction = new ResolveChoiceAction
        {
            Player = _actingPlayer,
            Choices = _cardsChosen
        };

        if (makeChoiceAction.Choices.Select(s => s.EntityId).Where(s => s == -1).Any())
        {
            var debugPoint = 0;
        }

        if (!makeChoiceAction.IsValidAction(_stateMachine.CardGame))
        {
            //TODO - Should probably clear out the choice or something here?
            return;
        }

        _gameService.ProcessAction(makeChoiceAction);
        _stateMachine.ToIdle();
    }
}
