﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardAbility
{
    public string Type;
    public int Priority { get; set; }
    public abstract string RulesText { get; }
}

public interface IModifyCanBlock
{
    bool ModifyCanBlock(CardGame cardGame);
}

public class CantBlockAbility : CardAbility, IModifyCanBlock
{
    public override string RulesText => "Can't Block";
    public CantBlockAbility()
    {
        Type = "Cant Block";
    }

    public bool ModifyCanBlock(CardGame cardGame)
    {
        //This creature can't ever block;
        return false;
    }
}

public interface IOnDamageDealt
{
    void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage);
}

public class LifelinkAbility : CardAbility, IOnDamageDealt
{
    public override string RulesText => "Lifelink";
    public LifelinkAbility()
    {
        Type = "Lifelink";
    }

    public void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Need a way to find out who owns which unit
        Player playerToGainLife = cardGame.GetOwnerOfUnit(damagingUnit);
        cardGame.Log($"{playerToGainLife} gained {damage} life from {damagingUnit.Name}'s Lifelink!");
        cardGame.HealingSystem.HealPlayer(cardGame, playerToGainLife, damage);
    }
}

public class DeathtouchAbility : CardAbility, IOnDamageDealt
{
    public override string RulesText => "Deathtouch";
    public DeathtouchAbility()
    {
        Type = "Deathtouch";
    }

    public void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Filter out damage events that are not dealing to units, or else this will crash.
        if (damagedUnit == null)
        {
            return;
        }
        //Need a way to find out who owns which unit
        //hack - setting toughness to 0.
        //later on we will probably have some sort of DestroyingSystem and we would call that instead.
        cardGame.Log($"{damagedUnit.Name} died from {damagingUnit.Name}'s deathtouch!");
        damagedUnit.Toughness = 0;
    }
}
public interface IModifyCanAttackDirectly
{
    bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane);
}
public class FlyingAbility : CardAbility, IModifyCanAttackDirectly
{
    public override string RulesText => "Flying";
    public FlyingAbility()
    {
        Type = "Flying";
    }

    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        var defendingUnit = defendingLane.UnitInLane;
        //If the other unit does not have flying, then this creature can attack directly.
        if (defendingUnit.Abilities.Where(ab => ab is FlyingAbility).Count() > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public class UnblockableAbility : CardAbility, IModifyCanAttackDirectly
{
    public override string RulesText => "Unblockable";
    public UnblockableAbility()
    {
        Priority = 10; //Unblockable should take priority over any IModifyCanAttackDirectly Ability.
    }
    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        return true;
    }
}

public interface IModifyCanBeTargeted
{
    bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect);
}


public class ShroudAbility : CardAbility, IModifyCanBeTargeted
{
    public override string RulesText => "Shroud";

    public bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect)
    {
        return false;
    }
}

public class HexproofAbility : CardAbility, IModifyCanBeTargeted
{
    public override string RulesText => "Hexproof";

    public bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect)
    {
        return cardGame.GetOwnerOfCard(unitWithAbility) == ownerOfEffect;
    }
}

public interface IModifyCanAttack
{
    public bool CanAttack(CardGame cardGame, CardInstance card);

}

public class HasteAbility : CardAbility, IModifyCanAttack
{
    public override string RulesText => "Haste";

    public HasteAbility()
    {
        //should always apply before any other effects, if something external causes the unit to not be able to attack then it should take preference.
        Priority = -1;
    }

    public bool CanAttack(CardGame cardGame, CardInstance card)
    {
        return true;
    }
    //The ability should let the unit attack the same turn it comes into play, but it should not override any "Can't Attack" effects.
}

//The logic of trample will be handled directly in the DefaultBattleSystem.cs class.
public class TrampleAbility : CardAbility
{
    public override string RulesText => "Trample";
}


public enum TriggerType
{
    SelfEntersPlay,
    SelfDies,
    SelfAttacks,
    AtTurnStart,
    AtTurnEnd
}


public class TriggeredAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            string text = "";

            switch (TriggerType)
            {
                case TriggerType.SelfEntersPlay:
                    text = "When this enters play ";
                    break;
                case TriggerType.SelfDies:
                    text = "When this dies ";
                    break;
                case TriggerType.SelfAttacks:
                    text = "When this attacks ";
                    break;
                case TriggerType.AtTurnStart:
                    text = "At the start of your turn ";
                    break;
                default:
                    text += "";
                    break;
            }

            foreach (var effect in Effects)
            {
                text += effect.RulesText;
                text += ",";
            }

            //Get rid of the last ",";
            text = text.Substring(0, text.Length - 1);


            return text;
        }
    }
    public TriggerType TriggerType { get; set; }
    public List<Effect> Effects { get; set; }

    public TriggeredAbility(TriggerType triggerType, Effect effect)
    {
        TriggerType = triggerType;
        Effects = new List<Effect>();
        Effects.Add(effect);
    }
}

public abstract class Effect
{
    public abstract string RulesText { get; }
    public virtual TargetType TargetType { get; set; }
}

public enum TargetType
{
    None,
    Self, //Player
    Opponent,
    UnitSelf, //Self Unit
    TargetPlayers,
    TargetUnits,
    TargetUnitsOrPlayers,
    OpenLane,
    OpenLaneBesideUnit, //mainly for token creation, tries to place the token nearest left or right to the unit that is creating it.
}

//Damage Abiltiies are handled by the DamageSystem themselves?
public class DamageEffect : Effect
{
    public override string RulesText => $"Deal {Amount} Damage";
    public int Amount { get; set; }

    public override TargetType TargetType { get; set; } = TargetType.TargetUnitsOrPlayers;
}

public class LifeGainEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Life";
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class PumpUnitEffect : Effect
{
    public override string RulesText => $"Target Unit gets +{Power}/+{Toughness}";
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
}

public class DrawCardEffect : Effect
{
    public override string RulesText => $"Draw {Amount} Cards";
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class AddManaEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Mana";
    public int Amount;
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class AddTempManaEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Mana until end of turn";
    public int Amount;
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class DarkConfidantEffect : Effect
{
    public override string RulesText => $"Draw a card and lose life equal to its mana cost";
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class SacrificeSelfEffect : Effect
{
    public override string RulesText => "Sacrifice this unit";
    public override TargetType TargetType { get; set; } = TargetType.UnitSelf; //Should never need to change.
}

public class CreateTokenEffect : Effect
{
    public override string RulesText => $"Create a {TokenData.Power}/{TokenData.Toughness} {TokenData.Name} token with {TokenData.RulesText}";
    public UnitCardData TokenData { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.OpenLane;

    public CreateTokenEffect(UnitCardData cardData)
    {
        TokenData = cardData;
    }
}

public class TransformEffect : Effect
{
    public override string RulesText => $"Transform into {TransformData.Name}";
    public UnitCardData TransformData { get; set; }

    public override TargetType TargetType { get; set; } = TargetType.None;
}
