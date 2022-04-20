public interface IContinuousEffectSystem
{
    void Apply(CardGame cardGame, CardGameEntity source, StaticAbility sourceAbility);
}


public class DefaultContinousEffectSystem : IContinuousEffectSystem
{
    public void Apply(CardGame cardGame, CardGameEntity source, StaticAbility sourceAbility)
    {
        //get the entities that the ability affects.
        var entitiesToApply = GetEntitiesToApplyAbility(cardGame, source, sourceAbility);

        foreach(var entity in entitiesToApply)
        {
            //if there is no applied effect that corresponds to the sourceAbility, then add one.
            //todo - apply effects to unit.
        }
    }
}