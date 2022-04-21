using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DefaultDamageSystem : IDamageSystem
{
    public void DealAbilityDamage(CardGame cardGame, DamageEffect abilitySource, CardInstance damagingCard, CardGameEntity damagedEntity)
    {
        DealDamage(damagedEntity, abilitySource.Amount);
        cardGame.Log($"{damagingCard.Name} dealt {abilitySource.Amount} damage to {damagedEntity.Name}!");
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects(cardGame);
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


        //IF UNIT HAS TRAMPLE
        if (attackingUnit.GetAbilities<TrampleAbility>().Count > 0)
        {
            var damageToUnit = Math.Min(attackingDamage, defendingUnit.Toughness);
            var damageToPlayer = attackingUnit.Power - damageToUnit;

            if (damageToPlayer > 0)
            {
                var defendingPlayer = cardGame.GetOwnerOfCard(defendingUnit);
                DealDamage(cardGame.GetOwnerOfCard(defendingUnit), damageToPlayer);
                cardGame.Log($"{defendingPlayer.Name} took {damageToPlayer} trample damage!");
            }
            DealDamage(attackingUnit, defendingDamage);
            DealDamage(defendingUnit, damageToUnit);
            cardGame.Log($"{defendingUnit.Name} took {damageToUnit} combat damage");
            cardGame.Log($"{attackingUnit.Name} took {defendingDamage} combat damage");
        }
        //IF UNIT DOES NOT HAVE TRAMPLE
        else
        {
            DealDamage(attackingUnit, defendingDamage);
            DealDamage(defendingUnit, attackingDamage);
            cardGame.Log($"{defendingUnit.Name} took {attackingDamage} combat damage");
            cardGame.Log($"{attackingUnit.Name} took {defendingDamage} combat damage");
        }

        cardGame.StateBasedEffectSystem.CheckStateBasedEffects(cardGame);

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

    public void DealDamage(CardGame cardGame, CardGameEntity source, CardGameEntity target, int amount)
    {
        DealDamage(target, amount);
    }

    private void DealDamage(CardInstance damagedUnit, int damage)
    {
        damagedUnit.DamageTaken += damage;
    }

    private void DealDamage(CardGameEntity damagedEntity, int damage)
    {
        if (damagedEntity is Player)
        {
            ((Player)damagedEntity).Health -= damage;
        }
        if (damagedEntity is CardInstance)
        {
            DealDamage((CardInstance)damagedEntity, damage);
        }
    }
}
