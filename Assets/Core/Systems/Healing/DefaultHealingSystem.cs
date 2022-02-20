public class DefaultHealingSystem : IHealingSystem
{
    public void HealPlayer(CardGame cardGame, Player playerToHeal,int amount)
    {
        playerToHeal.Health += amount;
    }
}