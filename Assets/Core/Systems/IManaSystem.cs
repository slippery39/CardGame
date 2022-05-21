using System;
using System.Linq;

public interface IManaSystem
{
    void AddMana(CardGame cardGame, Player player, int amount);
    void AddEssence(CardGame cardGame, Player player, EssenceType essenceType, int amount);
    void AddTemporaryManaAndEssence(CardGame cardGame, Player player, EssenceType manaType, int amount);
    void AddTemporaryEssence(CardGame cardGame, Player player, EssenceType essenceType, int amount);
    void SpendManaAndEssence(CardGame cardGame, Player player, string cost);
    void ResetManaAndEssence(CardGame cardGame, Player player);
    bool CanPlayCard(CardGame cardGame, Player player, CardInstance card);
    bool CanPlayManaCard(CardGame cardGame, Player player, CardInstance card);
    void PlayManaCard(CardGame cardGame, Player player, CardInstance card);

    /// <summary>
    /// Checks whether a player can theoretically pay a mana cost. Mana Cost is passed in as a string format. 
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <param name="manaCost"></param>
    bool CanPayManaCost(CardGame cardGame, Player player, string manaCost);

}

public class DefaultManaSystem : IManaSystem
{
    //Adds mana to the mana pool without effecting the total amount.
    public void AddTemporaryManaAndEssence(CardGame cardGame, Player player, EssenceType manaType, int amount)
    {
        player.ManaPool.AddTemporaryEssenceAndMana(manaType, amount);
    }

    public void AddMana(CardGame cardGame, Player player, int amount)
    {
        player.ManaPool.AddMana(amount);
    }

    public void SpendManaAndEssence(CardGame cardGame, Player player, string cost)
    {
        //TODO - change this.

        var costInManaAndEssence = new ManaAndEssence(cost);

        //Spend the mana;
        if (costInManaAndEssence.Mana > 0)
        {
            player.ManaPool.SpendMana(costInManaAndEssence.Mana);
        }

        if (costInManaAndEssence.TotalSumOfEssence > 0)
        {
            //Spend the essence.
            foreach (var color in costInManaAndEssence.Essence.Keys)
            {
                player.ManaPool.SpendEssence(color, costInManaAndEssence.Essence[color]);
            }
        }
    }

    public void ResetManaAndEssence(CardGame cardGame, Player player)
    {
        player.ManaPool.ResetManaAndEssence();
    }


    //How to we convert a costToPay into a ManaPool?
    public bool CanPayManaCost(ManaAndEssence costToPay, ManaAndEssence payingPool)
    {
        return payingPool.IsEnoughToPayCost(costToPay);
    }

    public bool CanPlayCard(CardGame cardGame, Player player, CardInstance card)
    {
        //TODO - Handle Non Integer Mana Costs.
        return CanPayManaCost(new ManaAndEssence(card.ManaCost), player.ManaPool.CurrentManaAndEssence);
    }

    public bool CanPlayManaCard(CardGame cardGame, Player player, CardInstance card)
    {
        return player.ManaPlayedThisTurn < player.TotalManaThatCanBePlayedThisTurn;
    }

    public void PlayManaCard(CardGame cardGame, Player player, CardInstance card)
    {
        player.ManaPlayedThisTurn++;
        var manaCard = card.CurrentCardData as ManaCardData;

        var manaAndEssenceCounts = new ManaAndEssence(manaCard.ManaAdded);

        //Mana is easy enough, just add the mana count stated in the ManaAndEssence object

        //For now, we are going to make our mana cards simple, they will always just add the total essence as mana.
        AddMana(cardGame, player, manaAndEssenceCounts.TotalSumOfEssence);

        var essence = manaAndEssenceCounts.Essence;

        var essenceAdded = essence.Keys.Where(k => essence[k] > 0);

        foreach (var essenceType in essence.Keys)
        {
            if (manaAndEssenceCounts.Essence[essenceType] > 0)
            {
                AddEssence(cardGame, player, essenceType, essence[essenceType]);
            }
        }

        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.DiscardPile);
    }

    public void AddEssence(CardGame cardGame, Player player, EssenceType essenceType, int amount)
    {
        player.ManaPool.AddEssence(essenceType, amount);
    }

    public bool CanPayManaCost(CardGame cardGame, Player player, string manaCost)
    {
        return (player.ManaPool.CurrentManaAndEssence.IsEnoughToPayCost(new ManaAndEssence(manaCost))); ;
    }

    public void AddTemporaryEssence(CardGame cardGame, Player player, EssenceType essenceType, int amount)
    {
        player.ManaPool.AddTemporaryEssence(essenceType, amount);
    }
}