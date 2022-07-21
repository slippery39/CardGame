using System;
using System.Collections.Generic;
using System.Linq;

public interface ITargetSystem
{
    List<CardGameEntity> GetValidTargets(Player player, CardInstance card);

    List<CardGameEntity> GetValidEffectTargets(Player player, List<Effect> effects);
    List<CardGameEntity> GetValidAbilityTargets(Player player, CardInstance card);
    List<CardGameEntity> GetEntitiesToApplyEffect(Player player, CardGameEntity source, Effect effect);
    List<Effect> GetEffectsThatNeedTargets(List<Effect> effects);
    bool CardNeedsTargets(Player player, CardInstance card);
    bool EffectNeedsTargets(Effect effect);
    bool ActivatedAbilityNeedsTargets(Player player, CardInstance cardWithAbility);
}

public static class TargetHelper
{
    public static List<TargetType> TypesThatDontNeedTargets = new List<TargetType> { TargetType.Self, TargetType.OpenLane, TargetType.AllUnits, TargetType.OpponentUnits, TargetType.OurUnits, TargetType.UnitSelf, TargetType.Opponent, TargetType.None, TargetType.RandomOurUnits };

    public static bool NeedsTargets(ActivatedAbility ability)
    {
        var abilityTargets = ability.Effects.Select(a => a.TargetType);
        return abilityTargets.Where(te => TypesThatDontNeedTargets.Contains(te) == false).Count() > 0;
    }
}


public class DefaultTargetSystem : ITargetSystem
{

    private List<TargetType> typesThatDontNeedTargets = TargetHelper.TypesThatDontNeedTargets;
    private CardGame cardGame;

    public DefaultTargetSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    /// <summary>
    /// Gets the correct entities to apply an effect to when there is no manual targets.
    /// </summary>
    /// <param name="cardGame">The card game</param>
    /// <param name="player">The owner of the effect</param>
    /// <param name="effect">The effect that is being applied</param>
    /// <returns></returns>
    /// 
    public List<CardGameEntity> GetEntitiesToApplyEffect(Player player, CardGameEntity effectSource, Effect effect)
    {
        switch (effect.TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
            case TargetType.Self:
                {
                    if (effect is PumpUnitEffect)
                    {
                        return new List<CardGameEntity> { effectSource };
                    }
                    return new List<CardGameEntity> { player };
                }
            case TargetType.AllUnits:
                return cardGame.GetUnitsInPlay().Cast<CardGameEntity>().ToList();
            case TargetType.OurUnits:
                return player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.OpponentUnits:
                return cardGame.Players.Where(p => player.PlayerId != p.PlayerId).First().Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.UnitSelf:
                return new List<CardGameEntity>() { effectSource };
            case TargetType.Opponent:
                return cardGame.Players.Where(p => p.EntityId != player.EntityId).Cast<CardGameEntity>().ToList();
            case TargetType.RandomOurUnits:
                var ourUnits = player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Randomize();
                var filtered = CardFilter.ApplyFilter(ourUnits.ToList(), effect.Filter);
                return new List<CardGameEntity> { filtered.FirstOrDefault() };
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {effect.TargetType}");
        }
    }

    public bool CardNeedsTargets(Player player, CardInstance card)
    {
        //Temporary Work Around for non SpellCards that might need this...
        if (!(card.CurrentCardData is SpellCardData))
        {
            return false;
        }

        var spellCard = (SpellCardData)card.CurrentCardData;
        var spellTargetTypes = spellCard.Effects.Select(effect => effect.TargetType);

        return (spellTargetTypes.Where(tt => typesThatDontNeedTargets.Contains(tt) == false).Count() > 0);
    }

    //TODO - Generalize this and SpellNeedsTargets... perhaps it should just be under ActionNeedsTargets...
    public bool ActivatedAbilityNeedsTargets(Player player, CardInstance cardWithAbility)
    {
        var targetsFromEffects = cardWithAbility.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault().Effects.Select(e => e.TargetType);
        return targetsFromEffects.Where(te => typesThatDontNeedTargets.Contains(te) == false).Count() > 0;
    }

    private List<CardGameEntity> GetValidUnitTargets(Player player)
    {
        var units =
            cardGame
            .GetEntities<Lane>()
            .Where(lane => lane.IsEmpty() == false)
            .Select(lane => lane.UnitInLane);

        units = units.Where(unit =>
        {
            var modTargetAbilities = unit.GetAbilitiesAndComponents<IModifyCanBeTargeted>();
            var canBeTargeted = true;

            foreach (var ability in modTargetAbilities)
            {
                canBeTargeted = ability.ModifyCanBeTargeted(cardGame, unit, player);
            }

            return canBeTargeted;
        });

        return units.Cast<CardGameEntity>().ToList();
    }

    private List<CardGameEntity> GetValidPlayerTargets(Player player)
    {
        return cardGame.Players.Cast<CardGameEntity>().ToList();
    }

    public bool EffectsNeedsTargets(Player player, List<Effect> effects)
    {
        var effectTargets = effects.Select(e => e.TargetType).ToList();
        return effectTargets.Where(te => typesThatDontNeedTargets.Contains(te) == false).Count() > 0;
    }

    public List<Effect> GetEffectsThatNeedTargets(List<Effect> effects)
    {
        return effects.Where(e => typesThatDontNeedTargets.Contains(e.TargetType) == false).ToList();
    }

    public List<CardGameEntity> GetValidEffectTargets(Player player, List<Effect> effects)
    {
        var effectTargets = effects.Select(e => e.TargetType).ToList();

        if (!EffectsNeedsTargets(player, effects))
        {
            return new List<CardGameEntity>();
        }

        if (effectTargets.Contains(TargetType.TargetPlayers))
        {
            return GetValidPlayerTargets(player);
        }
        else if (effectTargets.Contains(TargetType.TargetUnits))
        {
            return GetValidUnitTargets(player);
        }
        else if (effectTargets.Contains(TargetType.TargetUnitsOrPlayers))
        {
            var validTargets = GetValidUnitTargets(player);
            validTargets.AddRange(GetValidPlayerTargets(player));
            return validTargets;
        }

        return new List<CardGameEntity>();
    }

    public List<CardGameEntity> GetValidAbilityTargets(Player player, CardInstance cardWithAbility)
    {
        var effectTargets = cardWithAbility.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault().Effects.Select(e => e.TargetType); //for compatibility purposes. 
        if (!ActivatedAbilityNeedsTargets(player, cardWithAbility))
        {
            return new List<CardGameEntity>();
        }
        else
        {
            if (effectTargets.Contains(TargetType.TargetPlayers))
            {
                return GetValidPlayerTargets(player);
            }
            else if (effectTargets.Contains(TargetType.TargetUnits))
            {
                return GetValidUnitTargets(player);
            }
            else if (effectTargets.Contains(TargetType.TargetUnitsOrPlayers))
            {
                var validTargets = GetValidUnitTargets(player);
                validTargets.AddRange(GetValidPlayerTargets(player));
                return validTargets;
            }
        }
        return new List<CardGameEntity>();

    }
    public List<CardGameEntity> GetValidTargets(Player player, CardInstance card)
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

            if (!CardNeedsTargets(player, card))
            {
                return new List<CardGameEntity>();
            }
            else
            {
                if (effects.Contains(TargetType.TargetPlayers))
                {
                    return GetValidPlayerTargets(player);
                }
                else if (effects.Contains(TargetType.TargetUnits))
                {
                    return GetValidUnitTargets(player);
                }
                else if (effects.Contains(TargetType.TargetUnitsOrPlayers))
                {
                    var validTargets = GetValidUnitTargets(player);
                    validTargets.AddRange(GetValidPlayerTargets(player));
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