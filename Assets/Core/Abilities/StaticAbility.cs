using Assets.Core;
using System;
using System.Collections.Generic;
using System.Linq;

public class StaticAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            return String.Join(" and ", Effects.Select(eff =>
             {
                 return eff.RulesText;

             })
            )
            .UpperFirst();
        }
    }
    public ZoneType ApplyWhenIn { get; set; } = ZoneType.InPlay;
}

public class TarmogoyfAbility : CardAbility, IModifyPower, IModifyToughness
{
    public override string RulesText => "Tarmogoyf gets +X/+X where X is twice the number of unique card types in your graveyard";

    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        var uniqueCardsInGraveyard = card.GetOwner().DiscardPile.Select(c => c.CardType).Distinct().Count();
        return originalPower + (uniqueCardsInGraveyard * 2);
    }

    public int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness)
    {
        var uniqueCardsInGraveyard = card.GetOwner().DiscardPile.Select(c => c.CardType).Distinct().Count();
        return originalToughness + (uniqueCardsInGraveyard * 2);
    }
}


public class StaticPumpEffect : Effect
{
    public override string RulesText => $"{TargetInfo.GetRulesText()} gain {(Power >= 0 ? "+" : "-")}{Power}/{(Toughness >= 0 ? "+" : "-")}{Toughness}";
    public int Power { get; set; }
    public int Toughness { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;
            if (cardInstance == null)
            {
                continue;
            }
            var abilitySource = source.Abilities.First(ab => ab.Effects.Contains(this));

            var modification = new ModAddToPowerToughness
            {
                Power = Power,
                Toughness = Toughness,
                OneTurnOnly = false
            };

            modification.StaticInfo = new StaticInfo
            {
                SourceAbility = abilitySource,
                SourceCard = source
            };

            cardInstance.Modifications.Add(modification);
        }
    }
}

public class StaticManaReductionEffect : Effect
{
    public override string RulesText
    {
        get
        {
            //Generic

            //Cards you play cost 1 less;
            //Goblins you play cost 1 less;
            //Units you play cost 1 less;
            //#cardType# #targetType# cost 1 less;

            //Goblins cost 1 less

            //Target Type is handled by the parent ability of this effect, in there it also handles the CardFilter part of it.

            var str = $"#targetType# cost {ReductionAmount} less to play.";

            return str;
        }
    }
    public string ReductionAmount { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;
            if (cardInstance == null)
            {
                continue;
            }

            var manaModification = new ModReduceManaCost
            {
                ReductionAmount = ReductionAmount,
                OneTurnOnly = false
            };

            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).ToList();

            manaModification.StaticInfo = new StaticInfo
            {
                SourceAbility = abilitySource[0],
                SourceCard = source
            };

            cardInstance.Modifications.Add(manaModification);
        }
    }
}

public class ModReduceManaCost : Modification, IModifyManaCost
{
    public string ReductionAmount { get; set; }
    public string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost)
    {
        var costAsCounts = new Mana(originalManaCost);
        var reductionCostAsCounts = new Mana(ReductionAmount);

        costAsCounts.ColorlessMana -= reductionCostAsCounts.ColorlessMana;
        costAsCounts.ColorlessMana = Math.Max(0, costAsCounts.ColorlessMana);

        foreach (var essenceType in reductionCostAsCounts.ColoredMana)
        {
            costAsCounts.ColoredMana[essenceType.Key] -= essenceType.Value;
            costAsCounts.ColoredMana[essenceType.Key] = Math.Max(0, costAsCounts.ColoredMana[essenceType.Key]);
        }

        return costAsCounts.ToManaString();
    }
}

public class StaticRevealTopCardEffect : Effect
{
    public override string RulesText => "You can look at the top card of your deck";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var playerEntity = entity as Player;

            if (playerEntity == null)
            {
                continue;
            }

            var revealTopCardModification = new RevealTopCardModification()
            {
                OneTurnOnly = false
            };

            revealTopCardModification.StaticInfo = new StaticInfo
            {
                SourceAbility = source.Abilities.First(ab => ab.Effects.Contains(this)),
                SourceCard = source
            };

            playerEntity.Modifications.Add(revealTopCardModification);
        }
    }
}

public interface IOnAfterStateBasedEffects
{
    void OnAfterStateBasedEffects(CardGame cardGame, Player player);
}

public class RevealTopCardModification : Modification, IOnAfterStateBasedEffects
{
    public void OnAfterStateBasedEffects(CardGame cardGame, Player player)
    {
        if (player.Deck.GetTopCard() != null)
        {
            player.Deck.GetTopCard().RevealedToOwner = true;
        }
    }
}

public class StaticPlayAdditionalLandEffect : Effect
{
    public int Amount { get; set; }
    public override string RulesText => $"#cardType# may play an additional {Amount} mana cards each turn";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var playerEntity = entity as Player;

            if (playerEntity == null)
            {
                continue;
            }

            var manaPerTurnModification = new ManaPerTurnModification
            {
                Amount = Amount,
                OneTurnOnly = false
            };

            manaPerTurnModification.StaticInfo = new StaticInfo
            {
                SourceAbility = source.Abilities.First(ab => ab.Effects.Contains(this)),
                SourceCard = source
            };

            playerEntity.Modifications.Add(manaPerTurnModification);
        }
    }
}

public class OracleOfMulDayaEffect : Effect
{
    public override string RulesText => "You may play lands from the top of your deck";

    public OracleOfMulDayaEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var playerEntity = entity as Player;

            if (playerEntity == null)
            {
                continue;
            }

            var modification = new OracleOfMulDayaModification()
            {
                OneTurnOnly = false
            };

            modification.StaticInfo = new StaticInfo
            {
                SourceAbility = source.Abilities.First(ab => ab.Effects.Contains(this)),
                SourceCard = source
            };

            cardGame.PlayerAbilitySystem.GiveModification(playerEntity, modification);
        }
    }
}

public class OracleOfMulDayaModification : Modification, IModifyCastZones
{

    public List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones)
    {
        if (card.CardType == "Mana" && cardGame.GetOwnerOfCard(card).Deck.GetTopCard() == card)
        {
            return originalCastZones.Union(new List<ZoneType> { ZoneType.Deck }).ToList();
        }
        else
        {
            return originalCastZones;
        }
    }
}


public class StaticGiveAbilityEffect : Effect
{
    public override string RulesText => $"#targetType# gain {Ability.RulesText}.";
    public CardAbility Ability { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;

            if (cardInstance == null)
            {
                continue;
            }

            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).First();

            var abilityToGive = Ability.Clone();

            var sourceAbility = source.Abilities.First(ab => ab.Effects.Contains(this));

            if (sourceAbility == null)
            {
                cardGame.Log("Could not find the source ability for an effect... possibly it was modified before the effect could take place");
            }

            abilityToGive.Components.Add(new ContinuousAblityComponent
            {
                SourceEffect = this,
                SourceCard = source,
                SourceAbility = sourceAbility as StaticAbility
            });

            cardInstance.Abilities.Add(abilityToGive);
        }
    }
}







