using System;
using System.Collections.Generic;
using System.Linq;

public interface IActivatedAbilitySystem
{
    //Assumes the card only has 1 activated ability.
    void AcivateAbility(CardGame cardGame, Player player, CardInstance card);
    void ActivateAbilityWithTargets(CardGame cardGame, Player player, CardInstance card, List<CardGameEntity> targets);
    bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card);
}

public class DefaultActivatedAbilitySystem : IActivatedAbilitySystem
{

    //Need one with targets
    public void AcivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        ActivateAbilityWithTargets(cardGame,player,card,new List<CardGameEntity>());
    }

    public void ActivateAbilityWithTargets(CardGame cardGame, Player player, CardInstance card,List<CardGameEntity> targets)
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

    public bool CanActivateAbility(CardGame cardGame, Player player, CardInstance card)
    {
        var activatedAbility = card.GetAbilities<ActivatedAbility>().FirstOrDefault();
        if (activatedAbility == null) { return false; }
        return cardGame.ManaSystem.CanPayManaCost(cardGame, player, activatedAbility.ManaCost);
    }
}