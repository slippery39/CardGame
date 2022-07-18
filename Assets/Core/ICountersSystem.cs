public interface ICountersSystem
{
    public void AddPlusOnePlusOneCounter(CardInstance card, int amount);
}


public class DefaultCountersSystem : ICountersSystem
{
    private CardGame cardGame;
    public DefaultCountersSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void AddPlusOnePlusOneCounter(CardInstance card, int amount)
    {

        for (var i = 0; i < amount; i++)
        {
            card.Counters.Add(new PlusOnePlusOneCounter());
        }
    }
}


