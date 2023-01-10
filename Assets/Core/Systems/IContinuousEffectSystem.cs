using System;
using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    public void ApplyStaticEffects();
    public void RemoveStaticEffects();
}

public class DefaultContinousEffectSystem : CardGameSystem, IContinuousEffectSystem
{

    private List<CardInstance> cardsWithStaticAbilities = null;

    public DefaultContinousEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void ApplyStaticEffects()
    {

        if (cardsWithStaticAbilities == null)
        {
            cardsWithStaticAbilities = cardGame.GetEntities<CardInstance>().Where(c => c.GetAbilitiesAndComponents<StaticAbility>().Any()).ToList();
        }

        foreach (var card in cardsWithStaticAbilities)
        {
            foreach (var sAbility in card.GetAbilitiesAndComponents<StaticAbility>())
            {
                if (cardGame.IsInZone(card, sAbility.ApplyWhenIn))
                {
                    Apply(card, sAbility);
                }
            }
        }
    }

    public void RemoveStaticEffects()
    {
        var cardsInPlay = cardGame.GetCardsInPlay();
        var cardsInGraveyards = cardGame.Players.Select(p => p.DiscardPile).SelectMany(discard => discard.Cards);

        foreach (var card in cardGame.GetEntities())
        {
            var continousEffectsOnUnit = card.ContinuousEffects;

            if (continousEffectsOnUnit.Count == 0)
            {
                continue;
            }

            Func<ContinuousEffect, bool> GetContinuousEffectsToRemove = (ContinuousEffect ce) =>
            {
                var cardsInPlayAndDiscard = cardsInPlay.Concat(cardsInGraveyards);
                var zoneOfSourceCard = cardGame.GetZoneOfCard(ce.SourceCard);
                return zoneOfSourceCard.ZoneType != ce.SourceAbility.ApplyWhenIn;
            };

            var continousEffectsToRemove = continousEffectsOnUnit
                 .Where(ce => GetContinuousEffectsToRemove(ce)
            ).ToList();

            foreach (var effect in continousEffectsToRemove)
            {
                RemoveContinuousEffects(effect.SourceCard);
            };
        }
    }

    private bool HasEffectFromSource(CardGameEntity entityToCheck, CardInstance source, StaticAbility sourceAbility)
    {
        return entityToCheck.ContinuousEffects.Where(ce => ce.SourceCard == source && ce.SourceAbility == sourceAbility).Any();
    }

    private void Apply(CardInstance source, StaticAbility sourceAbility)
    {
        //get the entities that the ability affects.
        var unitsToApply = GetUnitsToApplyAbility(source, sourceAbility);

        foreach (var unit in unitsToApply)
        {
            if (!HasEffectFromSource(unit, source, sourceAbility))
            {
                var continuousEffect = new ContinuousEffect
                {
                    SourceCard = source,
                    SourceAbility = sourceAbility,
                    SourceEffect = sourceAbility.Effects.First()
                };

                //This way each effect can be responsible for how it needs to apply and remove from the uni.
                ApplyTo(continuousEffect, unit);
            }
        }
    }

    private void ApplyTo(ContinuousEffect effect, CardGameEntity entity)
    {
        //entity.ContinuousEffects.Add(effect);
        //Note this makes it so that static abilities with multiple effects will not work.
        var sourceEffect = effect.SourceAbility.Effects.First();

        if (effect.SourceAbility.Effects.Count() > 1)
        {
            cardGame.Log("Continuous effects are only supported for 1 effect at a time... if you are seeing this message it is likely you have a bug and it is time to update");
        }

        sourceEffect.Apply(cardGame, cardGame.GetOwnerOfCard(effect.SourceCard), effect.SourceCard, new List<CardGameEntity> { entity });
    }

    /// <summary>
    /// Remove all continous effects form all units in play where the effect came from a specific source.
    /// </summary>
    /// <param name="sourceCard"></param>
    private void RemoveContinuousEffects(CardInstance sourceCard)
    {
        foreach (var card in cardGame.GetEntities())
        {
            //Remove all continuous effects
            card.ContinuousEffects = card.ContinuousEffects.Where(ce => ce.SourceCard != sourceCard).ToList();

            //Remove any modifications that came from the source
            var modificationsToKeep = card.Modifications.Where(mod => mod.StaticInfo.SourceCard != sourceCard).ToList();
            card.Modifications = modificationsToKeep;

            //Remove any abilities that come from the source
            var cardInstance = card as CardInstance;

            if (cardInstance != null)
            {
                cardInstance.Abilities = cardInstance.Abilities.Where(ab =>
                {
                    if (!ab.Components.Any())
                    {
                        return false;
                    }
                    var components = ab.Components.GetOfType<ContinuousAblityComponent>();

                    if (components.Where(comp => comp.SourceCard == sourceCard).Any())
                    {
                        return false;
                    }

                    return true;

                }).ToList();
            }
        }
    }

    private List<CardInstance> ApplyFilter(List<CardInstance> originalList, CardFilter filter)
    {

        if (filter == null)
        {
            return originalList;
        }

        var filteredList = new List<CardInstance>();

        if (filter.CreatureType != null)
        {
            filteredList = originalList.Where(ol => ol.CreatureType == filter.CreatureType).ToList();
        }

        return filteredList;
    }

    //TODO - fix this.
    private List<CardGameEntity> GetUnitsToApplyAbility(CardInstance source, StaticAbility sourceAbility)
    {
        //TODO - only one effect per static ability?

        var effect = sourceAbility.Effects.First();
        var targetType = effect.TargetType;
        var filter = effect.Filter;

        switch (targetType)
        {
            case TargetType.PlayerSelf:
                {
                    return new List<CardGameEntity> { cardGame.GetOwnerOfCard(source) };
                }
            case TargetType.UnitSelf:
                return new List<CardGameEntity> { source };
            case TargetType.OtherCreaturesYouControl:
                {
                    var owner = cardGame.GetOwnerOfCard((CardInstance)source);
                    return ApplyFilter(
                        cardGame.GetUnitsInPlay().Where(u => u.OwnerId == owner.PlayerId && u.EntityId != source.EntityId).ToList(),
                        filter).Cast<CardGameEntity>().ToList();
                }
            case TargetType.CardsInHand:
                {
                    var owner = cardGame.GetOwnerOfCard((CardInstance)source);
                    return ApplyFilter(owner.Hand.Cards, filter).Cast<CardGameEntity>().ToList();
                }
            default:
                {
                    throw new System.Exception($"GetUnitsToApplyAbility :: StaticAbilityEntitiesEffected: {effect.TargetType} is not handled");
                }
        }
    }
}


public class ContinuousAblityComponent : AbilityComponent
{
    public CardInstance SourceCard { get; set; }
    public StaticAbility SourceAbility { get; set; }
    public Effect SourceEffect { get; set; }
}