using System;
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
    }
}
