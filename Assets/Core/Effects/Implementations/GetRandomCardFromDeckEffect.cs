using System.Collections.Generic;
using System.Linq;

public class GetRandomCardFromDeckEffect : Effect
{
    public int Amount { get; set; } = 1;

    public override string RulesText
    {
        get
        {
            var str = "draw a random #cardType# from your deck";

            if (Filter?.CreatureType != null)
            {
                return str.Replace("#cardType#", Filter.CreatureType);
            }
            else
            {
                return str.Replace("#cardType#", "card");
            }
        }
    }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        for (var i = 0; i < Amount; i++)
        {
            cardGame.CardDrawSystem.GrabRandomCardFromDeck(player, Filter);
        }
    }
}


