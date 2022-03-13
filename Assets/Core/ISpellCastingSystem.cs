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
    //TODO - this should have targets in the method signature.
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

        var effects = ((SpellCardData)spellCard.CurrentCardData).Effects;


        //TODO - check if spell card is a spell card.
        foreach (var effect in effects)
        {
            if (effect is DamageEffect)
            {
                //For now just grab a random target on the opponents side of the board.
                var validTargets = cardGame.Player2.Lanes.Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).ToList();

                if (validTargets.Count == 0)
                {
                    continue;
                }

                var target = validTargets.Randomize().First();

                cardGame.DamageSystem.DealAbilityDamage(cardGame, (DamageEffect)effect, spellCard, target);
            }
            if (effect is LifeGainEffect)
            {
                var owner = cardGame.GetOwnerOfCard(spellCard);
                cardGame.HealingSystem.HealPlayer(cardGame, owner, ((LifeGainEffect)effect).Amount);

            }
            if (effect is PumpUnitEffect)
            {
                var validTargets = cardGame.Player1.Lanes.Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).ToList();

                if (validTargets.Count == 0)
                {
                    continue;
                }

                var target = validTargets.Randomize().First();

                cardGame.UnitPumpSystem.PumpUnit(cardGame, target, (PumpUnitEffect)effect);
            }
            if (effect is DrawCardEffect)
            {
                var ability = (DrawCardEffect)effect;
                var owner = cardGame.GetOwnerOfCard(spellCard);
                for (int i = 0; i < ability.Amount; i++)
                {
                    cardGame.CardDrawSystem.DrawCard(cardGame, owner);
                }
            }
            if (effect is AddManaEffect)
            {
                var ability = (AddManaEffect)effect;
                var owner = cardGame.GetOwnerOfCard(spellCard);
                cardGame.ManaSystem.AddMana(cardGame, owner, ability.Amount);
            }
            //Figure out how to resolve abilities.
        }
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, spellCard, player.DiscardPile);
    }
}
