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
                unit.ContinuousEffects.Add(continuousEffect);
            }
        }
    }

    public void RemoveContinousEffects(CardGame cardGame, CardInstance effectSource)
    {
        //Remove all continuous effects from a source
        foreach (var unit in cardGame.GetUnitsInPlay())
        {
            unit.ContinuousEffects.RemoveAll(effect => effect.SourceCard == effectSource);
        }
    }

    private List<CardInstance> ApplyFilter (List<CardInstance> originalList, CardFilter filter)
    {

        if (filter == null)
        {
            return originalList;
        }

        var filteredList = new List<CardInstance>();

        if (filter.CreatureType != null)
        {
             filteredList = originalList.Where(ol=>ol.CreatureType == filter.CreatureType).ToList();
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
                    return ApplyFilter(owner.Hand.Cards,filter);
                }
            default:
                {
                    throw new System.Exception($"GetUnitsToApplyAbility :: StaticAbilityEntitiesEffected: {sourceAbility.EntitiesAffectedInfo.EntitiesAffected} is not handled");
                }
        }
    }
}