public interface ICountersSystem
{
    public void AddPlusOnePlusOneCounter(CardInstance card, int amount);
}


public class DefaultCountersSystem : CardGameSystem, ICountersSystem
{
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


