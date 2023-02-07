using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DefaultDamageSystem : CardGameSystem, IDamageSystem
{
    public DefaultDamageSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void DealAbilityDamage(DamageEffect abilitySource, CardInstance damagingCard, CardGameEntity damagedEntity)
    {
        DealDamage(damagedEntity, abilitySource.Amount);
        cardGame.Log($"{damagingCard.Name} dealt {abilitySource.Amount} damage to {damagedEntity.Name}!");
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    public void DealCombatDamageToPlayer(CardInstance damagingUnit, Player damagedPlayer)
    {
        int damage = damagingUnit.Power;
        damagedPlayer.Health -= damagingUnit.Power;

        //TODO - Deal Damage needs to work with players too.

        cardGame.Log($"{damagedPlayer} has taken {damage} combat damage!");
        cardGame.GameEventSystem.FireEvent(
                        new DamageEvent
                        {
                            DamagedId = damagedPlayer.EntityId,
                            DamageAmount = damage
                        });

        //Trigger Damage Dealt Abilities.
        //TODO - Handle case if 0 damage is dealt for whatever reason.
        var damageDealtAbilities = damagingUnit.GetAbilitiesAndComponents<IOnDamageDealt>();

        foreach (var ability in damageDealtAbilities)
        {
            //TODO - fix deathtouchh ability.
            ability.OnDamageDealt(cardGame, damagingUnit, null, damagingUnit.Power);
        }
    }
    public void DealCombatDamageToUnits(CardInstance attackingUnit, CardInstance defendingUnit)
    {
        var attackingDamage = attackingUnit.Power;
        var defendingDamage = defendingUnit.Power;


        //IF UNIT HAS TRAMPLE
        if (attackingUnit.GetAbilitiesAndComponents<TrampleAbility>().Count() > 0)
        {
            var damageToUnit = Math.Min(attackingDamage, (defendingUnit.Toughness - defendingUnit.DamageTaken));
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

        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();

        //Attacker Damage Dealt Abilities
        var attackingAbilities = attackingUnit.GetAbilitiesAndComponents<IOnDamageDealt>();
        foreach (var ability in attackingAbilities)
        {
            ability.OnDamageDealt(cardGame, attackingUnit, defendingUnit, attackingUnit.Power);
        }

        //Defender Damage Dealt Abilities
        var defendingAbilities = defendingUnit.GetAbilitiesAndComponents<IOnDamageDealt>();
        foreach (var ability in defendingAbilities)
        {
            ability.OnDamageDealt(cardGame, defendingUnit, attackingUnit, defendingUnit.Power);
        }
    }

    public void DealDamage(CardGameEntity source, CardGameEntity target, int amount)
    {
        DealDamage(target, amount);
    }

    private void DealDamage(CardInstance damagedUnit, int damage)
    {
        //HARD CODED OUR SHIELDS IN HERE.
        if (damagedUnit.Shields > 0)
        {
            damagedUnit.Shields = damagedUnit.Shields - 1;
            return;
        }
        damagedUnit.DamageTaken += damage;
        cardGame.GameEventSystem.FireEvent(
        new DamageEvent
        {
            DamagedId = damagedUnit.EntityId,
            DamageAmount = damage
        }
        );
    }

    private void DealDamage(CardGameEntity damagedEntity, int damage)
    {
        if (damagedEntity is Player)
        {
            ((Player)damagedEntity).Health -= damage;

            cardGame.GameEventSystem.FireEvent(
                                new DamageEvent
                                {
                                    DamagedId = damagedEntity.EntityId,
                                    DamageAmount = damage
                                });
        }
        if (damagedEntity is CardInstance)
        {
            DealDamage((CardInstance)damagedEntity, damage);
        }
    }
}
