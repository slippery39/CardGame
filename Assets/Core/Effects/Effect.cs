using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Effect
{
    public abstract string RulesText { get; }

    [Obsolete("Being replaced by TargetInfo")]
    public virtual TargetType TargetType { get; set; }
    [Obsolete("Being replaced by TargetInfo")]
    public CardFilter Filter { get; set; }

    public TargetInfo TargetInfo { get; set; }

    public abstract void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply);

    public static string CompileRulesText(Effect effect)
    {
        var effectText = effect.RulesText;

        effectText = effectText.Replace("#effectTargetType#", TargetTypeHelper.TargetTypeToRulesText(effect.TargetType));

        var unitType = "unit";
        if (effect.Filter != null && effect.Filter.RulesTextString() != "")
        {
            unitType = effect.Filter.RulesTextString();
        }

        effectText = effectText.Replace("#unitType#", unitType);

        return effectText;
    }

    //Temporary placing this here while we refactor our TargetSystem.
    private static readonly List<TargetType> TypesThatDontNeedTargets = new List<TargetType> { TargetType.PlayerSelf, TargetType.RandomOpponentOrUnits, TargetType.OpenLane, TargetType.AllUnits, TargetType.OpponentUnits, TargetType.OurUnits, TargetType.UnitSelf, TargetType.Opponent, TargetType.None, TargetType.RandomOurUnits };

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

    public IEnumerable<CardGameEntity> GetEffectTargets(CardGame cardGame, Player player)
    {
        if (!NeedsTargets())
        {
            return new List<CardGameEntity>();
        }

        switch (TargetType)
        {
            case TargetType.TargetPlayers:
                {
                    return GetPlayers(cardGame);
                }
            case TargetType.TargetUnits:
                {
                    return GetUnits(cardGame);
                }
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
        switch (TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
            case TargetType.PlayerSelf:
                {
                    return new List<CardGameEntity> { player };
                }
            case TargetType.AllUnits:
                return cardGame.GetUnitsInPlay().Cast<CardGameEntity>().ToList();
            case TargetType.OurUnits:
                return player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.OpponentUnits:
                return cardGame.Players.Find(p => player.PlayerId != p.PlayerId).Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
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


