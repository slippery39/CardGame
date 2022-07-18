public interface ISacrificeSystem
{
    void Sacrifice(Player owner, CardInstance unit);
}


public class DefaultSacrificeSystem : ISacrificeSystem
{
    private CardGame cardGame;

    public DefaultSacrificeSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void Sacrifice(Player owner, CardInstance card)
    {
        cardGame.ZoneChangeSystem.MoveToZone(card, owner.DiscardPile);
        cardGame.Log($@"{card.Name} was sacrificed!");
    }
}