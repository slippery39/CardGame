public interface ITurnSystem
{
    int TurnId { get; set; }
    void StartTurn(CardGame cardGame);
    void EndTurn(CardGame cardGame);
}

public enum TurnPhase
{
    StartOfTurn,
    Main,
    Battle,
    EndOfTurn
}

public class DefaultTurnSystem : ITurnSystem
{
    public int TurnId { get; set;}

    public DefaultTurnSystem()
    {
        TurnId = 1;
    }
    public void StartTurn(CardGame cardGame)
    {
        
        //Reset any spent mana
        cardGame.ManaSystem.ResetMana(cardGame, cardGame.ActivePlayer);
        //Active Player Gains A Mana
        cardGame.ManaSystem.AddMana(cardGame, cardGame.ActivePlayer, 1);
        //Reset any variables that track per turn       

        //Active Player draws a card
        cardGame.CardDrawSystem.DrawCard(cardGame, cardGame.ActivePlayer);
    }

    public void EndTurn(CardGame cardGame)
    {
        //Execute our battles:
        cardGame.BattleSystem.ExecuteBattles(cardGame);
        //Change the active player
        _ = cardGame.ActivePlayerId == 1 ? cardGame.ActivePlayerId = 2 : cardGame.ActivePlayerId = 1;
        TurnId++;
    }
}