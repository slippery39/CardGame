using System.Linq;

public class PumpPowerByNumberOfArtifactsEffect : Effect
{

    public int CountArtifacts(CardGame cardGame, Player player)
    {
        var thingsInPlay = cardGame.GetUnitsInPlay().Where(u => cardGame.GetOwnerOfCard(u) == player).ToList();
        thingsInPlay.AddRange(player.Items.Cards);
        return thingsInPlay.Where(thing => thing.Subtype.ToLower() == "artifact").Count();
    }

    public override string RulesText
    {
        get
        {
            return $"A unit gets +X/+0 until end of turn where X is the amount of artifacts you control.";
        }
    }
}


