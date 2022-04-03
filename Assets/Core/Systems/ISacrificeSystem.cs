public interface ISacrificeSystem
{    void SacrificeUnit(CardGame cardGame, Player owner, CardInstance unit);
}


public class DefaultSacrificeSystem : ISacrificeSystem
{
    public void SacrificeUnit(CardGame cardGame, Player owner, CardInstance unit)
    {
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, unit, owner.DiscardPile);
        cardGame.Log($@"{unit.Name} was sacrificed!");
    }
}