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
        int damage = damagingUnitData.Power;
        damagedPlayer.Health -= damagingUnitData.Power;

        cardGame.Log($"{damagedPlayer} has taken {damage} combat damage!");

        //Trigger Damage Dealt Abilities.
        //TODO - Handle case if 0 damage is dealt for whatever reason.
        var damageDealtAbilities = damagingUnitData.GetAbilities<IOnDamageDealt>();

        foreach (var ability in damageDealtAbilities)
        {
            //TODO - fix deathtouchh ability.
            ability.OnDamageDealt(cardGame, damagingUnit, null, damagingUnitData.Power);
        }
    }
    public void DealCombatDamageToUnits(CardGame cardGame, CardInstance attackingUnit, CardInstance defendingUnit)
    {
        var defendingUnitData = (UnitCardData)defendingUnit.CurrentCardData;
        var attackingUnitData = (UnitCardData)attackingUnit.CurrentCardData;

        var attackingDamage = attackingUnitData.Power;
        var defendingDamage = defendingUnitData.Power;

        attackingUnitData.Toughness -= defendingDamage;
        defendingUnitData.Toughness -= attackingDamage;

        cardGame.Log($"{defendingUnitData.Name} took {attackingDamage} combat damage");
        cardGame.Log($"{attackingUnitData.Name} took {defendingDamage} combat damage");

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
