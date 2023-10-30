using Assets.Core;
using System;
using System.Collections.Generic;
using System.Linq;

public interface ITargetSystem
{
    List<CardGameEntity> GetValidTargets(Player player, CardInstance card);
    List<CardGameEntity> GetValidEffectTargets(Player player, List<Effect> effects);
    List<CardGameEntity> GetValidAbilityTargets(Player player, CardInstance card);
    List<CardGameEntity> GetEntitiesToApplyEffect(Player player, CardGameEntity source, Effect effect);
    bool CardNeedsTargets(Player player, CardInstance card);
    bool ActivatedAbilityNeedsTargets(CardInstance cardWithAbility);
}


public class DefaultTargetSystem : CardGameSystem, ITargetSystem
{
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
        return effect.GetEntitiesToApplyEffect(cardGame, player, effectSource).ToList();
    }

    public bool CardNeedsTargets(Player player, CardInstance card)
    {
        //Temporary Work Around for non SpellCards that might need this...
        if (!(card.CurrentCardData is SpellCardData))
        {
            return false;
        }

        //TODO - this could be changed to use IHasEffects
        var spellCard = (SpellCardData)card.CurrentCardData;
        return spellCard.Effects.NeedsTargets();
    }

    //TODO - Generalize this and SpellNeedsTargets... perhaps it should just be under ActionNeedsTargets...
    public bool ActivatedAbilityNeedsTargets(CardInstance cardWithAbility)
    {
        var activatedAbility = cardWithAbility.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault();
        if (activatedAbility == null)
        {
            throw new Exception("ActivatedAbilityNeedsTargets is being called, but we can't find an actiated ability");
        }
        return activatedAbility.Effects.NeedsTargets();
    }

    private List<CardGameEntity> GetValidUnitTargets(Player player)
    {
        var units =
            cardGame
            .GetEntities<Lane>()
            .Where(lane => !lane.IsEmpty())
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

    private List<CardGameEntity> GetValidPlayerTargets()
    {
        return cardGame.Players.Cast<CardGameEntity>().ToList();
    }

    /// <summary>
    /// Gets the valid effect targets for a List of effects.
    /// This assumes a list of effects will always have only 1 TargetType declared.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="effects"></param>
    /// <returns></returns>
    public List<CardGameEntity> GetValidEffectTargets(Player player, List<Effect> effects)
    {
        var effectTargets = effects.SelectMany(effect => effect.GetEffectTargets(cardGame, player));
        effectTargets = effectTargets.Where(target =>
        {
            CardInstance cardInstanceTarget = target as CardInstance;
            if (cardInstanceTarget != null)
            {
                //TODO - It shouldn't matter what type of card it is, 
                //We should just be able to call CardGameEntity.GetComponents<IModifyCanBeTargeted>()
                //Without needing to know if it is an ability or not.

                var modTargetAbilities = cardInstanceTarget.GetAbilitiesAndComponents<IModifyCanBeTargeted>();
                var canBeTargeted = true;

                foreach (var ability in modTargetAbilities)
                {
                    canBeTargeted = ability.ModifyCanBeTargeted(cardGame, cardInstanceTarget, player);
                }

                return canBeTargeted;
            }
            else
            {

                return true;
            }
        });

        return effectTargets.ToList();
    }

    public List<CardGameEntity> GetValidAbilityTargets(Player player, CardInstance cardWithAbility)
    {
        var ability = cardWithAbility.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault();

        if (ability == null)
        {
            return new List<CardGameEntity>();
        }

        var effectTargets = GetValidEffectTargets(player, ability.Effects);
        return effectTargets;
    }
    //TODO - Perhaps Targets should only matter for Actions, and not for cards.
    //So instead of passing in a card here, we pass in an action and process the targets that way.
    public List<CardGameEntity> GetValidTargets(Player player, CardInstance card)
    {
        if (card.CurrentCardData is UnitCardData)
        {
            var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty()).ToList();
            var emptyLanesCast = emptyLanes.Cast<CardGameEntity>();

            return emptyLanesCast.ToList();
        }

        var spellCard = card.CurrentCardData as SpellCardData;

        if (spellCard != null)
        {
            if (!CardNeedsTargets(player, card))
            {
                return new List<CardGameEntity>();
            }
            else
            {
                return GetValidEffectTargets(player, spellCard.Effects);
            }
        }

        return new List<CardGameEntity>();
    }
}
