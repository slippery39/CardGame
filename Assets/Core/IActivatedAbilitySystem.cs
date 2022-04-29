using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    //Assumes the card only has 1 activated ability.
    public void AcivateAbility(CardGame cardGame, Player player, CardInstance card);
    public bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card);
}

public class DefaultActivatedAbilitySystem : IActivatedAbilitySystem
{
    public void AcivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        //Grab the card's first result of an activated ability
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();

        if (activatedAbility == null) { return; }


        cardGame.ManaSystem.SpendMana(cardGame, player, Convert.ToInt32(activatedAbility.ManaCost));

        cardGame.EffectsProcessor.ApplyEffects(
            cardGame,
            player,
            card,
            new List<Effect> { activatedAbility.AbilityEffect },
            new List<CardGameEntity> { });
    }

    public bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();
        if (activatedAbility == null) { return false; }

        if (player.Mana >= Convert.ToInt32(activatedAbility.ManaCost))
        {
            return true;
        }
        return false;
        //Need to check if the player has enough mana to activate the ability
    }
}