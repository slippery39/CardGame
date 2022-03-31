public interface ITurnSystem
{
    int TurnId { get; set; }
    int TurnIdPerPlayer { get; set; }
    Player ActivePlayer { get; set; }
    void StartTurn(CardGame cardGame);
    void EndTurn(CardGame cardGame);
}

public enum TurnPhase
{
    StartOfTurn,
    Main,
    Battle,
    EndOfTurkjn
}

public class DefaultTurnSystem
{
    public void StartTurn(CardGame cardGame)
    {
        //Reset any variables that track per turn

        //Active Player draws a card

        //Trigger all at Start of Turn Effects
    }

    public void EndTurn(CardGame cardGame)
    {
        //Trigger any at end of turn effects
    }
}