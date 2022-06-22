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
            entitiesToEffect = cardGame.TargetSystem.GetEntitiesToApplyEffect(cardGame, player, source, effect);
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
                cardGame.ManaSystem.AddEssence(cardGame, (Player)entity, ability.ManaType, ability.Amount);
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
                cardGame.ManaSystem.AddTemporaryManaAndEssence(cardGame, (Player)entity, ability.ManaType, ability.Amount);
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
                var cardDrawn = cardGame.CardDrawSystem.DrawCard(cardGame, player);
                cardGame.DamageSystem.DealDamage(cardGame, source, player, new ManaAndEssence(cardDrawn.ManaCost).Mana);
                cardGame.Log($@"Dark confidant effect : Drawn a card and you have lost {new ManaAndEssence(cardDrawn.ManaCost).Mana} life.");
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
                cardGame.SacrificeSystem.SacrificeUnit(cardGame, player, card);
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
                cardGame.DestroySystem.DestroyUnit(cardGame, source, card);
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
            cardGame.UnitPumpSystem.PumpUnit(cardGame, source, new PumpUnitEffect { Power = 2 * goblinsInPlay, Toughness = 0 });
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

            cardGame.CardDrawSystem.GrabRandomCardFromDeck(cardGame, player, fromDeckEffect.Filter);


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
                    cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.Hand);
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
                    cardGame.DiscardSystem.Discard(cardGame, player, card);
                }
            }
            else
            {
                foreach (var card in validCardsToDiscard.Randomize().Take(discardCardEffect.Amount))
                {
                    cardGame.DiscardSystem.Discard(cardGame, player, card);
                }
            }
        }

        if (effect is CompoundEffect)
        {
            var compoundEffect = effect as CompoundEffect;
            this.ApplyEffects(cardGame, player, source, compoundEffect.Effects, targets);
        }
    }
    public void ApplyEffects(CardGame cardGame, Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets)
    {
        foreach (var effect in effects)
        {
            ApplyEffect(cardGame, player, source, effect, targets);
        }
    }
}
