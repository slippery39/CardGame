internal interface IDestroySystem
{
    void DestroyUnit(CardGame cardGame, CardGameEntity source, CardInstance target);
}


public class DefaultDestroySystem : IDestroySystem
{
    public void DestroyUnit(CardGame cardGame, CardGameEntity source, CardInstance target)
    {
        var owner = cardGame.GetOwnerOfCard(target);
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, target, owner.DiscardPile);
    }
}                                                      