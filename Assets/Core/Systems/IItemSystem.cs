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

        card.GetAbilitiesAndComponents<IOnSummon>().ForEach(ab =>
        {
            ab.OnSummoned(cardGame, card); 
        });
    }
}