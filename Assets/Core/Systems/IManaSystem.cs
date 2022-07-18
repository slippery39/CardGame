using System;
using System.Collections.Generic;
using System.Linq;

public interface IManaSystem
{
    void AddMana(Player player, int amount);
    void AddColoredMana(Player player, ManaType essenceType, int amount);
    void AddTemporaryManaAndEssence(Player player, ManaType manaType, int amount);
    void AddTemporaryEssence(Player player, ManaType essenceType, int amount);
    void SpendMana(Player player, string cost);
    void ResetManaAndEssence(Player player);
    bool CanPlayCard(Player player, CardInstance card);
    bool CanPlayManaCard(Player player, CardInstance card);
    void PlayManaCard(Player player, CardInstance card);

    /// <summary>
    /// Checks whether a player can theoretically pay a mana cost. Mana Cost is passed in as a string format. 
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <param name="manaCost"></param>
    bool CanPayManaCost(Player player, string manaCost);

}

public class DefaultManaSystem : IManaSystem
{
    private CardGame cardGame;

    public DefaultManaSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    //Adds mana to the mana pool without effecting the total amount.
    public void AddTemporaryManaAndEssence(Player player, ManaType manaType, int amount)
    {
        player.ManaPool.AddTemporaryColorAndColorless(manaType, amount);
    }

    public void AddMana(Player player, int amount)
    {
        player.ManaPool.AddColorlessMana(amount);
    }

    public void SpendMana(Player player, string cost)
    {
        //TODO - change this.

        var costInManaAndEssence = new Mana(cost);

        //Spend the mana;
        if (costInManaAndEssence.ColorlessMana > 0)
        {
            player.ManaPool.SpendColorlessMana(costInManaAndEssence.ColorlessMana);
        }

        if (costInManaAndEssence.TotalSumOfColoredMana > 0)
        {
            //Spend the essence.
            foreach (var color in costInManaAndEssence.ColoredMana.Keys)
            {
                player.ManaPool.SpendColoredMana(color, costInManaAndEssence.ColoredMana[color]);
            }
        }
    }

    public void ResetManaAndEssence(Player player)
    {
        player.ManaPool.ResetMana();
    }


    //How to we convert a costToPay into a ManaPool?
    public bool CanPayManaCost(Mana costToPay, Mana payingPool)
    {
        return payingPool.IsEnoughToPayCost(costToPay);
    }

    public bool CanPlayCard(Player player, CardInstance card)
    {
        //TODO - Handle Non Integer Mana Costs.
        return CanPayManaCost(new Mana(card.ManaCost), player.ManaPool.CurrentMana);
    }

    public bool CanPlayManaCard(Player player, CardInstance card)
    {
        return player.ManaPlayedThisTurn < player.TotalManaThatCanBePlayedThisTurn;
    }

    public void PlayManaCard(Player player, CardInstance card)
    {
        player.ManaPlayedThisTurn++;
        var manaCard = card.CurrentCardData as ManaCardData;

        var manaAndEssenceCounts = new Mana(manaCard.ManaAdded);

        //Mana is easy enough, just add the mana count stated in the ManaAndEssence object

        //For now, we are going to make our mana cards simple, they will always just add the total essence as mana.
        AddMana(player, manaAndEssenceCounts.ColorlessMana);

        var coloredMana = manaAndEssenceCounts.ColoredMana;

        var coloredManaAdded = coloredMana.Keys.Where(k => coloredMana[k] > 0);

        foreach (var manaType in coloredMana.Keys)
        {
            if (manaAndEssenceCounts.ColoredMana[manaType] > 0)
            {
                AddColoredMana(player, manaType, coloredMana[manaType]);
            }
        }

        //Trigger any SelfEnters play effects
        var selfEntersPlayEffects = card.GetAbilitiesAndComponents<TriggeredAbility>().Where(ta => ta.TriggerType == TriggerType.SelfEntersPlay);

        foreach (var ab in selfEntersPlayEffects)
        {
            cardGame.EffectsProcessor.ApplyEffects(player, card, ab.Effects, new List<CardGameEntity>());
        }

        cardGame.ZoneChangeSystem.MoveToZone(card, player.DiscardPile);
    }

    public void AddColoredMana(Player player, ManaType essenceType, int amount)
    {
        player.ManaPool.AddColor(essenceType, amount);
    }

    public bool CanPayManaCost(Player player, string manaCost)
    {
        return (player.ManaPool.CurrentMana.IsEnoughToPayCost(new Mana(manaCost))); ;
    }

    public void AddTemporaryEssence(Player player, ManaType essenceType, int amount)
    {
        player.ManaPool.AddTemporaryColor(essenceType, amount);
    }
}