using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Creating this for Mana Leak, but should be able to expand it for any Instant type effects we want in our game.
public class RespondToCastAbility : CardAbility
{
    public override string RulesText => "Respond - Opponent casts a spell and has 2 colorless mana or less : Cancel the spell";

    public void BeforeSpellResolve(CardGame cardGame, ResolvingCardInstanceActionInfo info)
    {

        //Check if the spell is still on the stack.. (to handle the edge case that it has been cancelled already)

        var spellOwner = cardGame.GetOwnerOfCard(info.CardInstance);

        //We need some way to know who the owner of this ability is.
        var cardWithAbility = cardGame.GetCardThatHasResponseAbility(this);
        var ownerOfCardWithAbility = cardGame.GetOwnerOfCard(cardWithAbility);

        var spellCastIsByOpponent = cardGame.GetOwnerOfCard(info.CardInstance) != ownerOfCardWithAbility;
        var enoughManaToCast = cardGame.ManaSystem.CanPayManaCost(ownerOfCardWithAbility, cardWithAbility.ManaCost);
        var responseCondition = spellOwner.ManaPool.CurrentColorlessMana < 3;

        if (spellCastIsByOpponent && enoughManaToCast && responseCondition)
        {
            //Cancel the spell.. 
            cardGame.ResolvingSystem.Cancel(info);
            //This card should get moved to the graveyard, (not treating it as a cast for now)
            cardGame.ZoneChangeSystem.MoveToZone(cardWithAbility, ownerOfCardWithAbility.DiscardPile);
        }
    }
}

public class RespondToOpponentEndOfTurnAbility : CardAbility
{
    public override string RulesText => "Respond - Opponents End Of Turn: ";

    public void AtOpponentEndOfTurn(CardGame cardGame, CardInstance sourceCard)
    {
        //This does not work because it checks if the person casting the spell is the ActivePlayer...
        //..we need a new method that does not check that.
        if (!cardGame.CanPlayCard(sourceCard, false))
        {
            return;
        }

        var owner = cardGame.GetOwnerOfCard(sourceCard);
        //Check if the spell is still on the stack.. (to handle the edge case that it has been cancelled already)
        cardGame.Log($"Summoning Trap has been played by {owner.Name}");
        cardGame.PlayCard(owner, sourceCard, 0, new List<CardGameEntity> { }, false);
    }
}


