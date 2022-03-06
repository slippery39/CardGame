using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DefaultDamageSystem : IDamageSystem
{
    public void DealAbilityDamage(CardGame cardGame, DamageAbility abilitySource, CardInstance damagingCard, CardInstance damagedCard)
    {
        DealDamage(damagedCard, abilitySource.Amount);
        cardGame.Log($"{damagingCard.Name} dealt {abilitySource.Amount} damage to {damagedCard.Name}!");
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects(cardGame);
        //TODO - Fire any of our on damage dealt effects.
    }

    public void DealCombatDamageToPlayer(CardGame cardGame, CardInstance damagingUnit, Player damagedPlayer)
    {
        int damage = damagingUnit.Power;
        damagedPlayer.Health -= damagingUnit.Power;

        //TODO - Deal Damage needs to work with players too.

        cardGame.Log($"{damagedPlayer} has taken {damage} combat damage!");

        //Trigger Damage Dealt Abilities.
        //TODO - Handle case if 0 damage is dealt for whatever reason.
        var damageDealtAbilities = damagingUnit.GetAbilities<IOnDamageDealt>();

        foreach (var ability in damageDealtAbilities)
        {
            //TODO - fix deathtouchh ability.
            ability.OnDamageDealt(cardGame, damagingUnit, null, damagingUnit.Power);
        }
    }
    public void DealCombatDamageToUnits(CardGame cardGame, CardInstance attackingUnit, CardInstance defendingUnit)
    {
        var attackingDamage = attackingUnit.Power;
        var defendingDamage = defendingUnit.Power;

        DealDamage(attackingUnit, defendingDamage);
        DealDamage(defendingUnit, attackingDamage);

        cardGame.Log($"{defendingUnit.Name} took {attackingDamage} combat damage");
        cardGame.Log($"{attackingUnit.Name} took {defendingDamage} combat damage");

        //Attacker Damage Dealt Abilities
        var attackingAbilities = attackingUnit.GetAbilities<IOnDamageDealt>();
        foreach (var ability in attackingAbilities)
        {
            ability.OnDamageDealt(cardGame, attackingUnit, defendingUnit, attackingUnit.Power);
        }

        //Defender Damage Dealt Abilities
        var defendingAbilities = defendingUnit.GetAbilities<IOnDamageDealt>();
        foreach (var ability in defendingAbilities)
        {
            ability.OnDamageDealt(cardGame, defendingUnit, attackingUnit, defendingUnit.Power);
        }
    }

    private void DealDamage(CardInstance damagedUnit,int damage)
    {
        damagedUnit.Toughness -= damage;
        
    }
}
