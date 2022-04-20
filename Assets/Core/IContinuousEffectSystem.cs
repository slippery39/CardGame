using System.Collections.Generic;
using System.Linq;

public interface IContinuousEffectSystem
{
    void Apply(CardGame cardGame, CardInstance source, StaticAbility sourceAbility);
}


public class DefaultContinousEffectSystem : IContinuousEffectSystem
{
    public void Apply(CardGame cardGame, CardInstance source, StaticAbility sourceAbility)
    {
        //get the entities that the ability affects.
        var unitsToApply = GetUnitsToApplyAbility(cardGame, source, sourceAbility);

        foreach (var unit in unitsToApply)
        {
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
    private List<CardInstance> GetUnitsToApplyAbility(CardGame cardGame, CardInstance source, StaticAbility sourceAbility)
    {
        if (sourceAbility.EffectType == StaticAbilityType.OtherCreaturesYouControl)
        {
            var owner = cardGame.GetOwnerOfCard((CardInstance)source);
            return cardGame.GetUnitsInPlay().Where(u => u.OwnerId == owner.EntityId && u.EntityId!= source.EntityId).ToList();
        }

        throw new System.Exception("Invalid Static Effect Type in call to get EntitiesToApplyAbility");
    }
}