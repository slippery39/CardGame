using System;

public interface IManaSystem
{
    void AddMana(CardGame cardGame,Player player, int amount);
    void SpendMana(CardGame cardGame, Player player, int amount);
    bool CanPlayCard(CardGame cardGame, Player player, CardInstance card);
}

public class DefaultManaSystem : IManaSystem
{
    public void AddMana(CardGame cardGame,Player player,int amount)
    {
        player.Mana += amount;
    }

    public void SpendMana(CardGame cardGame, Player player, int amount)
    {
        player.Mana -= amount;
    }

    public bool CanPlayCard(CardGame cardGame,Player player, CardInstance card)
    {
        return player.Mana >= Convert.ToInt32(card.ManaCost);
    }
}