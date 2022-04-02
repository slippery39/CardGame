using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEffectsProcessor
{
    void ApplyEffect(CardGame cardGame, Player player, CardInstance source, Effect effects, List<CardGameEntity> targets);
    void ApplyEffects(CardGame cardGame, Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets);
}


public class DefaultEffectsProcessor : IEffectsProcessor
{
    public void ApplyEffect(CardGame cardGame, Player player, CardInstance source, Effect effect, List<CardGameEntity> targets)
    {
        List<CardGameEntity> entitiesToEffect;
        if (!cardGame.TargetSystem.EffectNeedsTargets(effect))
        {
            entitiesToEffect = cardGame.TargetSystem.GetEntitiesToApplyEffect(cardGame, player, effect);
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
                cardGame.DamageSystem.DealAbilityDamage(cardGame, (DamageEffect)effect, source, entity);
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
        if (effect is AddTempManaEffect)
        {
            var ability = (AddTempManaEffect)effect;

            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is Player))
                {
                    throw new Exception("Error : only players can gain man");
                }
                cardGame.ManaSystem.AddTemporaryMana(cardGame, (Player)entity, ability.Amount);
            }
        }
        if (effect is DarkConfidantEffect)
        {
            foreach(var entity in entitiesToEffect)
            {
                if (!(entity is Player))
                {
                    throw new Exception("Error : only players can be effected with the dark confidant effect");
                }
                var cardDrawn = cardGame.CardDrawSystem.DrawCard(cardGame, player);
                cardGame.DamageSystem.DealDamage(cardGame, source, player, Convert.ToInt32(cardDrawn.ManaCost));
                cardGame.Log($@"Dark confidant effect : Drawn a card and you have lost {Convert.ToInt32(cardDrawn.ManaCost)} life.");                
            }
        }
    }
    public void ApplyEffects(CardGame cardGame, Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets)
    {
        foreach(var effect in effects)
        {
            ApplyEffect(cardGame, player, source, effect, targets);
        }
    }
}
