using System;
using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    public void ApplyStaticEffects();
    public void RemoveStaticEffects();
}

public class DefaultContinousEffectSystem : IContinuousEffectSystem
{
    private CardGame cardGame;
    public DefaultContinousEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void ApplyStaticEffects()
    {
        var cardsInPlay = cardGame.GetCardsInPlay();
        var cardsInGraveyards = cardGame.Players.Select(p => p.DiscardPile).SelectMany(discard => discard.Cards);

        foreach (var card in cardsInPlay.Concat(cardsInGraveyards))
        {
            var unitStaticAbilities = card.GetAbilitiesAndComponents<StaticAbility>();
            if (unitStaticAbilities.Count > 0)
            {
                foreach (var sAbility in unitStaticAbilities)
                {
                    if (cardGame.IsInZone(card, sAbility.ApplyWhenIn))
                    {
                        Apply(card, sAbility);
                    }
                }
            }
        }
    }

    public void RemoveStaticEffects()
    {
        var cardsInPlay = cardGame.GetCardsInPlay();
        var cardsInGraveyards = cardGame.Players.Select(p => p.DiscardPile).SelectMany(discard => discard.Cards);

        foreach (var card in cardGame.GetEntities<CardInstance>())
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

    private bool HasEffectFromSource(CardInstance cardToCheck, CardInstance source, StaticAbility sourceAbility)
    {

        return cardToCheck.ContinuousEffects.Where(ce => ce.SourceCard == source && ce.SourceAbility == sourceAbility).Any();

        /*
         * Not sure if we need this code or not?
         */

        //We check if it has a modification or an added ability from the static ability.
        if (cardToCheck.Modifications.Where(m => m.StaticInfo.SourceCard == source && m.StaticInfo.SourceAbility == sourceAbility).Count() > 0)
        {
            return true;
        }


        if (cardToCheck.Abilities.Where(ab =>
        {

            var comp = ab.GetComponent<ContinuousAblityComponent>();

            if (comp == null)
            {
                return false;
            }
            return comp.SourceCard == source && comp.SourceAbility == sourceAbility;
        }
        )
        .Count() > 0
        )
        {
            return true;
        }

        return false;
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

    private void ApplyTo(ContinuousEffect effect, CardInstance unit)
    {
        unit.ContinuousEffects.Add(effect);
        //Note this makes it so that static abilities with multiple effects will not work.
        var sourceEffect = effect.SourceAbility.Effects.First();

        if (effect.SourceAbility.Effects.Count() > 1)
        {
            cardGame.Log("Continuous effects are only supported for 1 effect at a time... if you are seeing this message it is likely you have a bug and it is time to update");
        }

        sourceEffect.Apply(cardGame, cardGame.GetOwnerOfCard(effect.SourceCard), effect.SourceCard, new List<CardGameEntity> { unit });
    }

    /// <summary>
    /// Remove all continous effects form all units in play where the effect came from a specific source.
    /// </summary>
    /// <param name="sourceCard"></param>
    private void RemoveContinuousEffects(CardInstance sourceCard)
    {
        foreach (var card in cardGame.GetEntities<CardInstance>())
        {
            //Remove all continuous effects
            card.ContinuousEffects = card.ContinuousEffects.Where(ce => ce.SourceCard != sourceCard).ToList();

            //Remove any modifications that came from the source
            var modificationsToKeep = card.Modifications.Where(mod => mod.StaticInfo.SourceCard != sourceCard).ToList();
            card.Modifications = modificationsToKeep;

            //Remove any abilities that come from the source
            card.Abilities = card.Abilities.Where(ab =>
            {
                var components = ab.Components.GetOfType<ContinuousAblityComponent>();

                if (components.Where(comp => comp.SourceCard == sourceCard).Any())
                {
                    return false;
                }

                return true;

            }).ToList();
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
    private List<CardInstance> GetUnitsToApplyAbility(CardInstance source, StaticAbility sourceAbility)
    {
        var filter = sourceAbility.EntitiesAffectedInfo.Filter;
        var entitiesAffected = sourceAbility.EntitiesAffectedInfo.EntitiesAffected;

        switch (entitiesAffected)
        {
            case EntityType.Self:
                return new List<CardInstance> { source };
            case EntityType.OtherCreaturesYouControl:
                {
                    var owner = cardGame.GetOwnerOfCard((CardInstance)source);
                    return ApplyFilter(
                        cardGame.GetUnitsInPlay().Where(u => u.OwnerId == owner.PlayerId && u.EntityId != source.EntityId).ToList(),
                        filter);
                }
            case EntityType.CardsInHand:
                {
                    var owner = cardGame.GetOwnerOfCard((CardInstance)source);
                    return ApplyFilter(owner.Hand.Cards, filter);
                }
            default:
                {
                    throw new System.Exception($"GetUnitsToApplyAbility :: StaticAbilityEntitiesEffected: {sourceAbility.EntitiesAffectedInfo.EntitiesAffected} is not handled");
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