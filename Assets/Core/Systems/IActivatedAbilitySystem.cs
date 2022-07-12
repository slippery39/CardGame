using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    bool CanActivateAbility(Player player, CardInstance card);
    public void ActivateAbililty(Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo);
}

public class DefaultActivatedAbilitySystem : IActivatedAbilitySystem
{
    //Temporary method, testing stuff out to get this working better
    private CardGame cardGame;

    public DefaultActivatedAbilitySystem(CardGame cardGames)
    {
        cardGame = cardGames;
    }
    public void ActivateAbililty(Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        //Validating if we have the proper info to be able to correctly activate the ability;
        if (activateAbilityInfo == null && (activatedAbility.HasTargets() || activatedAbility.HasAdditionalCostChoices()))
        {
            throw new Exception("Cannot activate the ability, need ActivateAbilityInfo for either/or targets or choices");
        }
        if (activatedAbility.HasTargets() && !(activateAbilityInfo.Targets.Any()))
        {
            throw new Exception("Cannot activate the ability, we need targets but no targets were specified in ActivateAbilityInfo");
        }
        if (activatedAbility.HasAdditionalCostChoices() && !(activateAbilityInfo.Choices.Any()))
        {
            throw new Exception("Cannot activate the ability, we need choices but no choices were specified in ActivatedAbilityInfo");
        }

        cardGame.ManaSystem.SpendManaAndEssence(player, activatedAbility.ManaCost);

        //Pay any additional costs.
         if (activatedAbility.HasAdditionalCost())
        {
            cardGame.AdditionalCostSystem.PayAdditionalCost(player, card, activatedAbility.AdditionalCost, new CostInfo() { EntitiesChosen = activateAbilityInfo.Choices });
        }

        //If its a once per turn ability, place an ability cooldown component on it.
        if (activatedAbility.OncePerTurn)
        {
            activatedAbility.Components.Add(new AbilityCooldown());
        }

        //Check if it has targets or not to call the appropriate method.
        if (activatedAbility.HasTargets())
        {
            cardGame.ResolvingSystem.Add(activatedAbility, card, activateAbilityInfo.Targets.First());
        }
        else
        {
            cardGame.ResolvingSystem.Add(activatedAbility, card);
        }
    }


    public bool CanActivateAbility(Player player, CardInstance card)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility.GetComponent<AbilityCooldown>() != null)
        {
            cardGame.Log("Cannot use ability because its on cooldown");
            return false;
        }

        if (activatedAbility == null) { return false; }

        var canPayManaCost = cardGame.ManaSystem.CanPayManaCost(player, activatedAbility.ManaCost);
        var canPayAdditionalCost = true;

        if (activatedAbility.HasAdditionalCost())
        {
            canPayAdditionalCost = cardGame.AdditionalCostSystem.CanPayAdditionalCost(player, card, activatedAbility.AdditionalCost);
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