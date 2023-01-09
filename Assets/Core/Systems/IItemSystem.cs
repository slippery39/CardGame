using System.Collections.Generic;

public interface IItemSystem
{
    void PlayItem(Player player, CardInstance card, List<CardGameEntity> targets, ResolvingCardInstanceActionInfo resolvingCardInstance);
}

public class DefaultItemSystem : CardGameSystem, IItemSystem
{
    public DefaultItemSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void PlayItem(Player player, CardInstance card, List<CardGameEntity> targets, ResolvingCardInstanceActionInfo resolvingCardInstance)
    {
        cardGame.ZoneChangeSystem.MoveToZone(card, player.Items);

        foreach (var ab in card.GetAbilitiesAndComponents<IOnSummon>())
        {
            ab.OnSummoned(cardGame, card);
        }
    }
}