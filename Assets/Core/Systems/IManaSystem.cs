using System;

public interface IManaSystem
{
    void AddMana(CardGame cardGame,Player player, int amount);
    void AddTemporaryMana(CardGame cardGame, Player player, int amount);
    void SpendMana(CardGame cardGame, Player player, int amount);
    void ResetMana(CardGame cardGame, Player player);
    bool CanPlayCard(CardGame cardGame, Player player, CardInstance card);
    bool CanPlayManaCard(CardGame cardGame, Player player, CardInstance card);
    void PlayManaCard(CardGame cardGame, Player player, CardInstance card);

}

public class DefaultManaSystem : IManaSystem
{
    //Adds mana to the mana pool without effecting the total amount.
    public void AddTemporaryMana(CardGame cardGame,Player player, int amount)
    {
        player.Mana += amount;
    }
    public void AddMana(CardGame cardGame,Player player,int amount)
    {
        player.Mana += amount;
        player.TotalMana += amount;
    }

    public void SpendMana(CardGame cardGame, Player player, int amount)
    {
        player.Mana -= amount;
    }

    public void ResetMana(CardGame cardGame, Player player)
    {
        player.Mana = player.TotalMana; 
    }

    public bool CanPlayCard(CardGame cardGame,Player player, CardInstance card)
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
        AddMana(cardGame, player, Convert.ToInt32(manaCard.ManaAdded));
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.DiscardPile);        
    }
}