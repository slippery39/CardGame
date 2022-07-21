using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PumpPowerByNumberOfSpellsInGraveyardEffect : Effect
{
    public override string RulesText => "unit gets +X/+0 where X is the number of spells in the owner's graveyard";

    private int CountSpells(Player player)
    {
        var thingsInDiscard = player.DiscardPile.Cards.Where(c => c.IsOfType<SpellCardData>());
        return thingsInDiscard.Count();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with PumpPowerByNumberOfSpellsInGraveyardEffect");
            }

            var card = (CardInstance)entity;

            Func<CardGame, CardInstance, int, int> powerMod = (c, ci, o) =>
            {
                return o + CountSpells(c.GetOwnerOfCard(card));
            };

            //We will need a new modification.
            var mod = new ModAddXToPowerToughness(powerMod, null);
            mod.OneTurnOnly = true;
            cardGame.ModificationsSystem.AddModification(card, mod);
        }
    }
}

