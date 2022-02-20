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
        //Assuming that a players units cannot attack him, it should always be the inactive player getting attacked.
        cardGame.DamageSystem.DealCombatDamageToPlayer(cardGame, attackingLane.UnitInLane, cardGame.InactivePlayer);
    }

    private void FightUnits(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        Debug.Log("Both Lanes have Units");
        Debug.Log(gameState);
        Debug.Log(attackingLane.UnitInLane);
        Debug.Log(defendingLane.UnitInLane);
        //Both lanes have units, they will attack eachother.
        gameState.DamageSystem.DealCombatDamageToUnits(gameState, attackingLane.UnitInLane, defendingLane.UnitInLane);

        var attackingUnit = (UnitCardData)attackingLane.UnitInLane.CurrentCardData;
        var defendingUnit = (UnitCardData)defendingLane.UnitInLane.CurrentCardData;

        if (attackingUnit.Toughness <= 0)
        {
            //should die
            Debug.Log("Attacking Unit Is Dying");
            attackingLane.RemoveUnitFromLane();
        }
        if (defendingUnit.Toughness <= 0)
        {
            //should also die.
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
        var defendingUnit = (UnitCardData)defendingLane.UnitInLane.CurrentCardData;
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
