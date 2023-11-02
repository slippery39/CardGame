using System.Collections.Generic;

public class LookAtOpponentHandEffect : Effect
{    
    public override string RulesText => "Look at your opponents hand";

    public LookAtOpponentHandEffect()
    {
        TargetInfo = TargetInfo.Opponent();
    }
    
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var opponent = entitiesToApply[0] as Player;
        if (opponent == null)
        {
            cardGame.Log("Entity to apply a Look At Opponent Hand Effect should be a player");
            return;
        }

        foreach (var card in opponent.Hand)
        {
            card.RevealedToAll = true;
        }
    }
}

