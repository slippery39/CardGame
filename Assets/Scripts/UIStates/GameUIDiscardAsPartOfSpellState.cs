using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Now used in our GameUIChoiseAsPartOfResolveState class.
public class GameUIDiscardAsPartOfSpellState : IGameUIState
{
    private CardGame _cardGame => _stateMachine.CardGame;
    private GameService _gameService => _stateMachine.GameService;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private DiscardCardEffect _sourceEffect;

    public List<CardInstance> _cardsChosen;
    public GameUIDiscardAsPartOfSpellState(GameUIStateMachine stateMachine, DiscardCardEffect sourceEffect)
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
        return "Please discard a card";
    }

    public void OnApply()
    {
        //Slightly differs from our usual highlight, we are highlighting the cards we have chosen to discard in red.
        var validChoices = _actingPlayer.Hand;

        if (validChoices.Count() == 0)
        {
            return;
        }

        var uiEntities = _stateMachine.GameController.GetUIEntities();
        var choicesAsInts = validChoices.Select(c => c.EntityId).ToList();
        //Highlight all entities that share an entity id with the valid choices;   
        var i = 0;
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
        _stateMachine.GameController.SetStateLabel(GetMessage());
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
    }

    public void HandleSelection(int entityId)
    {
        var entitySelected = _stateMachine.GameController.CardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();

        if (entitySelected == null)
        {
            return;
        }

        if (_cardGame.GetZoneOfCard(entitySelected).ZoneType != ZoneType.Hand)
        {
            return;
        }

        _cardsChosen.Add(entitySelected);
        //TODO - some sort of indicator of the cards chosen.

        if (_cardsChosen.Count >= _sourceEffect.Amount)
        {
            var makeChoiceAction = new ResolveChoiceAction
            {
                Player = _actingPlayer,
                Choices = _cardsChosen
            };

            if (!makeChoiceAction.IsValidAction(_stateMachine.CardGame))
            {
                //TODO - Should probably clear out the choice or something here?
                return;
            }

            _gameService.ProcessAction(makeChoiceAction);
            _stateMachine.ToIdle();
        }
    }
}
