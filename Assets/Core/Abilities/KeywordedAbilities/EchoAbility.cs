public class EchoAbility: CardAbility, IOnSummon, IOnTurnStart
{
    public string EchoCost { get; set; }
    public override string RulesText => $"Echo : {EchoCost}";

    private bool doEcho = true;

    public void OnSummoned(CardGame cardGame, CardInstance source)
    {
        doEcho = true; // should always do the the echo cost the turn after it is summoned.
    }

    public void OnTurnStart(CardGame cardGame, Player player, CardInstance source)
    {
        cardGame.Log("Echo Ability is recognized!");
        if (!doEcho)
        {
            return;
        }

        /**
         * Echo Rules: 
         * 
         * Try to pay the echo cost at the start of the turn.
         * If it cannot be payed, sacrifice the card instead.
         **/
        if (cardGame.GetZoneOfCard(source).ZoneType == ZoneType.InPlay)
        {
            if (cardGame.ManaSystem.CanPayManaCost(player, EchoCost))
            {
                cardGame.Log("Echo Ability has triggered!");
                doEcho = false;
                cardGame.ManaSystem.SpendMana(player, EchoCost);
            }
            else
            {
                cardGame.Log("Uktabi Drake was sacrificed for not paying the echo cost!");
                cardGame.SacrificeSystem.Sacrifice(player, source);
            }
        }
     
    }
}


