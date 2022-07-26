using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    void Apply(CardInstance source, StaticAbility sourceAbility);
    void RemoveContinuousEffects(CardInstance effectSource);
}

public class DefaultContinousEffectSystem : IContinuousEffectSystem
{
    private CardGame cardGame;
    public DefaultContinousEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void Apply(CardInstance source, StaticAbility sourceAbility)
    {
        //get the entities that the ability affects.
        var unitsToApply = GetUnitsToApplyAbility(source, sourceAbility);

        foreach (var unit in unitsToApply)
        {
            //Apply the effect if it does not currently exist.
            if (unit.ContinuousEffects.Where(ce => ce.SourceCard == source && ce.SourceAbility == sourceAbility).Count() == 0)
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

    private void RemoveFrom(ContinuousEffect contEffect, CardInstance unit)
    {
        unit.ContinuousEffects.Remove(contEffect);

        var sourceEffect = contEffect.SourceAbility.Effects.First();

        if (sourceEffect is StaticGiveAbilityEffect)
        {
            var giveAbilityEffect = sourceEffect as StaticGiveAbilityEffect;

            //Remove all abilities which have a continous ability component that point to the effect.

            unit.Abilities = unit.Abilities.Where(ab =>
            {
                var components = ab.Components.GetOfType<ContinuousAblityComponent>();

                if (components.Where(comp => comp.SourceEffect == contEffect.SourceEffect).Any())
                {
                    return false;
                }

                return true;

            }).ToList();
        }

        //Pump Effects don't need any additional special processing.
    }

    public void RemoveContinuousEffects(CardInstance effectSource)
    {
        //Remove all continuous effects from a source
        foreach (var unit in cardGame.GetUnitsInPlay())
        {
            var effectsToRemove = unit.ContinuousEffects.Where(effect => effect.SourceCard == effectSource).ToList();

            //
            foreach (var effect in effectsToRemove)
            {
                //todo - we are modifying the list in a loop. this is giving us a might be modified error. Fix this by removing them all at once.
                RemoveFrom(effect, unit);
            }

            var modificationsToKeep = unit.Modifications.Where(mod => mod.StaticInfo.EffectSource != effectSource).ToList();
            unit.Modifications = modificationsToKeep;
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
    public Effect SourceEffect { get; set; }
}