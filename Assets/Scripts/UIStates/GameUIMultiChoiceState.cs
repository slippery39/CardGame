using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Now used in our GameUIChoiseAsPartOfResolveState class.
public class GameUIMultiChoiceState : IGameUIState
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private GameService _gameService => _stateMachine.GameService;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private IEffectWithChoice _sourceEffect;

    public List<CardInstance> _cardsChosen;
    public GameUIMultiChoiceState(GameUIStateMachine stateMachine, IEffectWithChoice sourceEffect)
    {
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
        //Slightly differs from our usual highlight, we are highlighting the cards we have chosen to discard in red.
        var validChoices = _sourceEffect.GetValidChoices(_cardGame, _actingPlayer);

        if (!validChoices.Any())
        {
            return;
        }

        _stateMachine.GameController.ViewChoiceWindow(validChoices, GetMessage(), false);

        var uiEntities = _stateMachine.GameController.GetUIEntities();
        var choicesAsInts = validChoices.Select(c => c.EntityId).ToList();
        //Highlight all entities that share an entity id with the valid choices;   

        var entitiesToHighlight = uiEntities.Where(e => choicesAsInts.Contains(e.EntityId));

        foreach (var entity in entitiesToHighlight)
        {
            if (_cardsChosen.Select(c => c.EntityId).Contains(entity.EntityId))
            {
                entity.Highlight(Color.red);
            }
            else
            {
                //Highlight with a red color if it is an already chosen card?
                entity.Highlight();
            }
        }
    }

    public void OnUpdate()
    {
        OnApply();
    }

    public void OnDestroy()
    {
        var uiEntities = _stateMachine.GameController.GetUIEntities();
        foreach (var entity in uiEntities)
        {
            entity.StopHighlight();
        }

        _stateMachine.GameController.CloseChoiceWindow();

    }

    public void HandleSelection(int entityId)
    {
        var entitySelected = _cardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();

        if (entitySelected == null)
        {
            return;
        }

        if (!_sourceEffect.GetValidChoices(_cardGame, _actingPlayer).Contains(entitySelected))
        {
            return;
        }

        //This used to be in the MultiEffect.MakeChoice method.
        if (_sourceEffect.Choices.Contains(entitySelected))
        {
            return;
        }
        else
        {
            _sourceEffect.Choices.Add(entitySelected);
        }


        if (_sourceEffect.Choices.Count >= _sourceEffect.NumberOfChoices)
        {

            var makeChoiceAction = new ResolveChoiceAction
            {
                Player = _actingPlayer,
                Choices = _cardsChosen
            };

            if (!makeChoiceAction.IsValidAction(_cardGame))
            {
                //TODO - Should probably clear out the choice or something here?
                return;
            }

            _gameService.ProcessAction(makeChoiceAction);
            _stateMachine.ToIdle();
        }
    }
}
