using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, List<CardGameEntity> targets);
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, CardGameEntity target);
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    //TODO - this should have targets in the method signature.

    /// <summary>
    /// Gets the correct entities to apply an effect to when there is no manual targets.
    /// </summary>
    /// <param name="cardGame">The card game</param>
    /// <param name="player">The owner of the effect</param>
    /// <param name="effect">The effect that is being applied</param>
    /// <returns></returns>
    private List<CardGameEntity> GetEntitiesToApplyEffect(CardGame cardGame, Player player, Effect effect)
    {
        switch (effect.TargetType)
        {
            case TargetType.Self:
                return new List<CardGameEntity> { player };
            case TargetType.Opponent:
                return cardGame.Players.Where(p => p.EntityId != player.EntityId).Cast<CardGameEntity>().ToList();
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {effect.TargetType}");
        }
    }

    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, CardGameEntity target)
    {
        CastSpell(cardGame, player, spellCard, new List<CardGameEntity> { target });
    }

    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, List<CardGameEntity> targets)
    {
        if (!cardGame.ManaSystem.CanPlayCard(cardGame, player, spellCard))
        {
            cardGame.Log($"Could not play card {spellCard.Name}. Not enough mana.");
            return;
        }

        //TODO - Handle mana costs better
        cardGame.ManaSystem.SpendMana(cardGame, player, Convert.ToInt32(spellCard.ManaCost));

        var effects = ((SpellCardData)spellCard.CurrentCardData).Effects;

        if (targets.Count > 1)
        {
            throw new Exception("Multiple Targets not supported for casting spells yet");
        }


        var typesThatDontTarget = new List<TargetType> { TargetType.None, TargetType.Opponent, TargetType.Self };


        foreach (var effect in effects)
        {
            List<CardGameEntity> entitiesToEffect;
            if (!cardGame.TargetSystem.EffectNeedsTargets(effect))
            {
                entitiesToEffect = GetEntitiesToApplyEffect(cardGame, player, effect);
            }
            else
            {
                entitiesToEffect = targets;
            }

            //Process Effect
            if (effect is DamageEffect)
            {
                foreach (var entity in entitiesToEffect)
                {
                    cardGame.DamageSystem.DealAbilityDamage(cardGame, (DamageEffect)effect, spellCard, entity);
                }
            }
            if (effect is LifeGainEffect)
            {
                foreach (var entity in entitiesToEffect)
                {
                    if (!(entity is Player))
                    {
                        throw new Exception("Error: Only players can be healed via a life gain effect");
                    }
                    cardGame.HealingSystem.HealPlayer(cardGame, (Player)entity, ((LifeGainEffect)effect).Amount);
                }
            }
            if (effect is PumpUnitEffect)
            {
                foreach (var entity in entitiesToEffect)
                {
                    if (!(entity is CardInstance))
                    {
                        throw new Exception("Error: only card instances can be pumped");
                    }

                    cardGame.UnitPumpSystem.PumpUnit(cardGame, (CardInstance)entity, (PumpUnitEffect)effect);
                }
            }
            if (effect is DrawCardEffect)
            {
                var ability = (DrawCardEffect)effect;
                foreach (var entity in entitiesToEffect)
                {
                    for (int i = 0; i < ability.Amount; i++)
                    {
                        if (!(entity is Player))
                        {
                            throw new Exception("Error : only players can draw cards");
                        }
                        cardGame.CardDrawSystem.DrawCard(cardGame, (Player)entity);
                    }
                }
            }
            if (effect is AddManaEffect)
            {
                var ability = (AddManaEffect)effect;

                foreach (var entity in entitiesToEffect)
                {
                    if (!(entity is Player))
                    {
                        throw new Exception("Error : only players can gain man");
                    }
                    cardGame.ManaSystem.AddMana(cardGame, (Player)entity, ability.Amount);
                }
            }
        }
    }
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard)
    {
        if (cardGame.TargetSystem.SpellNeedsTargets(cardGame, player, spellCard))
        {
            throw new Exception("Error: The spell that is being cast needs targets but is calling the CastSpell method without targets... make sure it is using the correct overloaded CastSpell method");
        }
        CastSpell(cardGame, player, spellCard, new List<CardGameEntity>());
    }
}
