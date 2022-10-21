using System.Linq;

public interface ICardDrawSystem
{
    CardInstance DrawCard(Player player);
    void DrawOpeningHand(Player player);
    void GrabRandomCardFromDeck(Player player, CardFilter filter);
    void GrabFromTopOfDeck(Player player, CardFilter filter, int amountToLookAt, int amountToGrab);

    void PutIntoHand(Player player, CardInstance card);
    void Shuffle(Player player);
}


public class DefaultCardDrawSystem : CardGameSystem, ICardDrawSystem
{
    public DefaultCardDrawSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    //TODO - What happens if we have too many cards in our hand?
    //TODO - What happens if we draw a card with no cards left in our deck?
    public CardInstance DrawCard(Player player)
    {
        if (player.Deck.Cards.Count > 0)
        {
            var card = player.Deck.GetTopCard();
            cardGame.ZoneChangeSystem.MoveToZone(card, player.Hand);
            return card;
        }
        return null;
    }

    public void DrawOpeningHand(Player player)
    {
        int cardsToDraw = 4;
        int manaToDraw = 3;
        //Should grab 3 mana cards automatically
        var manaCards = player.Deck.Cards.Where(card => card.CurrentCardData is ManaCardData).Randomize();
        var nonManaCards = player.Deck.Cards.Where(card => !(card.CurrentCardData is ManaCardData)).Randomize();

        if (manaCards.Count() < manaToDraw)
        {
            throw new System.Exception("There is not enough mana cards to draw the opening hand with!");
        }

        for (int i = 0; i < manaToDraw; i++)
        {
            cardGame.ZoneChangeSystem.MoveToZone(manaCards.ToList()[i], player.Hand);
        }


        for (int i = 0; i < cardsToDraw; i++)
        {
            cardGame.ZoneChangeSystem.MoveToZone(nonManaCards.ToList()[i], player.Hand);
        }
    }

    public void GrabRandomCardFromDeck(Player player, CardFilter filter)
    {
        var validCardsToGet = CardFilter.ApplyFilter(player.Deck.Cards.ToList(), filter);

        if (!validCardsToGet.Any())
        {
            cardGame.Log("No Valid cards to grab");
            return;
        }

        cardGame.Log("Grabbed random card from deck");
        cardGame.ZoneChangeSystem.MoveToZone(validCardsToGet.Randomize().ToList()[0], player.Hand);
    }

    public void GrabFromTopOfDeck(Player player, CardFilter filter, int amountToLookAt, int amountToGrab)
    {
        var copyOfDeck = player.Deck.Cards.ToList();
        copyOfDeck.Reverse();

        var cardsToLookAt = copyOfDeck.Take(amountToLookAt).ToList();

        var validCardsToGet = CardFilter.ApplyFilter(cardsToLookAt, filter).Randomize().ToList();

        for (int i = 0; i < amountToGrab; i++)
        {
            if (validCardsToGet.Count() > i)
            {
                cardGame.ZoneChangeSystem.MoveToZone(validCardsToGet.ToList()[i], player.Hand);
                validCardsToGet.Remove(validCardsToGet.ToList()[i]);
            }
        }

        //Put the remaining cards on the bottom

        foreach (var card in cardsToLookAt)
        {
            //This could be a method.
            player.Deck.Cards.Remove(card);
            player.Deck.Cards.Insert(0, card);
        }
    }

    public void Shuffle(Player player)
    {
        player.Deck.Shuffle();
        player.Deck.Cards.ForEach(c =>
        {
            c.RevealedToOwner = false;
        });
    }

    public void PutIntoHand(Player player, CardInstance card)
    {
        cardGame.ZoneChangeSystem.MoveToZone(card, player.Hand);
    }
}
