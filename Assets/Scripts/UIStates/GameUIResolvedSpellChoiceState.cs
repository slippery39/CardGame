using System.Linq;

//Note currently we are only handling discard choices.
public class GameUIResolvedSpellChoiceState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer => _cardGame.ActivePlayer;
    private GameUIStateMachine _stateMachine;
    private Effect _sourceEffect;
    public GameUIResolvedSpellChoiceState(GameUIStateMachine stateMachine, Effect sourceEffect)
    {
        _cardGame = stateMachine.CardGame;
        _stateMachine = stateMachine;
        _sourceEffect = sourceEffect;
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
            entity.Highlight();
        }
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

        //Otherwise let's send the choice over to the card game.
        _cardGame.MakeChoice(entitySelected);
        _stateMachine.ToIdle();
    }
}
