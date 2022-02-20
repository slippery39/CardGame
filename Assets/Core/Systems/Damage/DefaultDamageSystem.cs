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
        var getDamageDealtAbilities = damagingUnitData.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (getDamageDealtAbilities != null)
        {
            var damageDealtAbility = (IOnDamageDealt)getDamageDealtAbilities;
            damageDealtAbility.OnDamageDealt(cardGame, damagingUnit, null, damagingUnitData.Power);
        }
    }

    public void DealCombatDamageToUnits(CardGame cardGame, CardInstance attackingUnit, CardInstance defendingUnit)
    {
        var defendingUnitData = (UnitCardData)defendingUnit.CurrentCardData;
        var attackingUnitData = (UnitCardData)attackingUnit.CurrentCardData;

        Debug.Log(defendingUnitData.Power);
        Debug.Log(attackingUnitData.Power);

        attackingUnitData.Toughness -= defendingUnitData.Power;
        defendingUnitData.Toughness -= attackingUnitData.Power;

        Debug.Log("before checking the abilities");

        //Attacker Damage Dealt Abilities
        var damageDealtAbilitiesAtk = attackingUnitData.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (damageDealtAbilitiesAtk != null)
        {
            var damageDealtAbilityAtk = (IOnDamageDealt)damageDealtAbilitiesAtk;
            damageDealtAbilityAtk.OnDamageDealt(cardGame, attackingUnit, defendingUnit, attackingUnitData.Power);
        }

        //Defender Damage Dealt Abilities
        var damageDealtAbilitiesDef = defendingUnitData.Abilities.Where(ab => ab is IOnDamageDealt).FirstOrDefault();
        if (damageDealtAbilitiesDef != null)
        {
            var damageDealtAbilityDef = (IOnDamageDealt)damageDealtAbilitiesDef;
            damageDealtAbilityDef.OnDamageDealt(cardGame, defendingUnit, attackingUnit, defendingUnitData.Power);
        }
    }
}
