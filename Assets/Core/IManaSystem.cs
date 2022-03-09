public interface IManaSystem
{
    void AddMana(CardGame cardGame,Player player, int amount);
}

public class DefaultManaSystem : IManaSystem
{
    public void AddMana(CardGame cardGame,Player player,int amount)
    {
        player.Mana += amount;
    }
}