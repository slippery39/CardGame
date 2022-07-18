using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardAbility
{
    public string Type;
    public int Priority { get; set; }
    public abstract string RulesText { get; }
    public bool ThisTurnOnly { get; set; } = false;
    public List<AbilityComponent> Components { get; set; } = new List<AbilityComponent>();

    public CardAbility Clone()
    {
        CardAbility clone = (CardAbility)MemberwiseClone();
        clone.Components = Components.ToList();
        return clone;
    }

    public T GetComponent<T>()
    {
        return Components.Where(c => c is T).Cast<T>().FirstOrDefault();
    }
}

public abstract class AbilityComponent
{

}

public class AbilityCooldown : AbilityComponent
{

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
        Player playerToGainLife = cardGame.GetOwnerOfCard(damagingUnit);
        cardGame.Log($"{playerToGainLife} gained {damage} life from {damagingUnit.Name}'s Lifelink!");
        cardGame.HealingSystem.HealPlayer(playerToGainLife, damage);
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
        damagedUnit.BaseToughness = 0;
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

public class MadnessAbility : CardAbility
{
    public string ManaCost { get; set; }
    public override string RulesText => $"Madness : {ManaCost}";
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

    public TriggeredAbility()
    {

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
    AllUnits,
    OurUnits,
    OpponentUnits,
    UnitSelf, //Self Unit
    TargetPlayers,
    TargetUnits,
    TargetUnitsOrPlayers,
    OpenLane,
    OpenLaneBesideUnit, //mainly for token creation, tries to place the token nearest left or right to the unit that is creating it.
}

public static class TargetTypeHelper
{
    public static string TargetTypeToRulesText(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.AllUnits: return "each unit";
            case TargetType.OurUnits: return "each unit you control";
            case TargetType.OpponentUnits: return "each unit your opponent controls";
            case TargetType.TargetUnits: return "target unit";
            case TargetType.TargetPlayers: return "target player";
            case TargetType.TargetUnitsOrPlayers: return "target unit or player";
            case TargetType.UnitSelf: return "to itself";
            case TargetType.Self: return "to itself";
            default: return "";
        }
    }
}

//Damage Abiltiies are handled by the DamageSystem themselves?
public class DamageEffect : Effect
{
    public override string RulesText => $"Deal {Amount} Damage to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
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
    public override string RulesText
    {
        get
        {
            var powerSymbol = Power >= 0 ? "+" : "-";
            var toughnessSymbol = Toughness > 0 ? "+" : "-";
            var rulesText = $"Give {powerSymbol}{Power}/{toughnessSymbol}{Toughness} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
            return rulesText;
        }
    }
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
}

public class DrawCardEffect : Effect
{
    public override string RulesText
    {
        get
        {

            if (Amount == 1)
            {
                return "Draw a card";
            }
            else
            {
                return $"Draw {Amount} Cards";
            }
        }
    }

    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class AddManaEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Mana";
    public int Amount;
    public ManaType ManaType { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}

public class AddTempManaEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Mana until end of turn";
    public ManaType ManaType { get; set; }
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
    //TODO - change the rules text to take into account the amount of tokens created.
    public override string RulesText => $"Create a {TokenData.Power}/{TokenData.Toughness} {TokenData.Name} token with {TokenData.RulesText}";
    public UnitCardData TokenData { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.OpenLane;
    public int AmountOfTokens { get; set; } = 1;

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

public class DestroyEffect : Effect
{
    public override string RulesText => $"Destroy {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
}

public class AddTempAbilityEffect : Effect
{
    public override string RulesText => $@"Give {TempAbility.RulesText} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)} until end of turn";
    public CardAbility TempAbility { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
    public AddTempAbilityEffect(CardAbility tempAbility)
    {
        TempAbility = tempAbility;
        TempAbility.ThisTurnOnly = true;
    }
}

public class GoblinPiledriverEffect : Effect
{
    public override string RulesText => $@"Gets +2/+0 for each goblin you control";
    public override TargetType TargetType { get; set; } = TargetType.UnitSelf;
}

public class GetRandomCardFromDeckEffect : Effect
{

    public override string RulesText
    {
        get
        {
            var str = "draw a random #cardType# from your deck";

            if (Filter?.CreatureType != null)
            {
                return str.Replace("#cardType#", Filter.CreatureType);
            }
            else
            {
                return str.Replace("#cardType#", "card");
            }
        }
    }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public CardFilter Filter { get; set; }
}

public class GrabFromTopOfDeckEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var str = $"draw up to {Amount} #cardType# from the top {CardsToLookAt} cards of your deck";

            if (Filter?.CreatureType != null)
            {
                return str.Replace("#cardType#", Filter.CreatureType);
            }
            else
            {
                return str.Replace("#cardType#", "card");
            }
        }
    }
    public int CardsToLookAt { get; set; }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public CardFilter Filter { get; set; }
}

public class DiscardCardEffect : Effect
{
    public override string RulesText
    {
        get
        {
            if (Amount == 1)
            {
                return "Discard a card";
            }
            else
            {
                return $@"Discard {Amount} Cards";
            }
        }
    }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;

}

public class SwitchPowerToughnessEffect : Effect
{
    public override string RulesText
    {
        get
        {
            return "Switch power and toughness";
        }
    }
}

public class PumpPowerByNumberOfArtifactsEffect : Effect
{

    public int CountArtifacts(CardGame cardGame, Player player)
    {
        var thingsInPlay = cardGame.GetUnitsInPlay().Where(u => cardGame.GetOwnerOfCard(u) == player).ToList();
        thingsInPlay.AddRange(player.Items.Cards);
        return thingsInPlay.Where(thing => thing.Subtype.ToLower() == "artifact").Count();
    }

    public override string RulesText
    {
        get
        {
            return $"A unit gets +X/+0 until end of turn where X is the amount of artifacts you control.";
        }
    }
}

public class GiveShieldEffect : Effect
{
    public override string RulesText
    {
        get
        {
            return $"Give a shield";
        }
    }
}


//Represents an effect with multiple components.
public class CompoundEffect : Effect
{
    public List<Effect> Effects { get; set; }
    public override string RulesText => string.Join("\r\n", Effects.Select(e => e.RulesText));
}

public interface IModifyManaCost
{
    string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost);

}

public class ModifyManaCostComponent : AbilityComponent, IModifyManaCost
{

    private Func<CardGame, CardInstance, string, string> _modManaCostFunc;

    public ModifyManaCostComponent(Func<CardGame, CardInstance, string, string> modManaCostFunc)
    {
        _modManaCostFunc = modManaCostFunc;
    }
    public string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost)
    {
        return _modManaCostFunc(cardGame, card, originalManaCost);
    }
}

public interface IModifyCastZones
{
    List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones);

}

public class ModifyCastZonesComponent : AbilityComponent, IModifyCastZones
{
    private Func<CardGame, CardInstance, List<ZoneType>, List<ZoneType>> _modCastZoneFunc;

    public ModifyCastZonesComponent(Func<CardGame, CardInstance, List<ZoneType>, List<ZoneType>> modCastZoneFunc)
    {
        _modCastZoneFunc = modCastZoneFunc;
    }

    public List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones)
    {
        return _modCastZoneFunc(cardGame, card, originalCastZones);
    }
}

public interface IModifyAdditionalCost
{
    AdditionalCost ModifyAdditionalCost(CardGame cardGame, CardInstance sourceCard, AdditionalCost originalAdditionalCost);
}

public class ModifyAdditionalCostComponent : AbilityComponent, IModifyAdditionalCost
{
    private Func<CardGame, CardInstance, AdditionalCost, AdditionalCost> _modAdditionalCostFunc;

    public ModifyAdditionalCostComponent(Func<CardGame, CardInstance, AdditionalCost, AdditionalCost> modCastZoneFunc)
    {
        _modAdditionalCostFunc = modCastZoneFunc;
    }

    public AdditionalCost ModifyAdditionalCost(CardGame cardGame, CardInstance card, AdditionalCost originalAdditionalCost)
    {
        return _modAdditionalCostFunc(cardGame, card, originalAdditionalCost);
    }
}


public class FlashbackAbility : CardAbility
{
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; }
    public override string RulesText => $"Flashback : {ManaCost},{AdditionalCost.RulesText}";

    //Need to change things to have a cost, not just a mana cost.
    private string ChangeManaCost(CardGame cardGame, CardInstance cardInstance, string originalManaCost)
    {
        if (cardGame.GetZoneOfCard(cardInstance).ZoneType == ZoneType.Discard)
        {
            return ManaCost;
        }
        else
        {
            return originalManaCost;
        }
    }

    private List<ZoneType> ChangeCastZones(CardGame cardGame, CardInstance cardInstance, List<ZoneType> originalCastZones)
    {
        var modifiedCastZones = originalCastZones.ToList();
        if (!originalCastZones.Contains(ZoneType.Discard))
        {
            modifiedCastZones.Add(ZoneType.Discard);
        }

        return modifiedCastZones;
    }

    private AdditionalCost ChangeAdditionalCost(CardGame cardGame, CardInstance cardInstance, AdditionalCost originalAdditionalCost)
    {

        if (cardGame.GetZoneOfCard(cardInstance).ZoneType == ZoneType.Discard)
        {
            return new PayLifeAdditionalCost
            {
                Amount = 3
            };
        }
        else
        {
            return originalAdditionalCost;
        }
    }

    public FlashbackAbility()
    {
        this.Components.Add(new ModifyManaCostComponent(ChangeManaCost));
        this.Components.Add(new ModifyCastZonesComponent(ChangeCastZones));
        this.Components.Add(new ModifyAdditionalCostComponent(ChangeAdditionalCost));
    }
}


public class AffinityAbility : CardAbility
{
    public override string RulesText => $"Affinity for artifacts"; //only for artifacts right now.

    private string ChangeManaCost(CardGame cardGame, CardInstance cardInstance, string originalManaCost)
    {
        //We need to count the amount of artifacts in play for the controller.
        var cardOwner = cardGame.GetOwnerOfCard(cardInstance);
        var artifactCounts = cardGame.GetCardsInPlay(cardOwner).Where(c => c.Subtype.ToLower() == "artifact").Count();

        //Subtract the artifact counts from the colorless mana;
        var manaCostAsObj = new Mana(originalManaCost);

        manaCostAsObj.ColorlessMana -= artifactCounts;
        manaCostAsObj.ColorlessMana = Math.Max(0, manaCostAsObj.ColorlessMana);

        return manaCostAsObj.ToManaString();
    }

    public AffinityAbility()
    {
        this.Components.Add(new ModifyManaCostComponent(ChangeManaCost));
    }
}

public interface IOnSummon
{
    void OnSummoned(CardGame cardGame, CardInstance source);
}

public interface IOnDeath
{
    void OnDeath(CardGame cardGame, CardInstance source);
}

public class ModularAbility : CardAbility, IOnSummon, IOnDeath
{
    public int Amount { get; set; }
    public override string RulesText => $"Modular {Amount}";

    public void OnSummoned(CardGame cardGame, CardInstance source)
    {
        cardGame.CountersSystem.AddPlusOnePlusOneCounter(source, Amount);
    }

    public void OnDeath(CardGame cardGame, CardInstance source)
    {
        //We want to move our +1/+1 counters to a new artifact creature, prioritizing another modular creature if available.
        var creaturesInPlay = cardGame.GetOwnerOfCard(source).GetUnitsInPlay();

        var modularCreatures = creaturesInPlay.Where(c => c.GetAbilitiesAndComponents<ModularAbility>().Any());

        var amountOfCounters = source.Counters.GetOfType<PlusOnePlusOneCounter>().Count();

        if (modularCreatures.Any())
        {
            //pick a random one
            var selectedCreature = modularCreatures.Randomize().ToList()[0];
            cardGame.CountersSystem.AddPlusOnePlusOneCounter(selectedCreature, amountOfCounters);
        }
        else
        {
            var artifactCreatures = creaturesInPlay.Where(c => c.Subtype == "Artifact").ToList();

            if (artifactCreatures.Any())
            {
                var selectedCreature = artifactCreatures.Randomize().ToList()[0];
                cardGame.CountersSystem.AddPlusOnePlusOneCounter(selectedCreature, amountOfCounters);
            }
        }
    }
}

public class AddPlusOnePlusOneCounterEffect : Effect
{
    public int Amount { get; set; }

    public override string RulesText => "Add a +1/+1 counter";
}


