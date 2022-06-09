using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    public void ActivateAbility(CardGame cardGame, Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo);
    bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card);
    public bool CanPayAdditionalCost(CardGame cardGame, Player player, CardInstance source, AdditionalCost cost);
}

public class DefaultActivatedAbilitySystem : IActivatedAbilitySystem
{

    //Need one with targets
    private void ActivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        ActivateAbilityWithTargets(cardGame, player, card, new List<CardGameEntity>());
    }

    public void ActivateAbility(CardGame cardGame, Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo)
    {
        //Do some checks here.

        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();
        
        //Validating if we have the proper info to be able to correctly activate the ability;
        if (activateAbilityInfo == null && (activatedAbility.HasTargets() || activatedAbility.HasChoices()))
        {
            throw new Exception("Cannot activate the ability, need ActivateAbilityInfo for either/or targets or choices");        
        }
        if  (activatedAbility.HasTargets() && !(activateAbilityInfo.Targets.Any()))
        {
            throw new Exception("Cannot activate the ability, we need targets but no targets were specified in ActivateAbilityInfo");
        }
        if (activatedAbility.HasChoices() && !(activateAbilityInfo.Choices.Any()))
        {
            throw new Exception("Cannot activate the ability, we need choices but no choices were specified in ActivatedAbilityInfo");
        }

        //Pay any additional costs.
        if (activatedAbility.HasAdditionalCost())
        {
            PayAdditionalCost(cardGame, player, card,activatedAbility.AdditionalCost,new CostInfo() { EntitiesChosen = activateAbilityInfo.Choices });
        }

        if (activatedAbility.HasTargets())
        {
            ActivateAbilityWithTargets(cardGame, player, card, activateAbilityInfo.Targets);
        }
        else
        {
            ActivateAbility(cardGame, player, card);
        }
    }

    private void PayAdditionalCost(CardGame cardGame, Player player, CardInstance card, AdditionalCost additionalCost, CostInfo costInfo)
    {
        additionalCost.PayCost(cardGame, player, card,costInfo);
    }

    public void ActivateAbilityWithTargets(CardGame cardGame, Player player, CardInstance card, List<CardGameEntity> targets)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility == null) { return; }

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

public class ActivateAbilityInfo
{
    public List<CardGameEntity> Targets { get; set; }
    public List<CardGameEntity> Choices { get; set; }

    public ActivateAbilityInfo()
    {
        Targets = new List<CardGameEntity>();
        Choices = new List<CardGameEntity>();
    }

}