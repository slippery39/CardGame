using System.Collections.Generic;
using System.Linq;

public class GetRandomCardFromDeckEffect : Effect
{

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

    public CardFilter Filter { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        cardGame.CardDrawSystem.GrabRandomCardFromDeck(player, Filter);
    }
}


