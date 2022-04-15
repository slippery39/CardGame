using System;
using System.Collections.Generic;
using System.Linq;

public interface IUnitSummoningSystem
{
    void SummonUnit(CardGame cardGame, Player player, CardInstance unitCard, int laneId);
}

public class DefaultUnitSummoningSystem : IUnitSummoningSystem
{
    public void SummonUnit(CardGame cardGame, Player player, CardInstance unitCard, int laneId)
    {
        var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty() && lane.EntityId == laneId);

        if (!emptyLanes.Any())
        {
            cardGame.Log("Cannot summon unit, all lanes are full");
            return;
        }
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, unitCard, emptyLanes.First());
        //Search for TriggeredAbilities with the SelfEntersPlay effect
        cardGame.HandleTriggeredAbilities(new List<CardInstance> { unitCard }, TriggerType.SelfEntersPlay);
    }
}
