using System;
using System.Linq;

public interface IManaSystem
{
    void AddMana(CardGame cardGame, Player player, ManaType manaType, int amount);
    void AddTemporaryMana(CardGame cardGame, Player player, ManaType manaType, int amount);
    void SpendMana(CardGame cardGame, Player player, int amount);
    void ResetMana(CardGame cardGame, Player player);
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
    public void AddTemporaryMana(CardGame cardGame, Player player, ManaType manaType, int amount)
    {
        player.ManaPool.AddTemporaryMana(manaType, amount);
    }

    public void AddMana(CardGame cardGame, Player player, ManaType manaType, int amount)
    {
        //TODO - Player will now have a mana pool.
        //We need to handle more than just an amount;
        player.ManaPool.AddMana(manaType, amount);
    }

    public void SpendMana(CardGame cardGame, Player player, int amount)
    {
        //TODO - change this.
        player.ManaPool.SpendMana(ManaType.Any, amount);
    }

    public void ResetMana(CardGame cardGame, Player player)
    {
        player.ManaPool.ResetMana();
    }

    public bool CanPlayCard(CardGame cardGame, Player player, CardInstance card)
    {
        //TODO - handle non integer mana costs.
        return player.Mana >= card.ConvertedManaCost;
    }

    public bool CanPlayManaCard(CardGame cardGame, Player player, CardInstance card)
    {
        return player.ManaPlayedThisTurn < player.TotalManaThatCanBePlayedThisTurn;
    }

    public void PlayManaCard(CardGame cardGame, Player player, CardInstance card)
    {
        player.ManaPlayedThisTurn++;
        var manaCard = card.CurrentCardData as ManaCardData;

        var manaCounts = ManaHelper.ManaStringToColorCounts(manaCard.ManaAdded);

        var manaAdded = manaCounts.Keys.Where(k => manaCounts[k] > 0);

        foreach (var manaType in manaCounts.Keys)
        {
            if (manaCounts[manaType] > 0)
            {
                AddMana(cardGame, player, manaType, manaCounts[manaType]);
            }
        }

        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.DiscardPile);
    }

    public bool CanPayManaCost(CardGame cardGame, Player player, string manaCost)
    {
        return (player.Mana >= GetConvertedManaCost(cardGame, manaCost));
    }

    /// <summary>
    /// Get the converted mana cost from a mana cost in string format.
    /// Converted mana cost is the total integer value of the mana cost.
    /// For example, a mana cost of 2 would have a converted mana cost of 2.
    /// A mana cost of 4UU would have a converted mana cost of 6.
    /// 
    /// NOTE - a copy of this method is also located in the CardInstance.ConvertedManaCost property.
    /// A strong consideration is that we should only grab converted mana costs from here.
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="manaCost"></param>
    /// <returns></returns>
    public int GetConvertedManaCost(CardGame cardGame, string manaCost)
    {
        //From Left To Right
        //Count the number of colors symbols (i.e. should be letters)
        //Then Count the number as the generic symbol

        //Mana Costs should be in Magic Format (i.e. 3U, 5BB) with the generic mana cost first.
        var manaChars = manaCost.ToCharArray();
        int convertedCost = 0;
        string currentNumber = ""; //should only be 1 currentNumber
        for (int i = 0; i < manaChars.Length; i++)
        {
            if (manaChars[i].IsNumeric())
            {
                currentNumber += manaChars[i].ToString();
            }
            else
            {
                if (currentNumber.Length > 0)
                {
                    convertedCost += Convert.ToInt32(currentNumber);
                    currentNumber = "";
                }
                convertedCost++; //if its not a numeric symbol than it should be a colored symbol and we just add 1.
            }
        }

        if (currentNumber.Length > 0)
        {
            convertedCost += Convert.ToInt32(currentNumber); 
        }
        return convertedCost;
    }
}