using System.Collections.Generic;

public class PactLoseGameEffect : Effect
{
    public string ManaToPay { get; set; }
    public override string RulesText => $"At the start of your next turn, pay {ManaToPay} or you lose the game";

    public PactLoseGameEffect(string manaToPay)
    {
        ManaToPay = manaToPay;
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        player.Modifications.Add(new PactLoseGameModification(ManaToPay));
    }
}

public class PactLoseGameModification : Modification, IOnTurnStart
{
    public string ManaToPay { get; set; }
    public PactLoseGameModification(string manaToPay)
    {
        ManaToPay = manaToPay;
        OneTurnOnly = false;
    }

    public void OnTurnStart(CardGame cardGame, Player player, CardInstance source)
    {
        if (!cardGame.ManaSystem.CanPayManaCost(player, ManaToPay))
        {
            cardGame.WinLoseSystem.LoseGame(player);
            return;
        }
    }
}

