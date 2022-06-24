public class DefaultHealingSystem : IHealingSystem
{
    private CardGame cardGame;

    public DefaultHealingSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void HealPlayer(Player playerToHeal,int amount)
    {
        playerToHeal.Health += amount;
    }
}