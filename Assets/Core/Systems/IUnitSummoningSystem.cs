using System;
using System.Collections.Generic;
using System.Linq;

public interface IUnitSummoningSystem
{
    void SummonUnit( Player player, CardInstance unitCard, int laneId);
}

public class DefaultUnitSummoningSystem : IUnitSummoningSystem
{
    private CardGame cardGame;

    public DefaultUnitSummoningSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void SummonUnit( Player player, CardInstance unitCard, int laneEntityId)
    {
        var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty() && lane.EntityId == laneEntityId);

        if (!emptyLanes.Any())
        {
            cardGame.Log("Cannot summon unit, all lanes are full");
            return;
        }
        cardGame.ZoneChangeSystem.MoveToZone(unitCard, emptyLanes.First());
        //Search for TriggeredAbilities with the SelfEntersPlay effect
        cardGame.HandleTriggeredAbilities(new List<CardInstance> { unitCard }, TriggerType.SelfEntersPlay);
    }
}
