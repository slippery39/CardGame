using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    //Assumes the card only has 1 activated ability.
    void ActivateAbility(CardGame cardGame, Player player, CardInstance card);
    void ActivateAbilityWithTargets(CardGame cardGame, Player player, CardInstance card, List<CardGameEntity> targets);
    bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card);
    public void ActivateAbilityWithAdditionalCosts(CardGame cardGame, Player player, CardInstance card);
    public void PayAdditionalCost(CardGame cardGame, Player player, CardInstance card, AdditionalCost additionalCost);
    public bool CanPayAdditionalCost(CardGame cardGame, Player player, CardInstance source, AdditionalCost cost);
}

public class DefaultActivatedAbilitySystem : IActivatedAbilitySystem
{

    //Need one with targets
    public void ActivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        ActivateAbilityWithTargets(cardGame, player, card, new List<CardGameEntity>());
    }
    public void ActivateAbilityWithAdditionalCosts(CardGame cardGame, Player player, CardInstance card)
    {
        //This assumes we have already checked to see if the cost can be payed.
        //This is also for non choice costs (i.e. like paying life).
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();
        var additionalCost = activatedAbility.AdditionalCost;
        PayAdditionalCost(cardGame, player, card, additionalCost);
        ActivateAbility(cardGame, player, card);
    }

    public void PayAdditionalCost(CardGame cardGame, Player player, CardInstance card, AdditionalCost additionalCost)
    {
        additionalCost.PayCost(cardGame, player, card);
    }

    public void ActivateAbilityWithTargets(CardGame cardGame, Player player, CardInstance card, List<CardGameEntity> targets)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility == null) { return; }

        //Assuming additional costs are auto payed for now, for ones that need choices, we will need to change this.
        if (activatedAbility.HasAdditionalCost())
        {
            PayAdditionalCost(cardGame, player, card, activatedAbility.AdditionalCost);
        }

        cardGame.ManaSystem.SpendManaAndEssence(cardGame, player, activatedAbility.ManaCost);

        cardGame.EffectsProcessor.ApplyEffects(
            cardGame,
            player,
            card,
            new List<Effect> { activatedAbility.AbilityEffect },
            targets);
    }

    public bool CanPayAdditionalCost(CardGame cardGame, Player player, CardInstance source, AdditionalCost cost)
    {
        return cost.CanPay(cardGame, player, source);
    }

    public bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();
        if (activatedAbility == null) { return false; }

        var canPayManaCost = cardGame.ManaSystem.CanPayManaCost(cardGame, player, activatedAbility.ManaCost);
        var canPayAdditionalCost = true;

        if (activatedAbility.HasAdditionalCost())
        {
            canPayAdditionalCost = CanPayAdditionalCost(cardGame, player, card, activatedAbility.AdditionalCost);
        }

        return canPayManaCost && canPayAdditionalCost;
    }
}