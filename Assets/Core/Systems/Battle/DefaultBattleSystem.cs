using System.Collections.Generic;
using System.Linq;

public class DefaultBattleSystem : CardGameSystem, IBattleSystem
{
    public DefaultBattleSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public bool CanBattle(int laneIndex)
    {
        var lane = cardGame.ActivePlayer.Lanes[laneIndex];
        if (lane.IsEmpty()) { return false; }

        //Determine whether or not the unit can attack.
        var unitCanAttack = lane.UnitInLane.IsSummoningSick ? false : true;

        foreach (var canAttackAb in lane.UnitInLane.GetAbilitiesAndComponents<IModifyCanAttack>())
        {
            unitCanAttack = canAttackAb.CanAttack(cardGame, lane.UnitInLane);
        };

        //if it is exhausted it can't attack no matter what
        if (lane.UnitInLane.IsExhausted)
        {
            unitCanAttack = false;
        }

        return unitCanAttack;
    }


    public void Battle(int laneIndex)
    {
        var attackingLane = cardGame.ActivePlayer.Lanes[laneIndex];
        var defendingLane = cardGame.InactivePlayer.Lanes[laneIndex];

        if (!CanBattle(laneIndex))
        {
            return;
        }

        //The attacking unit needs to be exhausted immediately after attacking.
        var attackingUnit = attackingLane.UnitInLane;
        attackingUnit.IsExhausted = true;

        if (defendingLane.IsEmpty() || !DefenderCanBlock(attackingLane, defendingLane))
        {
            cardGame.EventLogSystem.AddEvent($"{attackingLane.UnitInLane.Name} has attacked {cardGame.InactivePlayer.Name}");
            //Attacking an empty lane trigger any on attack abilities
            var onAttackAbilities = attackingUnit.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfAttacks);
            var owner = cardGame.GetOwnerOfCard(attackingUnit);
            foreach (var onAttackAb in onAttackAbilities)
            {
                cardGame.EffectsProcessor.ApplyEffects(owner, attackingUnit, onAttackAb.Effects, new List<CardGameEntity>());
            };
            DirectAttack(attackingLane, defendingLane);
        }
        else
        {
            cardGame.EventLogSystem.AddEvent($"{attackingLane.UnitInLane.Name} has attacked {defendingLane.UnitInLane.Name}");
            var onAttackAbilities = attackingUnit.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfAttacks);
            var owner = cardGame.GetOwnerOfCard(attackingUnit);
            foreach (var onAttackAb in onAttackAbilities)
            {
                cardGame.EffectsProcessor.ApplyEffects(owner, attackingUnit, onAttackAb.Effects, new List<CardGameEntity>());
            };
            FightUnits(attackingLane, defendingLane);
        }
    }


    #region Private Methods

    private void DirectAttack(Lane attackingLane, Lane defendingLane)
    {
        cardGame.EventLogSystem.AddEvent($"{attackingLane.UnitInLane.Name} attacked {cardGame.InactivePlayer.Name}");
        cardGame.GameEventSystem.FireEvent(
            cardGame.GameEventSystem.CreateAttackEvent(attackingLane.UnitInLane.EntityId, cardGame.InactivePlayer.EntityId));
        //Assuming that a players units cannot attack him, it should always be the inactive player getting attacked.
        cardGame.DamageSystem.DealCombatDamageToPlayer(attackingLane.UnitInLane, cardGame.InactivePlayer);
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    private void FightUnits(Lane attackingLane, Lane defendingLane)
    {
        var attackingUnit = attackingLane.UnitInLane;
        var defendingUnit = defendingLane.UnitInLane;

        //Need to add a check to see if the defending lane still has a unit (could have been removed from play due to triggered ability or something)

        if (defendingUnit != null)
        {
            cardGame.EventLogSystem.AddEvent($"{attackingUnit.Name} attacked {defendingUnit.Name}");
            //Both lanes have units, they will attack eachother.
            cardGame.DamageSystem.DealCombatDamageToUnits(attackingUnit, defendingUnit);
        }
        //TODO If there is no defending unit at this point, do we deal damage to the player instead?? what should happen?
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    //Note - this method can be hooked into from Abilities.
    private bool DefenderCanBlock(Lane attackingLane, Lane defendingLane)
    {
        if (defendingLane.IsEmpty())
        {
            return false;
        }

        bool canBlock = true; //default is that the defender is able to block unless an ability states otherwise.

        //check to see if the attacker can attack directly by default

        var attackingUnit = attackingLane.UnitInLane;
        var directAttackAbilities = attackingUnit.GetAbilitiesAndComponents<IModifyCanAttackDirectly>();

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
        var defendingUnit = defendingLane.UnitInLane;
        var modifyBlockAbilities = defendingUnit.GetAbilitiesAndComponents<IModifyCanBlock>();
        foreach (var ability in modifyBlockAbilities)
        {
            canBlock = ability.ModifyCanBlock(cardGame);
        }
        var getAbilities = defendingUnit.Abilities.Where(ab => ab is IModifyCanBlock).FirstOrDefault();

        return canBlock;

    }
    #endregion
}
