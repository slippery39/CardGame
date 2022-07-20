using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PutSpellOnTopOfDeckEffect : Effect
{
    public override string RulesText => "Put a spell on top of your deck";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var randomSpell = player.Deck.Cards.Where(card => card.IsOfType<SpellCardData>()).Randomize();

        if (randomSpell.Any())
        {
            var spell = randomSpell.First();
            player.Deck.Cards.Remove(spell);
            player.Deck.Cards.Add(spell);
            cardGame.Log($"{spell.Name} has been moved to the top of the deck");
        }
    }
}
