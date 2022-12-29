using System;
using System.Collections.Generic;
using System.Linq;

public interface IManaSystem
{

    //TODO - Simplify this system... we have too many methods in here.. Change all of our methods to just accept a string of the mana cost instead.
    void AddMana(Player player, string manaToAdd);
    void AddTemporaryMana(Player player, string manaToAdd);

    void AddTotalMana(Player player, string manaToAdd);
    void SpendMana(Player player, string cost);
    void ResetManaAndEssence(Player player);
    bool CanPlayCard(Player player, CardInstance card);

    bool CanPlayCard(Player player, CardInstance card, List<ICastModifier> modifiers);
    bool CanPlayManaCard(Player player, CardInstance card);

    void PlayManaCardFromEffect(Player player, CardInstance card, bool forceEmpty);
    void PlayManaCard(Player player, CardInstance card);

    void PlayManaCard(Player player, CardInstance card, bool forceEmpty);

    /// <summary>
    /// Checks whether a player can theoretically pay a mana cost. Mana Cost is passed in as a string format. 
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <param name="manaCost"></param>
    bool CanPayManaCost(Player player, string manaCost);

}

public class DefaultManaSystem : CardGameSystem, IManaSystem
{
    public DefaultManaSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
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
    private bool CanPayManaCost(Mana costToPay, Mana payingPool)
    {
        return payingPool.IsEnoughToPayCost(costToPay);
    }

    public bool CanPlayCard(Player player, CardInstance card)
    {
        if (card.ManaCost == "")
        {
            return false;
        }
        //TODO - Handle Non Integer Mana Costs.
        return CanPayManaCost(new Mana(card.ManaCost), player.ManaPool.CurrentMana);
    }

    public bool CanPlayCard(Player player, CardInstance card, List<ICastModifier> castModifiers)
    {
        if (castModifiers.IsNullOrEmpty())
        {
            return CanPlayCard(player, card);
        }

        if (card.ManaCost == "")
        {
            return false;
        }

        var mana = new Mana(card.ManaCost);

        foreach (var mod in castModifiers)
        {
            mana.AddFromString(mod.GetCost(card));
        }

        return CanPayManaCost(mana, player.ManaPool.CurrentMana);

    }

    public bool CanPlayManaCard(Player player, CardInstance card)
    {
        return player.ManaPlayedThisTurn < player.TotalManaThatCanBePlayedThisTurn;
    }

    //Adds total mana without adding it to the current mana
    public void AddTotalMana(Player player, string mana)
    {
        player.ManaPool.AddTotalMana(mana);
    }

    public void PlayManaCard(Player player, CardInstance card)
    {
        PlayManaCard(player, card, false);
    }

    private void HandleManaCard(Player player, CardInstance card, bool forceEmpty)
    {
        var manaCard = card.CurrentCardData as ManaCardData;

        if (!(forceEmpty) && (manaCard.ReadyImmediately == true || (manaCard.ReadyImmediately == false && manaCard.ReadyCondition?.IsReady(cardGame, player) == true)))
        {
            AddMana(player, manaCard.ManaAdded);
        }
        else
        {
            AddTotalMana(player, manaCard.ManaAdded);
        }

        //Trigger any SelfEnters play effects
        var selfEntersPlayEffects = card.GetAbilitiesAndComponents<TriggeredAbility>().Where(ta => ta.TriggerType == TriggerType.SelfEntersPlay);

        foreach (var ab in selfEntersPlayEffects)
        {
            cardGame.EffectsProcessor.ApplyEffects(player, card, ab.Effects, new List<CardGameEntity>());
        }

        //Trigger any mana enters play effects      
        cardGame.HandleTriggeredAbilities(player.GetCardsInPlay(), TriggerType.SelfManaPlayed);
        cardGame.ZoneChangeSystem.MoveToZone(card, player.Exile);
    }

    public void PlayManaCardFromEffect(Player player, CardInstance card, bool forceEmpty)
    {
        HandleManaCard(player, card, forceEmpty);
    }

    public void PlayManaCard(Player player, CardInstance card, bool forceEmpty)
    {
        player.ManaPlayedThisTurn++;
        HandleManaCard(player, card, forceEmpty);
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    public bool CanPayManaCost(Player player, string manaCost)
    {
        //Cards without mana costs cannot be played at all (i.e. Lotus Bloom and other suspend cards)
        if (manaCost == "")
        {
            return false;
        }
        return (player.ManaPool.CurrentMana.IsEnoughToPayCost(new Mana(manaCost))); ;
    }
}