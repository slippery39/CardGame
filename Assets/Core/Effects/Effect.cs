using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Effect
{
    public abstract string RulesText { get; }

    [Obsolete("Being replaced by TargetInfo")]
    public virtual TargetType TargetType { get; set; }
    [Obsolete("Being replaced by TargetInfo")]

    private CardFilter _filter;
    public CardFilter Filter
    {
        //POSSIBLE CLONING ISSUE HERE?
        get
        {
            if (TargetInfo == null){
                return _filter;
            }
            else
            {
                return TargetInfo.TargetFilter;
            }
        }
        set { _filter = value; }
    }

    public virtual TargetInfo TargetInfo { get; set; }

    public abstract void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply);

    //TODO - this is not set up to work with TargetInfo yet, should probably actually be moved inside target info
    public string CompileRulesText()
    {
        if (TargetInfo != null)
        {
            return "Rules Text Needs to be updated for TargetInfo based rules";
        }

        string effectText = RulesText.Replace("#effectTargetType#", TargetTypeHelper.TargetTypeToRulesText(TargetType));

        var unitType = "unit";
        if (Filter != null && Filter.RulesTextString() != "")
        {
            unitType = Filter.RulesTextString();
        }

        effectText = effectText.Replace("#unitType#", unitType);

        return effectText;
    }

    //Temporary placing this here while we refactor our TargetSystem.
    [Obsolete("Use TargetInfo.NeedsTargets from now on instead")]
    private static readonly List<TargetType> TypesThatDontNeedTargets = new List<TargetType> { TargetType.PlayerSelf, TargetType.RandomOpponentOrUnits, TargetType.OpenLane, TargetType.UnitSelf, TargetType.Opponent, TargetType.None, TargetType.RandomOurUnits };

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

    private IEnumerable<CardGameEntity> GetPlayers(CardGame cardGame)
    {
        return cardGame.Players.Cast<CardGameEntity>().ToList();
    }

    private IEnumerable<CardGameEntity> GetUnits(CardGame cardGame)
    {
        var units =
        cardGame
        .GetEntities<Lane>()
        .Where(lane => !lane.IsEmpty())
        .Select(lane => lane.UnitInLane);
        return units.Cast<CardGameEntity>();
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
            //Old way of grabbing effect targets.. Obsolete but keeping here for compatibility.
            return GetEffectTargetsOld(cardGame);
        }
    }

    /// <summary>
    /// Old way of figuring out the targets of an effect via the TargetType enum
    /// </summary>
    /// <param name="cardGame"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    [Obsolete("Use TargetInfo.GetTargets() from now on instead")]
    private IEnumerable<CardGameEntity> GetEffectTargetsOld(CardGame cardGame)
    {
        if (!NeedsTargets())
        {
            return new List<CardGameEntity>();
        }

        switch (TargetType)
        {
            case TargetType.TargetUnitsOrPlayers:
                {
                    return GetPlayers(cardGame).Concat(GetUnits(cardGame));
                }
            default:
                {
                    throw new Exception($"Attempted to process invalid or unimplemented target type for GetEffectTargets() : ${TargetType}");
                }
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
            case TargetType.PlayerSelf:
                {
                    return new List<CardGameEntity> { player };
                }
            case TargetType.UnitSelf:
                return new List<CardGameEntity>() { effectSource };
            case TargetType.Opponent:
                return cardGame.Players.Where(p => p.EntityId != player.EntityId).Cast<CardGameEntity>().ToList();
            case TargetType.RandomOurUnits:
                var ourUnits = player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Randomize();
                var filtered = CardFilter.ApplyFilter(ourUnits.ToList(), Filter);
                return new List<CardGameEntity> { filtered.FirstOrDefault() };
            case TargetType.RandomOpponentOrUnits:
                var opponent = cardGame.Players.Find(p => p.EntityId != player.EntityId);
                var things = new List<CardGameEntity> { opponent };

                var everything = things.Union(opponent.GetUnitsInPlay());

                if (!everything.Any())
                {
                    return new List<CardGameEntity> { };
                }
                return new List<CardGameEntity> { everything.Randomize().First() };
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {TargetType}");
        }
    }
}


