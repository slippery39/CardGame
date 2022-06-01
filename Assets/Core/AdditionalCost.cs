using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// We need a class to handle the additional information we need to pass when we are paying an additional cost...
/// But it also needs to be abstract enough to handle various types of situations..
/// 
/// 
/// List<CardGameEntity> entitiesChosen</CardGameEntity>
/// 
/// </summary>
public class CostInfo
{   
    //do we need other information here??
    public List<CardGameEntity> EntitiesChosen { get; set; }
    
}

public abstract class AdditionalCost
{    public AdditionalCostType Type { get; set; }
     public int Amount { get; set; }    

    public abstract string RulesText { get; }
    public abstract bool CanPay(CardGame cardGame, Player player, CardGameEntity source);

    public abstract void PayCost(CardGame cardGame, Player player,CardGameEntity sourceCard);
    public abstract void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo);



    public bool NeedsChoice { get; set; } = false;
    public virtual List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        return new List<CardGameEntity>();
    }
};

public class PayLifeAdditionalCost: AdditionalCost
{
    public override string RulesText => $@"Pay {Amount} Life";

    public PayLifeAdditionalCost()
    {
        Type = AdditionalCostType.PayLife;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.Health >= Amount;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard)
    {
        player.Health -= Amount;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        PayCost(cardGame, player, sourceCard);
    }
}

public class SacrificeSelfAdditionalCost: AdditionalCost
{
    public override string RulesText => $@"Sacrifice #this#"; //#this needs to be replaced with the name of the unit.

    public SacrificeSelfAdditionalCost()
    {
        Type = AdditionalCostType.SacrificeSelf;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        //check if the source is in play...

        if (!(source is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }
        return cardGame.GetZoneOfCard(source as CardInstance).Name.ToLower() == "lane";
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard)
    {
        if (!(sourceCard is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }

        cardGame.SacrificeSystem.SacrificeUnit(cardGame,player,sourceCard as CardInstance);
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        PayCost(cardGame, player, sourceCard);
    }
}

public class SacrificeCreatureAdditionalCost : AdditionalCost
{
   public override string RulesText => $@"Sacrifice a creature:"; //#this needs to be replaced with the name of the unit.

    public SacrificeCreatureAdditionalCost()
    {        
        Type = AdditionalCostType.Sacrifice;
        NeedsChoice = true;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        //check to see if we have any units that can be sacrificed
        return player.Lanes.Where(l => !l.IsEmpty()).Any();
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard)
    {
        throw new Exception("We need cost info for a SacrificeCreatureAdditionalCost");
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        //Creature to sacrifice should be in the cost info.
        var entitiesToSacrifice = costInfo.EntitiesChosen.Cast<CardInstance>().ToList();

        foreach (var entity in entitiesToSacrifice)
        {
            cardGame.SacrificeSystem.SacrificeUnit(cardGame,player,entity);
        }
    }

    public override List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        //The valid choices are the players units in play
        var choices = player.Lanes.Where(l => !(l.IsEmpty())).Select(l=>l.UnitInLane).Cast<CardGameEntity>();
        return choices.ToList();
    }
}




