using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SummoningTrapEffect : Effect
{
    public override string RulesText => "Put the highest generic mana cost unit from the top 7 cards of your deck into play.";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var cardsToCheck = player.Deck.Cards.ToList();
        cardsToCheck.Reverse();

        cardsToCheck = cardsToCheck.Take(7).ToList();

        cardsToCheck = cardsToCheck.Where(c => c.IsOfType<UnitCardData>()).ToList();

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

        cardGame.UnitSummoningSystem.SummonUnit(player, cardsToCheck.First(), player.GetEmptyLanes().First().EntityId);

        //TODO - grab the first one.

    }
}

