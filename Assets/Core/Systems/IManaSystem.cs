using System;
using System.Linq;

public interface IManaSystem
{
    void AddMana(Player player, int amount);
    void AddEssence(Player player, EssenceType essenceType, int amount);
    void AddTemporaryManaAndEssence(Player player, EssenceType manaType, int amount);
    void AddTemporaryEssence(Player player, EssenceType essenceType, int amount);
    void SpendManaAndEssence(Player player, string cost);
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
    public void AddTemporaryManaAndEssence(Player player, EssenceType manaType, int amount)
    {
        player.ManaPool.AddTemporaryEssenceAndMana(manaType, amount);
    }

    public void AddMana(Player player, int amount)
    {
        player.ManaPool.AddMana(amount);
    }

    public void SpendManaAndEssence(Player player, string cost)
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

    public void ResetManaAndEssence(Player player)
    {
        player.ManaPool.ResetManaAndEssence();
    }


    //How to we convert a costToPay into a ManaPool?
    public bool CanPayManaCost(ManaAndEssence costToPay, ManaAndEssence payingPool)
    {
        return payingPool.IsEnoughToPayCost(costToPay);
    }

    public bool CanPlayCard(Player player, CardInstance card)
    {
        //TODO - Handle Non Integer Mana Costs.
        return CanPayManaCost(new ManaAndEssence(card.ManaCost), player.ManaPool.CurrentManaAndEssence);
    }

    public bool CanPlayManaCard(Player player, CardInstance card)
    {
        return player.ManaPlayedThisTurn < player.TotalManaThatCanBePlayedThisTurn;
    }

    public void PlayManaCard(Player player, CardInstance card)
    {
        player.ManaPlayedThisTurn++;
        var manaCard = card.CurrentCardData as ManaCardData;

        var manaAndEssenceCounts = new ManaAndEssence(manaCard.ManaAdded);

        //Mana is easy enough, just add the mana count stated in the ManaAndEssence object

        //For now, we are going to make our mana cards simple, they will always just add the total essence as mana.
        AddMana(player, manaAndEssenceCounts.TotalSumOfEssence);

        var essence = manaAndEssenceCounts.Essence;

        var essenceAdded = essence.Keys.Where(k => essence[k] > 0);

        foreach (var essenceType in essence.Keys)
        {
            if (manaAndEssenceCounts.Essence[essenceType] > 0)
            {
                AddEssence(player, essenceType, essence[essenceType]);
            }
        }

        cardGame.ZoneChangeSystem.MoveToZone(card, player.DiscardPile);
    }

    public void AddEssence(Player player, EssenceType essenceType, int amount)
    {
        player.ManaPool.AddEssence(essenceType, amount);
    }

    public bool CanPayManaCost(Player player, string manaCost)
    {
        return (player.ManaPool.CurrentManaAndEssence.IsEnoughToPayCost(new ManaAndEssence(manaCost))); ;
    }

    public void AddTemporaryEssence(Player player, EssenceType essenceType, int amount)
    {
        player.ManaPool.AddTemporaryEssence(essenceType, amount);
    }
}