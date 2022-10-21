public class DefaultHealingSystem : CardGameSystem, IHealingSystem
{
    public DefaultHealingSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void HealPlayer(Player playerToHeal,int amount)
    {
        playerToHeal.Health += amount;
    }
}