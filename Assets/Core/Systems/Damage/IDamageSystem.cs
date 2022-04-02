using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public interface IDamageSystem
{
    void DealCombatDamageToUnits(CardGame cardGame, CardInstance attackingUnit, CardInstance defendingUnit);
    void DealCombatDamageToPlayer(CardGame cardGame, CardInstance damagingUnit, Player damagedPlayer);
    void DealAbilityDamage(CardGame cardGame, DamageEffect abilitySource, CardInstance damagingCard, CardGameEntity damagedEntity);
    void DealDamage(CardGame cardGame, CardGameEntity source, CardGameEntity target, int amount);
}
