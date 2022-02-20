using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DefaultDamageSystem : IDamageSystem
{
    public void DealCombatDamageToPlayer(CardGame cardGame, CardInstance damagingUnit, Player damagedPlayer)
    {
        var damagingUnitData = (UnitCardData)damagingUnit.CurrentCardData;
        damagedPlayer.Health -= damagingUnitData.Power;

        //Trigger Damage Dealt Abilities.
        //TODO - Handle case if 0 damage is dealt for whatever reason.

        var damageDealtAbilities = damagingUnitData.GetAbilities<IOnDamageDealt>();

        foreach (var ability in damageDealtAbilities)
        {
            ability.OnDamageDealt(cardGame, damagingUnit, null, damagingUnitData.Power);
        }
    }
    public void DealCombatDamageToUnits(CardGame cardGame, CardInstance attackingUnit, CardInstance defendingUnit)
    {
        var defendingUnitData = (UnitCardData)defendingUnit.CurrentCardData;
        var attackingUnitData = (UnitCardData)attackingUnit.CurrentCardData;

        attackingUnitData.Toughness -= defendingUnitData.Power;
        defendingUnitData.Toughness -= attackingUnitData.Power;

        Debug.Log("before checking the abilities");

        //Attacker Damage Dealt Abilities
        var attackingAbilities = attackingUnitData.GetAbilities<IOnDamageDealt>();
        foreach (var ability in attackingAbilities)
        {
            ability.OnDamageDealt(cardGame, attackingUnit, defendingUnit, attackingUnitData.Power);
        }

        //Defender Damage Dealt Abilities
        var defendingAbilities = defendingUnitData.GetAbilities<IOnDamageDealt>();
        foreach (var ability in defendingAbilities)
        {
            ability.OnDamageDealt(cardGame, defendingUnit, attackingUnit, defendingUnitData.Power);
        }
    }
}
