using System.Linq;

public interface IPlayerModificationSystem
{
    public void GiveModification(Player player, Modification mod);
    public void RemoveOneTurnModifications(Player player);
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

    public void RemoveOneTurnModifications(Player player)
    {
        player.Modifications = player.Modifications.Where(mod => mod.OneTurnOnly == false).ToList();
    }
}