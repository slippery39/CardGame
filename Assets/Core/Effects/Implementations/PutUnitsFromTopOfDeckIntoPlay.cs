﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PutUnitsFromTopOfDeckIntoPlay : Effect
{
    public int Amount { get; set; } = 1;
    public int CardsToLookAt { get; set; }
    public CardFilter Filter { get; set; }
    public override string RulesText
    {
        get
        {
            if (Amount == 1)
            {
                return $"Put the highest generic mana cost unit from the top {CardsToLookAt} cards of your deck into play";
            }

            var defaultCardType = "units";

            if (Filter != null)
            {
                defaultCardType = Filter.RulesTextString(true);
            }

            return $"Put the {Amount} highest generic mana cost {defaultCardType.ToLower()} from the top {CardsToLookAt} cards of your deck into play";
        }

    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var cardsToCheck = player.Deck.Cards.ToList();
        cardsToCheck.Reverse();

        var cardsToLookAt = Math.Min(CardsToLookAt, cardsToCheck.Count);

        cardsToCheck = cardsToCheck.Take(cardsToLookAt).ToList();

        cardsToCheck = cardsToCheck.Where(c => c.IsOfType<UnitCardData>()).ToList();

        cardsToCheck = CardFilter.ApplyFilter(cardsToCheck,Filter);

        if (cardsToCheck.Count == 0)
        {
            cardGame.Log("Whiffff...");
            return;
        }

        cardsToCheck.Sort(
            (a, b) =>
            {
                var aMana = new Mana(a.ManaCost);
                var bMana = new Mana(b.ManaCost);
                return bMana.ColorlessMana - aMana.ColorlessMana;
            });

        cardGame.Log("mana COSTS FOR SUMMONING TRAP sorted b");
        cardGame.Log(string.Join(", ", cardsToCheck.Select(c => c.ManaCost)));
        cardGame.Log("---------");

        var amountToPutIntoPlay = Math.Min(cardsToCheck.Count, Amount);

        for (var i = 0; i < amountToPutIntoPlay; i++)
        {
            if (player.GetEmptyLanes().Any())
            {
                cardGame.UnitSummoningSystem.SummonUnit(player, cardsToCheck[0], player.GetEmptyLanes()[0].EntityId);
                cardsToCheck.RemoveAt(0);
            }
        }
        //Should actually put the rest on the bottom.
        cardGame.CardDrawSystem.Shuffle(player);
    }
}

