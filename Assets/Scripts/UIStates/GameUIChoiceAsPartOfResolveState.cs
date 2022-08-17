using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Note currently we are only handling discard choices.
public class GameUIChoiceAsPartOfResolveState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private IEffectWithChoice  _sourceEffect;

    public List<CardInstance> _cardsChosen;
    public GameUIChoiceAsPartOfResolveState(GameUIStateMachine stateMachine, IEffectWithChoice sourceEffect)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _sourceEffect = sourceEffect;
        _cardsChosen = new List<CardInstance>();
    }

    public void HandleInput()
    {

    }

    public string GetMessage()
    {
        return _sourceEffect.ChoiceMessage;
    }

    public void OnApply()
    {
        //TODO
        //We will want to open a ZoneViewer with the top two cards of the players deck.
        
        _stateMachine.GameController.ViewChoiceWindow(_sourceEffect.GetValidChoices(_cardGame, _actingPlayer),GetMessage());
    }

    public void OnUpdate()
    {

    }

    public void OnDestroy()
    {
        _stateMachine.GameController.CloseChoiceWindow();
    }

    public void HandleSelection(int entityId)
    {
        var entitySelected = _cardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();
        _cardsChosen.Add(entitySelected);
        _cardGame.MakeChoice(_cardsChosen);
        _stateMachine.ToIdle();
    }
}
