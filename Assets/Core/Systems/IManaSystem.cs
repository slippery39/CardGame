using System;
using System.Collections.Generic;
using System.Linq;

public interface IManaSystem
{

    //TODO - Simplify this system... we have too many methods in here.. Change all of our methods to just accept a string of the mana cost instead.
    void AddMana(Player player, string manaToAdd);
    void AddTemporaryMana(Player player, string manaToAdd);
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
    public void AddTemporaryColorAndColorless(Player player, ManaType manaType, int amount)
    {
        player.ManaPool.AddTemporaryColorAndColorless(manaType, amount);
    }

    public void AddMana(Player player, int amount)
    {
        player.ManaPool.AddColorlessMana(amount);
    }

    public void AddMana(Player player, string mana)
    {
        player.ManaPool.AddMana(mana);
    }

    public void AddTemporaryMana(Player player, string mana)
    {
        player.ManaPool.AddTemporary(mana);
    }


    public void SpendMana(Player player, string cost)
    {
        var manaAsObj = new Mana(cost);
        if (manaAsObj.ColorlessMana > 0)
        {
            player.ManaPool.SpendColorlessMana(manaAsObj.ColorlessMana);
        }

        if (manaAsObj.TotalSumOfColoredMana > 0)
        {
            foreach (var color in manaAsObj.ColoredMana.Keys)
            {
                player.ManaPool.SpendColoredMana(color, manaAsObj.ColoredMana[color]);
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
        AddMana(player, manaCard.ManaAdded);

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

    public void AddTemporaryColor(Player player, ManaType essenceType, int amount)
    {
        player.ManaPool.AddTemporaryColor(essenceType, amount);
    }
}