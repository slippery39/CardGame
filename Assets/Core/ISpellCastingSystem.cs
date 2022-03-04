﻿using System.Collections.Generic;
using System.Linq;

public interface ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard);
    //public List<CardInstance> GetValidTargets(Player player, CardInstance spellCard);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard)
    {
        //TODO - check if spell card is a spell card.
        foreach (var ab in spellCard.Abilities)
        {
            if (ab is DamageAbility)
            {
                //For now just grab a random target on the opponents side of the board.
                var validTargets = cardGame.Player2.Lanes.Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).ToList();

                if (validTargets.Count == 0)
                {
                    continue;
                }

                var target = validTargets.Randomize().First();

                cardGame.DamageSystem.DealAbilityDamage(cardGame, (DamageAbility)ab, spellCard, target);

            }
            //Figure out how to resolve abilities.
        }

        //TODO - Spell should move into the discard zone
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, spellCard, player.DiscardPile);

    }

    public List<CardInstance> GetValidTargets(Player player, SpellCardData spellCard)
    {
        throw new System.NotImplementedException();
    }
}
