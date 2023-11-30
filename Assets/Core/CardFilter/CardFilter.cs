using System;
using System.Collections.Generic;
using System.Linq;

[Obsolete("Refactor this filter to use a component based approach rather than a property based approach.")]
public class CardFilter
{
    public string CreatureType { get; set; }
    public string Subtype { get; set; }
    public string CardType { get; set; }

    public CardColor? CardColor { get; set; }
    public IManaFilter ManaCheck { get; set; }

    public bool Not { get; set; } = false; //Search for things that don't match the criteria.

    public string RulesTextString(bool plural = false)
    {
        /*
         * 
         * What is the order of operations?
         * 
         * i.e. Green Creature, Blue Sorcery, Basic Land
         * i.e. Red Goblin Creature
         * 
         * {Color} {CreatureType} {Subtype} {CardType} card with {AdditionalAttributes - (has a keyworded ability, certain mana cost)}
         * 
         */
        var str = "";

        if (CardColor != null)
        {
            str = AppendToFilterStr(str, $"{CardColor}");
        }

        if (CreatureType != null)
        {
            str = AppendToFilterStr(str, $"{CreatureType}");
        }

        if (Subtype != null)
        {
            str = AppendToFilterStr(str, $"{Subtype}");
        }

        if (CardType != null)
        {
            str = AppendToFilterStr(str, $"{CardType}");
        }

        if (CreatureType != null)
        {
            str = AppendToFilterStr(str, $"unit");
        }
        else
        {
            str = AppendToFilterStr(str, $"card");
        }

        if (plural)
        {
            str += "s";
        }

        if (ManaCheck != null)
        {
            str = AppendToFilterStr(str, " with " + ManaCheck.RulesTextString());
        }

        return str;
    }

    private string AppendToFilterStr(string existingStr, string input)
    {
        if (existingStr.IsNullOrEmpty())
        {
            return input;
        }
        else
        {
            return existingStr + " " + input;
        }
    }

    /// <summary>
    /// Applys a CardFilter to a list of CardGameEntities. Note that the filter will only filter out CardInstances.
    /// Any non CardInstances in the list will not be effected.
    /// </summary>
    /// <param name="list"></param>d
    /// <param name="filter"></param>
    /// <returns></returns>
    public IEnumerable<CardGameEntity> ApplyFilter(IEnumerable<CardGameEntity> list)
    {
        var nonCards = list.Where(e => e is not CardInstance);
        var cards = list.Where(e => e is CardInstance).Cast<CardInstance>().ToList();

        var cardsFiltered = ApplyFilter(cards, this);

        return nonCards.Concat(cardsFiltered);
    }

    [Obsolete("Use the non static ApplyFilter method instead")]
    public static List<CardInstance> ApplyFilter(List<CardInstance> list, CardFilter filter)
    {
        if (filter == null)
        {
            return list;
        }

        //Example, if we updated to a component based approach, these would instead be their own components.
        Func<CardInstance, bool> cardColorFilter = x => x.Colors.Contains((CardColor)filter.CardColor);
        Func<CardInstance, bool> creatureTypeFilter = x => x.CurrentCardData is UnitCardData && x.CreatureType.ToLower() == filter.CreatureType.ToLower();
        Func<CardInstance, bool> subTypeFilter = x => x.Subtype?.ToLower() == filter.Subtype.ToLower();
        Func<CardInstance, bool> cardTypeFilter = x => x.IsOfType(filter.CardType);
        Func<CardInstance, bool> manaFilter = x => filter.ManaCheck.Check(x);

        if (filter.Not)
        {
            cardColorFilter = x => !(x.Colors.Contains((CardColor)filter.CardColor));
            creatureTypeFilter = x => x.CurrentCardData is UnitCardData && x.CreatureType.ToLower() != filter.CreatureType.ToLower();
            subTypeFilter = x => x.Subtype?.ToLower() != filter.Subtype.ToLower();
            cardTypeFilter = x => !x.IsOfType(filter.CardType);
            manaFilter = x => !filter.ManaCheck.Check(x);
        }

        if (filter.CardColor != null)
        {
            list = list.Where(cardColorFilter).ToList();
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




