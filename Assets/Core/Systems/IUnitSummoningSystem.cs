using System;
using System.Collections.Generic;
using System.Linq;

public interface IUnitSummoningSystem
{
    void SummonUnit(Player player, CardInstance unitCard, int laneId);
}

public class DefaultUnitSummoningSystem : CardGameSystem, IUnitSummoningSystem
{
    public DefaultUnitSummoningSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void SummonUnit(Player player, CardInstance unitCard, int laneEntityId)
    {
        var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty() && lane.EntityId == laneEntityId);

        if (!emptyLanes.Any())
        {
            cardGame.Log("Cannot summon unit, all lanes are full");
            return;
        }
        cardGame.ZoneChangeSystem.MoveToZone(unitCard, emptyLanes.First());

        cardGame.GameEventSystem.FireEvent(new UnitSummonedEvent
        {
            PlayerId = player.PlayerId,
            LaneId = laneEntityId,
            UnitId = unitCard.EntityId
        });

        //Search for any abilities with an EntersPlay callback, slightly different from triggered abilities as these would happen immediatly.

        var onSummonAbilities = unitCard.Abilities.GetOfType<IOnSummon>();

        foreach (var ability in onSummonAbilities)
        {
            ability.OnSummoned(cardGame, unitCard);
        }

        //Search for TriggeredAbilities with the SelfEntersPlay effect
        //This was moved to the zone change system.
        //cardGame.HandleTriggeredAbilities(new List<CardInstance> { unitCard }, TriggerType.SelfEntersPlay);
    }
}
