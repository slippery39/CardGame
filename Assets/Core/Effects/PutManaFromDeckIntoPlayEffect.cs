using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PutManaFromDeckIntoPlayEffect : Effect
{
    public int Amount { get; set; } = 1;
    public bool ForceEmpty { get; set; } = true;
    public override string RulesText => " play a mana card from your deck";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var manaCards = player.Deck.Cards.Where(card => card.IsOfType<ManaCardData>()).ToList();
        manaCards.Randomize();

        //Edge case, no mana cards left.
        for (int i = 0; i < Amount; i++)
        {
            if (manaCards.Count == 0)
            {
                break;
            }

            var manaCardToPlay = manaCards[0];
            //TODO - should be either tapped or untapped.
            cardGame.ManaSystem.PlayManaCardFromEffect(player, manaCardToPlay, ForceEmpty);
            manaCards.Remove(manaCardToPlay);
        }
    }
}

