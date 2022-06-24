public interface ISacrificeSystem
{    void SacrificeUnit(Player owner, CardInstance unit);
}


public class DefaultSacrificeSystem : ISacrificeSystem
{
    private CardGame cardGame;

    public DefaultSacrificeSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void SacrificeUnit(Player owner, CardInstance unit)
    {
        cardGame.ZoneChangeSystem.MoveToZone(unit, owner.DiscardPile);
        cardGame.Log($@"{unit.Name} was sacrificed!");
    }
}