using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultBattleSystem : IBattleSystem
{
    public void ExecuteBattles(CardGame cardGame)
    {
        var attackingLanes = cardGame.ActivePlayer.Lanes;
        var defendingLanes = cardGame.InactivePlayer.Lanes;

        for (int i = 0; i < attackingLanes.Count; i++)
        {
            var attackingLane = attackingLanes[i];
            var defendingLane = defendingLanes[i];

            //Attacking Player does not have a unit in lane, so we should skip.
            if (attackingLane.IsEmpty())
            {
                continue;
            }

            //Determine whether or not the unit can attack.
            var unitCanAttack = attackingLane.UnitInLane.IsSummoningSick ? false : true;
            foreach (var canAttackAb in attackingLane.UnitInLane.GetAbilities<IModifyCanAttack>())
            {
                unitCanAttack = canAttackAb.CanAttack(cardGame, attackingLane.UnitInLane);
            };

            if (!unitCanAttack)
            {
                cardGame.Log($@"{attackingLane.UnitInLane.Name} cannot attack!");
                continue;
            }
            //Attacking an empty lane
            else if (defendingLane.IsEmpty() || !DefenderCanBlock(cardGame, attackingLane, defendingLane))
            {
                cardGame.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(i + 1)}");
                DirectAttack(cardGame, attackingLane, defendingLane);
            }
            else
            {
                cardGame.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(i + 1)}");
                FightUnits(cardGame, attackingLane, defendingLane);
            }
        }
    }

    #region Private Methods

    private void DirectAttack(CardGame cardGame, Lane attackingLane, Lane defendingLane)
    {
        cardGame.Log($"{attackingLane.UnitInLane.Name} is attacking directly!");
        //Assuming that a players units cannot attack him, it should always be the inactive player getting attacked.
        cardGame.DamageSystem.DealCombatDamageToPlayer(cardGame, attackingLane.UnitInLane, cardGame.InactivePlayer);
    }

    private void FightUnits(CardGame cardGame, Lane attackingLane, Lane defendingLane)
    {
        var attackingUnit = attackingLane.UnitInLane;
        var defendingUnit = defendingLane.UnitInLane;

        cardGame.Log($"{attackingUnit.Name} is fighting {defendingUnit.Name}");
        //Both lanes have units, they will attack eachother.
        cardGame.DamageSystem.DealCombatDamageToUnits(cardGame, attackingUnit, defendingUnit);
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects(cardGame);
    }

    //Note - this method can be hooked into from Abilities.
    private bool DefenderCanBlock(CardGame cardGame, Lane attackingLane, Lane defendingLane)
    {
        if (defendingLane.IsEmpty())
        {
            return false;
        }

        bool canBlock = true; //default is that the defender is able to block unless an ability states otherwise.

        //check to see if the attacker can attack directly by default

        var attackingUnit = attackingLane.UnitInLane;
        var directAttackAbilities = attackingUnit.GetAbilities<IModifyCanAttackDirectly>();

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
        var modifyBlockAbilities = defendingUnit.GetAbilities<IModifyCanBlock>();
        foreach (var ability in modifyBlockAbilities)
        {
            canBlock = ability.ModifyCanBlock(cardGame);
        }
        var getAbilities = defendingUnit.Abilities.Where(ab => ab is IModifyCanBlock).FirstOrDefault();

        return canBlock;

    }
    #endregion
}
