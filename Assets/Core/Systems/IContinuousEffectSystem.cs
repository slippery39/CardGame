﻿using System;
using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    public void ApplyStaticEffects();
    /// <summary>
    /// Removes static effects where the card applying the effect is no longer
    /// in the valid zone (ex. if a goblin warchief leaves play, haste should be removed from all goblins)
    /// </summary>
    public void RemoveStaticEffects();
    public void RemoveAllStaticEffects();

}

public class DefaultContinousEffectSystem : CardGameSystem, IContinuousEffectSystem
{

    private List<CardInstance> cardsWithStaticAbilities = null;

    public DefaultContinousEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    /// <summary>
    /// Removes all static effects from all game entities.
    /// </summary>
    public void RemoveAllStaticEffects()
    {
        foreach (var entity in cardGame.GetEntities())
        {
            //Things to remove : 
            //Remove continuous effects
            //Remove abilities with a continous ability component on it
            //Remove modifications with a static info.source card !=null
            entity.ContinuousEffects = new List<ContinuousEffect>();

            var cardInstance = entity as CardInstance;
            if (cardInstance != null)
            {
                cardInstance.Abilities = cardInstance.Abilities.Where(a => a.GetComponent<ContinuousAblityComponent>() == null).ToList();
                cardInstance.Modifications = cardInstance.Modifications.Where(mod => mod.StaticInfo == null || mod.StaticInfo.SourceCard == null).ToList();
            }
        }
    }

    public void ApplyStaticEffects()
    {
        if (cardsWithStaticAbilities == null)
        {
            cardsWithStaticAbilities = cardGame.GetEntities<CardInstance>().Where(c => c.GetAbilitiesAndComponents<StaticAbility>().Any()).ToList();
        }

        foreach (var card in cardsWithStaticAbilities)
        {
            var abilitiesAndComponents = card.GetAbilitiesAndComponents<StaticAbility>().ToList();
            foreach (var sAbility in abilitiesAndComponents)
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
        var contEffects = cardGame.GetEntities().Select(c => c.ContinuousEffects);
        foreach (var contEffect in contEffects)
        {
            if (contEffect.Count == 0)
            {
                continue;
            }

            Func<ContinuousEffect, bool> GetContinuousEffectsToRemove = (ContinuousEffect ce) =>
            {
                var zoneOfSourceCard = cardGame.GetZoneOfCard(ce.SourceCard);
                return zoneOfSourceCard.ZoneType != ce.SourceAbility.ApplyWhenIn;
            };

            var continousEffectsToRemove = contEffect
            .Where(ce => GetContinuousEffectsToRemove(ce))
            .ToList();

            foreach (var effect in continousEffectsToRemove)
            {
                RemoveContinuousEffectsFromSource(effect.SourceCard);
            }
        }
    }

    private bool HasEffectFromSource(CardGameEntity entityToCheck, CardInstance source, StaticAbility sourceAbility)
    {
        //Note - Ideally our abilities and our effects would also have unique id's so that we can compare if they are the same
        //by id and not necessarily compare them by reference. 
        return entityToCheck.ContinuousEffects.Exists(ce => ce.SourceCard.EntityId == source.EntityId && ce.SourceAbility.RulesText == sourceAbility.RulesText);
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
                    SourceEffect = sourceAbility.Effects[0]
                };

                //This way each effect can be responsible for how it needs to apply and remove from the uni.
                ApplyTo(continuousEffect, unit);
            }
        }
    }

    private void ApplyTo(ContinuousEffect effect, CardGameEntity entity)
    {
        //We still need this in order for duplicate effects not to be applied, but we should be depreciating continous effects out.
        //Instead the actual Modification or Ability will have info on whether or not it is a continous modification or continuous ability.
        //TODO - remove continuous effects
        entity.ContinuousEffects.Add(effect);
        //Note this makes it so that static abilities with multiple effects will not work.
        var sourceEffect = effect.SourceAbility.Effects[0];

        if (effect.SourceAbility.Effects.Count > 1)
        {
            cardGame.Log("Continuous effects are only supported for 1 effect at a time... if you are seeing this message it is likely you have a bug and it is time to update");
        }

        sourceEffect.Apply(cardGame, cardGame.GetOwnerOfCard(effect.SourceCard), effect.SourceCard, new List<CardGameEntity> { entity });
    }

    /// <summary>
    /// Remove all continous effects form all units in play where the effect came from a specific source.
    /// </summary>
    /// <param name="sourceCard"></param>
    private void RemoveContinuousEffectsFromSource(CardInstance sourceCard)
    {
        foreach (var card in cardGame.GetEntities())
        {
            //Remove all continuous effects
            card.ContinuousEffects = card.ContinuousEffects.Where(ce => ce.SourceCard != sourceCard).ToList();

            //Remove any modifications that came from the source
            var modificationsToKeep = card.Modifications.Where(mod => mod.StaticInfo == null || (mod.StaticInfo.SourceCard.EntityId != sourceCard.EntityId)).ToList();
            card.Modifications = modificationsToKeep;

            //Remove any abilities that come from the source
            var cardInstance = card as CardInstance;

            if (cardInstance != null)
            {
                cardInstance.Abilities = cardInstance.Abilities.Where(ab =>
                {
                    var components = ab.Components.GetOfType<ContinuousAblityComponent>();

                    if (components.Any(comp => comp.SourceCard == sourceCard))
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

        if (effect.TargetInfo != null)
        {
            //Note if we are having issues here, it might be because we aren't handling "other" creatures you control right now.
            return effect.TargetInfo.GetTargets(cardGame, source.GetOwner(), source).ToList();
        }

        switch (targetType)
        {
            case TargetType.PlayerSelf:
                {
                    return new List<CardGameEntity> { cardGame.GetOwnerOfCard(source) };
                }
            case TargetType.UnitSelf:
                return new List<CardGameEntity> { source };
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