using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IEffectsProcessor
{
    void ApplyEffect(Player player, CardInstance source, Effect effects, List<CardGameEntity> targets);
    void ApplyEffects(Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets);
}


public class DefaultEffectsProcessor : IEffectsProcessor
{
    private CardGame cardGame;

    public DefaultEffectsProcessor(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void ApplyEffect(Player player, CardInstance source, Effect effect, List<CardGameEntity> targets)
    {


        List<CardGameEntity> entitiesToEffect;
        if (!cardGame.TargetSystem.EffectNeedsTargets(effect))
        {
            entitiesToEffect = cardGame.TargetSystem.GetEntitiesToApplyEffect(player, source, effect);
        }
        else
        {
            entitiesToEffect = targets;
        }

        //TODO - Change this to Switch Case or... place all the effects processing inside the effects themselves?

        //Process Effect
        if (effect is DamageEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                cardGame.DamageSystem.DealAbilityDamage((DamageEffect)effect, source, entity);
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
                cardGame.HealingSystem.HealPlayer((Player)entity, ((LifeGainEffect)effect).Amount);
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

                cardGame.UnitPumpSystem.PumpUnit((CardInstance)entity, (PumpUnitEffect)effect);
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
                    cardGame.CardDrawSystem.DrawCard((Player)entity);
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
                cardGame.ManaSystem.AddMana((Player)entity, ability.Amount);
                cardGame.ManaSystem.AddEssence((Player)entity, ability.ManaType, ability.Amount);
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
                cardGame.ManaSystem.AddTemporaryManaAndEssence((Player)entity, ability.ManaType, ability.Amount);
            }
        }
        if (effect is DarkConfidantEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is Player))
                {
                    throw new Exception("Error : only players can be effected with the dark confidant effect");
                }
                var cardDrawn = cardGame.CardDrawSystem.DrawCard(player);
                cardGame.DamageSystem.DealDamage(source, player, new Mana(cardDrawn.ManaCost).ColorlessMana);
                cardGame.Log($@"Dark confidant effect : Drawn a card and you have lost {new Mana(cardDrawn.ManaCost).ColorlessMana} life.");
            }
        }
        if (effect is SacrificeSelfEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with the sacrifice self effect");
                }
                var card = (CardInstance)entity;
                cardGame.SacrificeSystem.SacrificeUnit(player, card);
            }
        }
        if (effect is TransformEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with the sacrifice self effect");
                }

                var card = (CardInstance)entity;
                var transFormEffect = (TransformEffect)effect;
                card.CurrentCardData = transFormEffect.TransformData.Clone();
            }
        }
        if (effect is DestroyEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with the sacrifice self effect");
                }

                var card = (CardInstance)entity;
                cardGame.DestroySystem.DestroyUnit(source, card);
            }
        }
        if (effect is AddTempAbilityEffect)
        {
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with the sacrifice self effect");
                }

                var card = (CardInstance)entity;
                var tempAbilityEffect = effect as AddTempAbilityEffect;
                //note, ideally the temp ability would be cloned from the AddTempAbilityEffect...
                card.Abilities.Add(tempAbilityEffect.TempAbility);
            }
        }

        if (effect is CreateTokenEffect)
        {
            var tokenEffect = effect as CreateTokenEffect;
            //put in open lanes. 

            var emptyLanes = player.GetEmptyLanes();
            for (var i = 0; i < tokenEffect.AmountOfTokens; i++)
            {
                var emptyLane = player.GetEmptyLanes().FirstOrDefault();
                if (emptyLane != null)
                {
                    cardGame.AddCardToGame(player, tokenEffect.TokenData, emptyLane);
                }
            }
        }
        if (effect is GoblinPiledriverEffect)
        {
            var goblinsInPlay = player.Lanes.Where(l => l.IsEmpty() == false).Select(l => l.UnitInLane).Where(u => u != source && u.CreatureType == "Goblin").Count();
            cardGame.UnitPumpSystem.PumpUnit(source, new PumpUnitEffect { Power = 2 * goblinsInPlay, Toughness = 0 });
        }

        if (effect is GetRandomCardFromDeckEffect)
        {
            var fromDeckEffect = effect as GetRandomCardFromDeckEffect;



            var validCardsToGet = player.Deck.Cards.Where(card =>
           {

               if (fromDeckEffect.Filter.CreatureType != null)
               {
                   return card.CreatureType == fromDeckEffect.Filter.CreatureType;
               }
               return true;
           });

            cardGame.CardDrawSystem.GrabRandomCardFromDeck(player, fromDeckEffect.Filter);


        }

        if (effect is GrabFromTopOfDeckEffect)
        {
            var topOfDeckEffect = effect as GrabFromTopOfDeckEffect;

            var cardsFromTopOfDeck = player.Deck.Cards.Take(topOfDeckEffect.CardsToLookAt).ToList();


            var validCardsToGet = player.Deck.Cards.Take(topOfDeckEffect.CardsToLookAt)
                .Where(card =>
                {

                    if (topOfDeckEffect.Filter.CreatureType != null)
                    {
                        return card.CreatureType == topOfDeckEffect.Filter.CreatureType;
                    }
                    return true;
                });


            if (validCardsToGet.Any())
            {
                validCardsToGet.ToList().ForEach(card =>
                {
                    cardGame.ZoneChangeSystem.MoveToZone(card, player.Hand);
                });
            }
        }

        if (effect is DiscardCardEffect)
        {
            var discardCardEffect = effect as DiscardCardEffect;
            //This does not happen here unless its a random discard.. it needs to happen at the choice level...
            return;

            var validCardsToDiscard = player.Hand.Cards;

            if (validCardsToDiscard.Count() < discardCardEffect.Amount)
            {
                foreach (var card in validCardsToDiscard)
                {
                    cardGame.DiscardSystem.Discard(player, card);
                }
            }
            else
            {
                foreach (var card in validCardsToDiscard.Randomize().Take(discardCardEffect.Amount))
                {
                    cardGame.DiscardSystem.Discard(player, card);
                }
            }
        }

        if (effect is CompoundEffect)
        {
            var compoundEffect = effect as CompoundEffect;
            this.ApplyEffects(player, source, compoundEffect.Effects, targets);
        }

        if (effect is SwitchPowerToughnessEffect)
        {
            var powerToughnessEffect = effect as SwitchPowerToughnessEffect;

            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with the SwitchPowerToughness effect");
                }

                var card = (CardInstance)entity;
                var mod = new ModSwitchPowerandToughness();

                //If they already have a switch power and toughness effect then it should cancel out (we will remove the existing one)
                if (card.Modifications.GetOfType<ModSwitchPowerandToughness>().Any())
                {

                    card.Modifications = card.Modifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
                }
                else
                {
                    cardGame.ModificationsSystem.AddModification(card, mod);
                }
            }
        }
        if (effect is PumpPowerByNumberOfArtifactsEffect)
        {
            var pumpPowerByArtifacts = effect as PumpPowerByNumberOfArtifactsEffect;
            foreach (var entity in entitiesToEffect)
            {
                if (!(entity is CardInstance))
                {
                    throw new Exception("Error : only units can be effected with PumpPowerByNumberOfArtifactsEffect");
                }

                var card = (CardInstance)entity;

                Func<CardGame, CardInstance, int, int> powerMod = (c, ci, o) =>
                {
                    return o + pumpPowerByArtifacts.CountArtifacts(c, c.GetOwnerOfCard(card));
                };

                //We will need a new modification.
                var mod = new ModAddXToPowerToughness(powerMod, null);
                mod.OneTurnOnly = true;
                cardGame.ModificationsSystem.AddModification(card, mod);
            }
        }
    }
    public void ApplyEffects(Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets)
    {
        foreach (var effect in effects)
        {
            ApplyEffect(player, source, effect, targets);
        }
    }
}
