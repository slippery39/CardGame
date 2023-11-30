using System.Collections.Generic;
using System.Linq;

public class SacrificeCreatureAdditionalCost : AdditionalCost
{
    public override string RulesText
    {
        get
        {
            if (Filter?.CreatureType != null)
            {
                return $"Sacrifice a {Filter.CreatureType}";
            }
            return "Sacrifice a creature";
        }
    }


    public SacrificeCreatureAdditionalCost()
    {
        Type = AdditionalCostType.Sacrifice;
        NeedsChoice = true;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        var unitsToSacrifice = player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane);

        if (Filter != null)
        {
            unitsToSacrifice = CardFilter.ApplyFilter(unitsToSacrifice.ToList(), Filter);
        }

        //TODO - apply the filter.

        return unitsToSacrifice.Any();

    }
    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        //Creature to sacrifice should be in the cost info.
        var entitiesToSacrifice = costInfo.EntitiesChosen.Cast<CardInstance>().ToList();

        foreach (var entity in entitiesToSacrifice)
        {
            cardGame.SacrificeSystem.Sacrifice(player, entity);
        }
    }

    public override List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        //The valid choices are the players units in play
        var choices = player.Lanes.Where(l => !(l.IsEmpty())).Select(l => l.UnitInLane).Cast<CardGameEntity>();

        if (Filter != null)
        {
            choices = CardFilter.ApplyFilter(choices.Cast<CardInstance>().ToList(), Filter).Cast<CardGameEntity>();
        }

        return choices.ToList();
    }
}




