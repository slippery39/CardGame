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
            Debug.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(i + 1)}");
            Debug.Log("Checking Attacking Lane is Empty");
            Debug.Log(attackingLane.IsEmpty());
            Debug.Log("Checking Defending Lane is Empty");
            Debug.Log(defendingLane.IsEmpty());

            //Attacking Player does not have a unit in lane, so we should skip.
            if (attackingLane.IsEmpty())
            {
                Debug.Log("Both Lanes were empty");
                continue;
            }
            //Attacking an empty lane
            else if (defendingLane.IsEmpty() || !DefenderCanBlock(cardGame, attackingLane, defendingLane))
            {
                DirectAttack(cardGame, attackingLane, defendingLane);
            }
            else
            {
                FightUnits(cardGame, attackingLane, defendingLane);
            }
        }
        //Temporary hack for now. In the future we should have a seperate system which handles our turn logic.
        _ = cardGame.ActivePlayerId == 1 ? cardGame.ActivePlayerId = 2 : cardGame.ActivePlayerId = 1;
    }

    #region Private Methods

    private void DirectAttack(CardGame cardGame, Lane attackingLane, Lane defendingLane)
    {
        Debug.Log("Defending Lane was Empty");
        var attackingUnit = (UnitCardData)attackingLane.UnitInLane.CurrentCardData;
        if (cardGame.ActivePlayerId == 1)
        {
            cardGame.Player2.Health -= attackingUnit.Power;
        }
        else
        {
            cardGame.Player1.Health -= attackingUnit.Power;
        }
        //Trigger Damage Dealt Abilities.
        //TODO - Handle case if 0 damage is dealt for whatever reason.
        var getDamageDealtAbilities = attackingUnit.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (getDamageDealtAbilities != null)
        {
            var damageDealtAbility = (IOnDamageDealt)getDamageDealtAbilities;
            damageDealtAbility.OnDamageDealt(cardGame, attackingLane.UnitInLane, null, attackingUnit.Power);
        }

    }

    private void FightUnits(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        Debug.Log("Both Lanes have Units");
        //Both lanes have units, they will attack eachother.
        var attackingUnit = (UnitCardData)attackingLane.UnitInLane.CurrentCardData;
        var defendingUnit = (UnitCardData)defendingLane.UnitInLane.CurrentCardData;

        attackingUnit.Toughness -= defendingUnit.Power;
        defendingUnit.Toughness -= attackingUnit.Power;

        //Attacker Damage Dealt Abilities
        var damageDealtAbilitiesAtk = attackingUnit.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (damageDealtAbilitiesAtk != null)
        {
            var damageDealtAbilityAtk = (IOnDamageDealt)damageDealtAbilitiesAtk;
            damageDealtAbilityAtk.OnDamageDealt(gameState, attackingLane.UnitInLane, defendingLane.UnitInLane, attackingUnit.Power);
        }

        //Defender Damage Dealt Abilities
        var damageDealtAbilitiesDef = defendingUnit.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (damageDealtAbilitiesDef != null)
        {
            var damageDealtAbilityDef = (IOnDamageDealt)damageDealtAbilitiesDef;
            damageDealtAbilityDef.OnDamageDealt(gameState, defendingLane.UnitInLane, attackingLane.UnitInLane, defendingUnit.Power);
        }

        if (attackingUnit.Toughness <= 0)
        {
            //should die
            Debug.Log("Attacking Unit Is Dying");
            attackingLane.RemoveUnitFromLane();
        }
        if (defendingUnit.Toughness <= 0)
        {
            Debug.Log("Defending Unit Is Dying");
            defendingLane.RemoveUnitFromLane();
        }
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

        var attackingUnit = (UnitCardData)attackingLane.UnitInLane.CurrentCardData;
        var getDirectAttackAbilities = attackingUnit.Abilities.Where(ab => ab is IModifyCanAttackDirectly).FirstOrDefault();

        if (getDirectAttackAbilities != null)
        {
            var attackDirectlyAbility = (IModifyCanAttackDirectly)getDirectAttackAbilities;
            var canAttackDirectly = attackDirectlyAbility.ModifyCanAttackDirectly(cardGame, attackingLane, defendingLane);
            if (canAttackDirectly == true)
            {
                return false; //Opposing creature cannot block.
            }
        }

        //Check the defenders abilities to see if it has any of the the type IModifyCanBlock;
        var defendingUnit = (UnitCardData)defendingLane.UnitInLane.CurrentCardData;
        var getAbilities = defendingUnit.Abilities.Where(ab => ab is IModifyCanBlock).FirstOrDefault();

        //TODO - need some sort of priority system for determining which abilities should apply first and last.
        //in case some abilities should always override other abilities.
        //For example, the Can't Block ability would always overrdie any other ModifyBlocking abilities.

        //For now we will just do the first one, and think about how to apply multiples later.
        if (getAbilities != null)
        {
            var modifyBlockAbility = (IModifyCanBlock)getAbilities;
            canBlock = modifyBlockAbility.ModifyCanBlock(cardGame);
        }

        return canBlock;

    }
    #endregion
}
