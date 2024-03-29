﻿using System.Linq;

public interface IWinLoseSystem
{
    public void LoseGame(Player player);
    public void CheckLosers();

    public GameOverInfo GetGameOverInfo();
}


public class DefaultWinLoseSystem : CardGameSystem, IWinLoseSystem
{
    public DefaultWinLoseSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void LoseGame(Player player)
    {
        //In MTG winning and losing is checked as a state based effect.
        //This makes it so that other effects could possibly change the outcome of this.
        player.IsLoser = true;
    }

    public void CheckLosers()
    {
        if (cardGame.CurrentGameState == GameState.GameOver)
        {
            return;
        }

        cardGame.Players.ForEach(p =>
        {
            if (p.IsLoser)
            {
                var winner = cardGame.Players.Where(pl => pl.EntityId != p.EntityId).FirstOrDefault();
                cardGame.CurrentGameState = GameState.GameOver;
                cardGame.GameEventSystem.FireGameStateUpdatedEvent();
                cardGame.GameEventSystem.FireEvent(new GameEndEvent
                {
                    LoserId = p.EntityId,
                    LoserName = p.Name,
                    WinnerId = winner.EntityId,
                    WinnerName = winner.Name
                });
                cardGame.EventLogSystem.AddEvent($"Player {p.Name} has lost the game!");
                cardGame.OnGameOver.OnNext(cardGame);
            }
        });
    }

    public GameOverInfo GetGameOverInfo()
    {
        if (cardGame.Player1.IsLoser && cardGame.Player2.IsLoser)
        {
            return new GameOverInfo
            {
                IsDraw = true,
            };
        }
        else if (cardGame.Player1.IsLoser)
        {
            return new GameOverInfo
            {
                Winner = cardGame.Player2,
                Loser = cardGame.Player1,
            };
        }
        else
        {
            return new GameOverInfo
            {
                Winner = cardGame.Player1,
                Loser = cardGame.Player2,
            };
        }
    }
}


public class GameOverInfo
{
    public Player Winner { get; set; }
    public Player Loser { get; set; }
    public bool IsDraw { get; set; }
}