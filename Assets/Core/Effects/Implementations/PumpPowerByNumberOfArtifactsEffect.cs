using System;
using System.Collections.Generic;
using System.Linq;

public class PumpPowerByNumberOfArtifactsEffect : Effect
{

    private int CountArtifacts(CardGame cardGame, Player player)
    {
        var thingsInPlay = cardGame.GetUnitsInPlay().Where(u => cardGame.GetOwnerOfCard(u) == player).ToList();
        thingsInPlay.AddRange(player.Items.Cards);
        return thingsInPlay.Where(thing => thing.Subtype.ToLower() == "artifact").Count();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with PumpPowerByNumberOfArtifactsEffect");
            }

            var card = (CardInstance)entity;

            Func<CardGame, CardInstance, int, int> powerMod = (c, ci, o) =>
            {
                return o + CountArtifacts(c, c.GetOwnerOfCard(card));
            };

            //We will need a new modification.
            var mod = new ModAddXToPowerToughness(powerMod, null);
            mod.OneTurnOnly = true;
            cardGame.ModificationsSystem.AddModification(card, mod);
        }
    }

    public override string RulesText
    {
        get
        {
            return $"A unit gets +X/+0 until end of turn where X is the amount of artifacts you control.";
        }
    }
}


