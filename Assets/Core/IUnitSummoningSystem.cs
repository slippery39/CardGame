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
        //Do not cast the spell if there isn't enough mana.
        if (!cardGame.ManaSystem.CanPlayCard(cardGame, player, unitCard))
        {
            cardGame.Log($"Could not play card {unitCard.Name}. Not enough mana.");
            return;
        }

        cardGame.ManaSystem.SpendMana(cardGame, player, Convert.ToInt32(unitCard.ManaCost));

        var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty() && lane.EntityId == laneId);

        if (!emptyLanes.Any())
        {
            cardGame.Log("Cannot summon unit, all lanes are full");
            return;
        }

        cardGame.ZoneChangeSystem.MoveToZone(cardGame, unitCard,emptyLanes.First());

        //Search for TriggeredAbilities with the SelfEntersPlay effect
        var entersPlayAbilities = unitCard.GetAbilities<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfEntersPlay);

        if (entersPlayAbilities.Count() > 0)
        {
            foreach(var ability in entersPlayAbilities)
            {
                //Note, we do not support triggered abilities with targets yet.
                cardGame.EffectsProcessor.ApplyEffects(cardGame, player, unitCard, ability.Effects, new List<CardGameEntity>());
            }
        }
    }
}
