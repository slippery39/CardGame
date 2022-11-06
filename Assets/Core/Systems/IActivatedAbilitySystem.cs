using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    [Obsolete]
    bool CanActivateAbility(Player player, CardInstance card);
    bool CanActivateAbility(CardInstance source, ActivatedAbility ability);
    public void ActivateAbililty(Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo);
}

public class DefaultActivatedAbilitySystem : CardGameSystem, IActivatedAbilitySystem
{

    public DefaultActivatedAbilitySystem(CardGame cardGames)
    {
        cardGame = cardGames;
    }
    public void ActivateAbililty(Player player, CardInstance card, ActivateAbilityInfo activateAbilityInfo)
    {
        //TODO - Get Ability that matches Activation Zone
        var activatedAbility = card.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault(a => a.ActivationZone == cardGame.GetZoneOfCard(card).ZoneType);

        //Might be a bug here with ZoneType.Items

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

        cardGame.ManaSystem.SpendMana(player, activatedAbility.ManaCost);

        //Pay any additional costs.
        if (activatedAbility.HasAdditionalCost())
        {
            cardGame.AdditionalCostSystem.PayAdditionalCost(player, card, activatedAbility.AdditionalCost, new CostInfo() { EntitiesChosen = activateAbilityInfo.Choices });
        }

        //If its a once per turn ability, place an ability cooldown component on it.
        if (activatedAbility.OncePerTurnOnly)
        {
            activatedAbility.Components.Add(new AbilityCooldown());
        }

        //Exhaust the card if nec essary
        if (activatedAbility.ExhaustOnUse)
        {
            card.IsExhausted = true;
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

    public bool CanActivateAbility(CardInstance card, ActivatedAbility activatedAbility)
    {
        //Need To Check The Zone
        if (activatedAbility.ActivationZone != card.GetZone().ZoneType)
        {
            return false;
        }

        //Activated Abilities can only be activated during the active players turn.
        if (cardGame.GetOwnerOfCard(card)!= cardGame.ActivePlayer)
        {
            return false;
        }
        //Changing to take into account exhaustion.        
        if (activatedAbility.ExhaustOnUse && card.IsExhausted)
        {
            cardGame.Log("Cannot use ability because the card is exhausted");
            return false;
        }

        //For abilities that can only be used once per turn, but do not exhaust the card (i.e. Basking Rootwalla's Pump Ability)
        if (activatedAbility.GetComponent<AbilityCooldown>() != null)
        {
            cardGame.Log("Cannot use ability because its on cooldown");
            return false;
        }

        if (activatedAbility == null) { return false; }

        var player = cardGame.GetOwnerOfCard(card);

        var canPayManaCost = cardGame.ManaSystem.CanPayManaCost(player, (string)activatedAbility.ManaCost);
        var canPayAdditionalCost = true;

        if (activatedAbility.HasAdditionalCost())
        {
            canPayAdditionalCost = cardGame.AdditionalCostSystem.CanPayAdditionalCost(player, (CardInstance)card, activatedAbility.AdditionalCost);
        }

        return canPayManaCost && canPayAdditionalCost;
    }

    [Obsolete]
    public bool CanActivateAbility(Player player, CardInstance card)
    {
        //TODO - Multiple Activated Abilities - Check ActivateZone
        var activatedAbility = card.GetAbilitiesAndComponents<ActivatedAbility>().FirstOrDefault();

        //Changing to take into account exhaustion.        
        if (activatedAbility.ExhaustOnUse && card.IsExhausted)
        {
            cardGame.Log("Cannot use ability because the card is exhausted");
            return false;
        }

        //For abilities that can only be used once per turn, but do not exhaust the card (i.e. Basking Rootwalla's Pump Ability)
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
    public List<CardGameEntity> Targets { get; set; } = new List<CardGameEntity>();
    public List<CardGameEntity> Choices { get; set; } = new List<CardGameEntity>();

    public ActivateAbilityInfo()
    {
    }

}