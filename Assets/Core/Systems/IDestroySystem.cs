public interface IDestroySystem
{
    void DestroyUnit(CardGameEntity source, CardInstance target);
}


public class DefaultDestroySystem : IDestroySystem
{
    private CardGame cardGame;

    public DefaultDestroySystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void DestroyUnit(CardGameEntity source, CardInstance target)
    {
        var owner = cardGame.GetOwnerOfCard(target);
        cardGame.ZoneChangeSystem.MoveToZone(target, owner.DiscardPile);
    }
}                                                      