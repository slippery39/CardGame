using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Note currently we are only handling discard choices.
public class GameUIDiscardAsPartOfSpellState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private DiscardCardEffect _sourceEffect;

    public List<CardInstance> _cardsChosen;
    public GameUIDiscardAsPartOfSpellState(GameUIStateMachine stateMachine, DiscardCardEffect sourceEffect)
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
        return "Please discard a card";
    }

    public void OnApply()
    {
        //Slightly differs from our usual highlight, we are highlighting the cards we have chosen to discard in red.
        var validChoices = _actingPlayer.Hand.Cards;

        if (validChoices.Count() == 0)
        {
            return;
        }

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
    }

    public void HandleSelection(int entityId)
    {
        var entitySelected = _cardGame.GetEntities<CardInstance>().Where(e => e.EntityId == entityId).FirstOrDefault();

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
            //Otherwise let's send the choice over to the card game.
            _cardGame.MakeChoice(_cardsChosen);
            _stateMachine.ToIdle();
        }
    }
}
