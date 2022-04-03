using System;
using System.Collections.Generic;
using System.Linq;

public interface ITargetSystem
{
    List<CardGameEntity> GetValidTargets(CardGame cardGame, Player player, CardInstance card);
    public List<CardGameEntity> GetEntitiesToApplyEffect(CardGame cardGame, Player player, CardGameEntity source, Effect effect);
    bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card);
    bool EffectNeedsTargets(Effect effect);
}

public class DefaultTargetSystem : ITargetSystem
{

    private List<TargetType> typesThatDontNeedTargets = new List<TargetType> { TargetType.Self, TargetType.AllUnits, TargetType.OpponentUnits,TargetType.OurUnits, TargetType.UnitSelf, TargetType.Opponent, TargetType.None };

    /// <summary>
    /// Gets the correct entities to apply an effect to when there is no manual targets.
    /// </summary>
    /// <param name="cardGame">The card game</param>
    /// <param name="player">The owner of the effect</param>
    /// <param name="effect">The effect that is being applied</param>
    /// <returns></returns>
    /// 
    public List<CardGameEntity> GetEntitiesToApplyEffect(CardGame cardGame, Player player, CardGameEntity effectSource, Effect effect)
    {
        switch (effect.TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
            case TargetType.Self:
                return new List<CardGameEntity> { player };
            case TargetType.AllUnits:
                return cardGame.GetUnitsInPlay().Cast<CardGameEntity>().ToList();
            case TargetType.OurUnits:
                return player.Lanes.Where(l => !l.IsEmpty()).Select(l=>l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.OpponentUnits:
                return cardGame.Players.Where(p => player.PlayerId != p.PlayerId).First().Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.UnitSelf:
                return new List<CardGameEntity>() { effectSource };
            case TargetType.Opponent:
                return cardGame.Players.Where(p => p.EntityId != player.EntityId).Cast<CardGameEntity>().ToList();
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {effect.TargetType}");
        }
    }

    public bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card)
    {
        var spellCard = (SpellCardData)card.CurrentCardData;
        var spellTargetTypes = spellCard.Effects.Select(effect => effect.TargetType);

        return (spellTargetTypes.Where(tt => typesThatDontNeedTargets.Contains(tt) == false).Count() > 0);
    }

    private List<CardGameEntity> GetValidUnitTargets(CardGame cardGame, Player player, CardInstance card)
    {
        var units =
            cardGame
            .GetEntities<Lane>()
            .Where(lane => lane.IsEmpty() == false)
            .Select(lane => lane.UnitInLane);

        units = units.Where(unit =>
        {
            var modTargetAbilities = unit.GetAbilities<IModifyCanBeTargeted>();
            var canBeTargeted = true;

            foreach (var ability in modTargetAbilities)
            {
                canBeTargeted = ability.ModifyCanBeTargeted(cardGame, unit, player);
            }

            return canBeTargeted;
        });

        return units.Cast<CardGameEntity>().ToList();
    }

    private List<CardGameEntity> GetValidPlayerTargets(CardGame cardGame, Player player, CardInstance card)
    {
        return cardGame.Players.Cast<CardGameEntity>().ToList();
    }
    public List<CardGameEntity> GetValidTargets(CardGame cardGame, Player player, CardInstance card)
    {
        if (card.CurrentCardData is UnitCardData)
        {
            var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty()).ToList();
            var emptyLanesCast = emptyLanes.Cast<CardGameEntity>();

            return emptyLanesCast.ToList();
        }
        else if (card.CurrentCardData is SpellCardData)
        {
            var spellCard = (SpellCardData)card.CurrentCardData;
            var effects = spellCard.Effects.Select(e => e.TargetType);

            if (!SpellNeedsTargets(cardGame, player, card))
            {
                return new List<CardGameEntity>();
            }
            else
            {
                if (effects.Contains(TargetType.TargetPlayers))
                {
                    return GetValidPlayerTargets(cardGame, player, card);
                }
                else if (effects.Contains(TargetType.TargetUnits))
                {
                    return GetValidUnitTargets(cardGame, player, card);
                }
                else if (effects.Contains(TargetType.TargetUnitsOrPlayers))
                {
                    var validTargets = GetValidUnitTargets(cardGame, player, card);
                    validTargets.AddRange(GetValidPlayerTargets(cardGame, player, card));
                    return validTargets;
                }
            }
        }
        return new List<CardGameEntity>();
    }
    public bool EffectNeedsTargets(Effect effect)
    {
        return typesThatDontNeedTargets.Contains(effect.TargetType) == false;
    }
}