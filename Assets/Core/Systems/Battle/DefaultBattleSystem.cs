using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultBattleSystem : IBattleSystem
{
    private CardGame cardGame;

    public DefaultBattleSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void ExecuteBattles()
    {
        var attackingLanes = cardGame.ActivePlayer.Lanes;

        for (int i = 0; i < attackingLanes.Count; i++)
        {
            Battle(i);
        }
    }


    public void Battle(int laneIndex)
    {
        var attackingLane = cardGame.ActivePlayer.Lanes[laneIndex];
        var defendingLane = cardGame.InactivePlayer.Lanes[laneIndex];

        //Attacking Player does not have a unit in lane, so we should skip.
        if (attackingLane.IsEmpty())
        {
            return;
        }

        //Determine whether or not the unit can attack.
        var unitCanAttack = attackingLane.UnitInLane.IsSummoningSick ? false : true;

        foreach (var canAttackAb in attackingLane.UnitInLane.GetAbilitiesAndComponents<IModifyCanAttack>())
        {
            unitCanAttack = canAttackAb.CanAttack(cardGame, attackingLane.UnitInLane);
        };

        //if it is exhausted it can't attack no matter what
        if (attackingLane.UnitInLane.IsExhausted)
        {
            unitCanAttack = false;
        }

        if (!unitCanAttack)
        {
            cardGame.Log($@"{attackingLane.UnitInLane.Name} cannot attack!");
            return;
        }
        else if (defendingLane.IsEmpty() || !DefenderCanBlock(attackingLane, defendingLane))
        {
            //Attacking an empty lane trigger any on attack abilities

            var attackingUnit = attackingLane.UnitInLane;
            var onAttackAbilities = attackingUnit.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfAttacks);
            var owner = cardGame.GetOwnerOfCard(attackingUnit);
            foreach (var onAttackAb in onAttackAbilities)
            {
                cardGame.EffectsProcessor.ApplyEffects(owner, attackingUnit, onAttackAb.Effects, new List<CardGameEntity>());
            };
            cardGame.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(laneIndex + 1)}");
            DirectAttack(attackingLane, defendingLane);
        }
        else
        {
            //Attacking an empty lane trigger any on attack abilities

            var attackingUnit = attackingLane.UnitInLane;
            var onAttackAbilities = attackingUnit.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfAttacks);
            var owner = cardGame.GetOwnerOfCard(attackingUnit);
            foreach (var onAttackAb in onAttackAbilities)
            {
                cardGame.EffectsProcessor.ApplyEffects(owner, attackingUnit, onAttackAb.Effects, new List<CardGameEntity>());
            };
            cardGame.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(laneIndex + 1)}");
            FightUnits(attackingLane, defendingLane);
        }

        //unit is exhausted after attacking
        //note if we implement vigilance we would need a way to override this
        attackingLane.UnitInLane.IsExhausted = true;
    }


    #region Private Methods

    private void DirectAttack(Lane attackingLane, Lane defendingLane)
    {
        cardGame.Log($"{attackingLane.UnitInLane.Name} is attacking directly!");
        //Assuming that a players units cannot attack him, it should always be the inactive player getting attacked.
        cardGame.DamageSystem.DealCombatDamageToPlayer(attackingLane.UnitInLane, cardGame.InactivePlayer);
    }

    private void FightUnits(Lane attackingLane, Lane defendingLane)
    {
        var attackingUnit = attackingLane.UnitInLane;
        var defendingUnit = defendingLane.UnitInLane;

        cardGame.Log($"{attackingUnit.Name} is fighting {defendingUnit.Name}");
        //Both lanes have units, they will attack eachother.
        cardGame.DamageSystem.DealCombatDamageToUnits(attackingUnit, defendingUnit);
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    //Note - this method can be hooked into from Abilities.
    private bool DefenderCanBlock(Lane attackingLane, Lane defendingLane)
    {
        if (defendingLane.IsEmpty())
        {
            return false;
        }

        bool canBlock = true; //default is that the defender is able to block unless an ability states otherwise.

        //check to see if the attacker can attack directly by default

        var attackingUnit = attackingLane.UnitInLane;
        var directAttackAbilities = attackingUnit.GetAbilitiesAndComponents<IModifyCanAttackDirectly>();

        var canAttackDirectly = false;
        //In this case, it might be possible to just take the highest priority ability in the list.
        //Although if any abilities are tracking some sort of internal state with their method then we may need
        //to still fire it..
        foreach (var ability in directAttackAbilities)
        {
            canAttackDirectly = ability.ModifyCanAttackDirectly(cardGame, attackingLane, defendingLane);
        }
        if (canAttackDirectly)
        {
            return false; //defender can't block, we can attack directly.
        }


        //TODO - need some sort of priority system for determining which abilities should apply first and last.
        //in case some abilities should always override other abilities.
        //For example, the Can't Block ability would always overrdie any other ModifyBlocking abilities.
        //Check the defenders abilities to see if it has any of the the type IModifyCanBlock;
        var defendingUnit = defendingLane.UnitInLane;
        var modifyBlockAbilities = defendingUnit.GetAbilitiesAndComponents<IModifyCanBlock>();
        foreach (var ability in modifyBlockAbilities)
        {
            canBlock = ability.ModifyCanBlock(cardGame);
        }
        var getAbilities = defendingUnit.Abilities.Where(ab => ab is IModifyCanBlock).FirstOrDefault();

        return canBlock;

    }
    #endregion
}
