using System.Collections.Generic;
using System.Linq;

public class SacrificeAdditionalCost : AdditionalCost
{
    public override string RulesText
    {
        get
        {
            return $"Sacrifice a {Filter.Subtype}";
        }
    }


    public SacrificeAdditionalCost()
    {
        Type = AdditionalCostType.Sacrifice;
        NeedsChoice = true;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        var thingsToSacrifice = player.GetCardsInPlay();

        if (Filter != null)
        {
            thingsToSacrifice = CardFilter.ApplyFilter(thingsToSacrifice.ToList(), Filter);
        }

        //TODO - apply the filter.

        return thingsToSacrifice.Any();

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
        List<CardInstance> choices = player.GetCardsInPlay();

        if (Filter != null)
        {
            choices = CardFilter.ApplyFilter(choices, Filter).ToList();
        }

        return choices.Cast<CardGameEntity>().ToList();
    }
}




