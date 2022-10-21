public interface IDestroySystem
{
    void DestroyUnit(CardGameEntity source, CardInstance target);
}


public class DefaultDestroySystem : CardGameSystem, IDestroySystem
{
    public DefaultDestroySystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void DestroyUnit(CardGameEntity source, CardInstance target)
    {
        if (target.Shields > 0)
        {
            target.Shields--;
            return;
        }
        var owner = cardGame.GetOwnerOfCard(target);
        cardGame.ZoneChangeSystem.MoveToZone(target, owner.DiscardPile);
    }
}