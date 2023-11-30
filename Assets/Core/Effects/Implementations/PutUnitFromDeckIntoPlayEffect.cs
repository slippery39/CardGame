using System;
using System.Collections.Generic;
using System.Linq;

public class PutUnitFromDeckIntoPlayEffect : Effect
{
    public int Amount { get; set; } = 1;
    public CardFilter Filter { get; set; }

    public override string RulesText
    {
        get
        {
            string amountTxt = Amount == 1 ? "a" : Amount.ToString();
            string cardTxt = Filter == null ? "unit" : Filter.RulesTextString();
            string pluralTxt = Amount > 1 ? "s" : "";
            return $"Play {amountTxt} random {cardTxt}{pluralTxt} from your deck";
        }
    }
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var validCards = CardFilter.ApplyFilter(player.Deck.Cards.ToList(), Filter);
        validCards.Randomize();
        var amountToPutIntoPlay = Math.Min(validCards.Count, Amount);

        for (var i = 0; i < amountToPutIntoPlay; i++)
        {
            if (player.GetEmptyLanes().Any())
            {
                cardGame.UnitSummoningSystem.SummonUnit(player, validCards[0], player.GetEmptyLanes()[0].EntityId);
                validCards.RemoveAt(0);
            }
        }
        cardGame.CardDrawSystem.Shuffle(player);
    }
}





