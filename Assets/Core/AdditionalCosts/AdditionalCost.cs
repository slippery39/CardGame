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

public enum CardType
{
    Unit,
    Spell,
    Item,
    Mana
}

public interface IManaFilter
{
    bool Check(CardInstance cardToCheck);
}


public class LessThanManaFilter : IManaFilter
{
    public int Amount { get; set; }
    public bool Check(CardInstance cardToCheck)
    {
        var mana = new Mana(cardToCheck.ManaCost);
        return mana.ColorlessMana < Amount;
    }
}

public class CardFilter
{
    public string CreatureType { get; set; }
    public string Subtype { get; set; }
    public string CardType { get; set; }
    public IManaFilter ManaCheck { get; set; }

    public bool Not { get; set; } = false; //Search for things that don't match the criteria.

    public static List<CardInstance> ApplyFilter(List<CardInstance> list, CardFilter filter)
    {
        if (filter == null)
        {
            return list;
        }

        Func<CardInstance, bool> creatureTypeFilter = x => x.CurrentCardData is UnitCardData && x.CreatureType == filter.CreatureType;
        Func<CardInstance, bool> subTypeFilter = x => x.Subtype?.ToLower() == filter.Subtype.ToLower();
        Func<CardInstance, bool> cardTypeFilter = x => x.IsOfType(filter.CardType);
        Func<CardInstance, bool> manaFilter = x => filter.ManaCheck.Check(x);

        if (filter.Not)
        {
            creatureTypeFilter = x => x.CurrentCardData is UnitCardData && x.CreatureType != filter.CreatureType;
            subTypeFilter = x => x.Subtype?.ToLower() != filter.Subtype.ToLower();
            cardTypeFilter = x => !x.IsOfType(filter.CardType);
            manaFilter = x => !filter.ManaCheck.Check(x);
        }

        //TODO - need to apply a NOT to everything (but how?)
        if (filter.CreatureType != null)
        {
            //Things to consider: 
            //Should have a method for if a card is a creature, we shouldn't be checking strings directly?
            //Should also have a method to check what type a card is?
            //Should also have a method to automatically filter a list of CardInstances based off of its type and return the proper cast.
            //i.e. List.GetOfType<UnitCard>()
            list = list.Where(creatureTypeFilter).ToList();
        }
        if (filter.Subtype != null)
        {
            list = list.Where(subTypeFilter).ToList();
        }

        if (filter.CardType != null && filter.CardType.Trim() != "")
        {
            list = list.Where(cardTypeFilter).ToList();
        }

        if (filter.ManaCheck != null)
        {
            list = list.Where(manaFilter).ToList();
        }
        return list;
    }
}

public abstract class AdditionalCost
{
    public AdditionalCostType Type { get; set; }
    public int Amount { get; set; }

    public CardFilter Filter { get; set; }

    public abstract string RulesText { get; }
    public abstract bool CanPay(CardGame cardGame, Player player, CardGameEntity source);

    //NOTE - we should get rid of this one, always implement the one with CostInfo.
    //public abstract void PayCost(CardGame cardGame, Player player,CardGameEntity sourceCard);

    //NOTE - we should always implement this one going forward.
    public abstract void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo);
    public bool NeedsChoice { get; set; } = false;
    public virtual List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        return new List<CardGameEntity>();
    }
};

public class PayLifeAdditionalCost : AdditionalCost
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

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        player.Health -= Amount;
    }
}

public class SacrificeManaAdditionalCost : AdditionalCost
{
    public override string RulesText => $@" Sacrifice {Amount} mana";

    public SacrificeManaAdditionalCost()
    {
        Type = AdditionalCostType.SacrificeMana;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.ManaPool.TotalColorlessMana >= Amount;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        player.ManaPool.TotalColorlessMana -= Amount;
    }
}

public class ExileRandomCreatureFromDiscardAdditionalCost : AdditionalCost
{
    public override string RulesText => "Exile a random unit from your graveyard";

    public ExileRandomCreatureFromDiscardAdditionalCost()
    {
        Type = AdditionalCostType.ExileRandomCreatureFromDiscard;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.DiscardPile.Cards.Where(card => card.IsOfType<UnitCardData>()).Any();
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        var cardChosen = player.DiscardPile.Cards.Where(card => card.IsOfType<UnitCardData>()).Randomize().FirstOrDefault();
        cardGame.ZoneChangeSystem.MoveToZone(cardChosen, player.Exile);
    }
}




public class SacrificeSelfAdditionalCost : AdditionalCost
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
        return cardGame.IsInPlay(source as CardInstance);
    }
    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        if (!(sourceCard is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }

        cardGame.SacrificeSystem.Sacrifice(player, sourceCard as CardInstance);
    }
}


public class DiscardCardAdditionalCost : AdditionalCost
{
    public override string RulesText
    {
        get
        {
            return "Discard a card";
        }
    }

    public DiscardCardAdditionalCost()
    {
        Type = AdditionalCostType.Discard;
        NeedsChoice = true;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        var cardsInHand = player.Hand.Cards;

        return cardsInHand.Any();
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        //Creature to sacrifice should be in the cost info.
        var cardsToDiscard = costInfo.EntitiesChosen.Cast<CardInstance>().ToList();

        foreach (var entity in cardsToDiscard)
        {
            cardGame.DiscardSystem.Discard(player, entity);
        }
    }
    public override List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        //The valid choices are the players units in play
        var choices = player.Hand.Cards;

        return choices.Cast<CardGameEntity>().ToList();
    }
}


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




