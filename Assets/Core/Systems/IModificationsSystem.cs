public interface IModificationsSystem
{
    public void AddModification(CardInstance cardToGetModification, Modification mod);
}

public class DefaultModificationSystem : IModificationsSystem
{
    private CardGame cardGame;
    public DefaultModificationSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    //TODO - Add source and if it should be static or not.
    public void AddModification(CardInstance cardToGetModification, Modification mod)
    {
        cardToGetModification.AddModification(mod);
    }
}