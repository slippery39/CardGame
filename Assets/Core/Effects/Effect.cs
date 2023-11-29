using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Effect
{
    public abstract string RulesText { get; }

    [Obsolete("Being replaced by TargetInfo")]
    public virtual TargetType TargetType { get; set; }
    public virtual TargetInfo TargetInfo { get; set; }

    public abstract void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply);

    //Temporary placing this here while we refactor our TargetSystem.
    [Obsolete("Use TargetInfo.NeedsTargets from now on instead")]
    private static readonly List<TargetType> TypesThatDontNeedTargets = new List<TargetType> { TargetType.OpenLane, TargetType.None};

    public bool NeedsTargets()
    {
        if (TargetInfo != null)
        {
            return TargetInfo.NeedsTargets;
        }
        else
        {
            return !TypesThatDontNeedTargets.Contains(TargetType);
        }
    }

    public IEnumerable<CardGameEntity> GetEffectTargets(CardGame cardGame, Player player, CardGameEntity sourceOfEffects)
    {
        if (TargetInfo != null)
        {
            //New way of getting effect targets
            return TargetInfo.GetTargets(cardGame, player, sourceOfEffects);
        }
        else
        {
            //Note that if we reach this point, it means that we have some TargetType that isn't properly being considered.
            //The old code would return an Empty List, but we shouldn't even need to do that anymore
            //Regardless, Gempalm Incinerator is breaking without this, so we are adding it here until we figure out the
            //correct path forward
            return new List<CardGameEntity>() { };
            //throw new Exception("Something went wrong, we should never reach this point in GetEffectTargets(). TargetInfo is null");
        }
    }

    public IEnumerable<CardGameEntity> GetEntitiesToApplyEffect(CardGame cardGame, Player player, CardGameEntity effectSource)
    {
        if (TargetInfo != null)
        {
            return TargetInfo.GetTargets(cardGame, player, effectSource);
        }
        else
        {
            return GetEntitiesToApplyEffectOld(cardGame, player, effectSource);
        }
    }

    /// <summary>
    /// Old way of getting figuring out which entity will be effected via the TargetType enum
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <param name="effectSource"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [Obsolete("Use TargetInfo.GetTargets() from now on instead")]
    private IEnumerable<CardGameEntity> GetEntitiesToApplyEffectOld(CardGame cardGame, Player player, CardGameEntity effectSource)
    {
        switch (TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {TargetType}");
        }
    }
}


