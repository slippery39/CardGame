using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    void Apply(CardGame cardGame, CardInstance source, StaticAbility sourceAbility);
    void RemoveContinousEffects(CardGame cardGame, CardInstance effectSource);
}

public class DefaultContinousEffectSystem : IContinuousEffectSystem
{
    public void Apply(CardGame cardGame, CardInstance source, StaticAbility sourceAbility)
    {
        //get the entities that the ability affects.
        var unitsToApply = GetUnitsToApplyAbility(cardGame, source, sourceAbility);

        foreach (var unit in unitsToApply)
        {
            //Apply the effect if it does not currently exist.
            if (unit.ContinuousEffects.Where(ce => ce.SourceCard == source && ce.SourceAbility == sourceAbility).Count() == 0)
            {
                var continuousEffect = new ContinuousEffect
                {
                    SourceCard = source,
                    SourceAbility = sourceAbility
                };

                //This way each effect can be responsible for how it needs to apply and remove from the uni.
                ApplyTo(continuousEffect, unit);
            }
        }
    }

    private void ApplyTo(ContinuousEffect effect, CardInstance unit)
    {
        unit.ContinuousEffects.Add(effect);

        var sourceEffect = effect.SourceAbility.Effects.First();

        if (sourceEffect is StaticGiveAbilityEffect)
        {
            var giveAbilityEffect = sourceEffect as StaticGiveAbilityEffect;

            var abilityToGive = giveAbilityEffect.Ability;

            abilityToGive.Components.Add(new ContinuousAblityComponent
            {
                SourceEffect = effect
            });

            //Warning, we are doing a shallow clone here... may run into issues if we need a deep clone done.
            unit.Abilities.Add(giveAbilityEffect.Ability.Clone());
        }

        //Pump Effects don't need any additional special processing.
    }

    private void RemoveFrom(ContinuousEffect effect, CardInstance unit)
    {
        unit.ContinuousEffects.Remove(effect);

            var sourceEffect = effect.SourceAbility.Effects.First();

        if (sourceEffect is StaticGiveAbilityEffect)
        {
            var giveAbilityEffect = sourceEffect as StaticGiveAbilityEffect;

            //Remove all abilities which have a continous ability component that point to the effect.

            unit.Abilities = unit.Abilities.Where(ab =>
            {
                var components = ab.Components.Where(comp => comp is ContinuousAblityComponent).Cast<ContinuousAblityComponent>();

                if (components.Where(comp => comp.SourceEffect == effect).Any())
                {
                    return false;
                }

                return true;

            }).ToList();

            var i = 0;
        }

        //Pump Effects don't need any additional special processing.
    }

    public void RemoveContinousEffects(CardGame cardGame, CardInstance effectSource)
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
    private List<CardInstance> GetUnitsToApplyAbility(CardGame cardGame, CardInstance source, StaticAbility sourceAbility)
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
    public ContinuousEffect SourceEffect { get; set; }
}