using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public interface IDamageSystem
{
    void DealCombatDamageToUnits(CardInstance attackingUnit, CardInstance defendingUnit);
    void DealCombatDamageToPlayer(CardInstance damagingUnit, Player damagedPlayer);
    void DealAbilityDamage(DamageEffect abilitySource, CardInstance damagingCard, CardGameEntity damagedEntity);
    void DealDamage(CardGameEntity source, CardGameEntity target, int amount);
}
