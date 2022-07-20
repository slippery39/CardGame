public interface IPlayerModificationSystem
{
    public void GiveModification(Player player, Modification mod);
}


public class PlayerModificationSystem : IPlayerModificationSystem
{
    private CardGame cardGame;

    public PlayerModificationSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void GiveModification(Player player, Modification mod)
    {
        player.Modifications.Add(mod);
    }
}