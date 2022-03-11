using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard)
    {

        //Do not cast the spell if there isn't enough mana.
        if (!cardGame.ManaSystem.CanPlayCard(cardGame, player, spellCard))
        {
            cardGame.Log($"Could not play card {spellCard.Name}. Not enough mana.");
            return;
        }

        //TODO - Handle mana costs better
        cardGame.ManaSystem.SpendMana(cardGame, player, Convert.ToInt32(spellCard.ManaCost));

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
            if (ab is LifeGainAbility)
            {
                var owner = cardGame.GetOwnerOfCard(spellCard);
                cardGame.HealingSystem.HealPlayer(cardGame, owner, ((LifeGainAbility)ab).Amount);

            }
            if (ab is PumpUnitAbility)
            {
                var validTargets = cardGame.Player1.Lanes.Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).ToList();

                if (validTargets.Count == 0)
                {
                    continue;
                }

                var target = validTargets.Randomize().First();

                cardGame.UnitPumpSystem.PumpUnit(cardGame, target, (PumpUnitAbility)ab);
            }
            if (ab is DrawCardAbility)
            {
                var ability = (DrawCardAbility)ab;
                var owner = cardGame.GetOwnerOfCard(spellCard);
                for (int i = 0; i < ability.Amount; i++)
                {
                    cardGame.CardDrawSystem.DrawCard(cardGame, owner);
                }
            }
            if (ab is AddManaAbility)
            {
                var ability = (AddManaAbility)ab;
                var owner = cardGame.GetOwnerOfCard(spellCard);
                cardGame.ManaSystem.AddMana(cardGame, owner, ability.Amount);
            }
            //Figure out how to resolve abilities.
        }
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, spellCard, player.DiscardPile);
    }
}
