using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class CardGameAction
{
    public Player Player { get; set; }
    public abstract void DoAction(CardGame cardGame);
    public abstract bool IsValidAction(CardGame cardGame);
}

public class PlayUnitAction : CardGameAction
{
   public Lane Lane { get; set; }
   public CardInstance Card { get; set; }

   public override bool IsValidAction(CardGame cardGame)
   {
        return Lane!=null && cardGame.CanPlayCard(Card) && Lane.IsEmpty();
   }

    public override void DoAction(CardGame cardGame)
    {
        cardGame.PlayCard(Player, Card, Lane.EntityId, null);
    }
}

public class PlayManaAction : CardGameAction
{
    public CardInstance Card { get; set; }
    //Etc..
    public override void DoAction(CardGame cardGame)
    {
        cardGame.PlayCard(Player, Card, 0 , null);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        return cardGame.CanPlayCard(Card);
    }
}

public class PlaySpellAction : CardGameAction
{
    //Cost Choices etc..
    //Targets etc..
    public override void DoAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }
}

public class ActivateAbilityAction : CardGameAction
{
    public CardInstance sourceCard;
    public ActivatedAbility ability;
    public List<CardGameEntity> additionalCostChoices;

    public override void DoAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }
}

public class FightAction : CardGameAction
{
    public CardInstance sourceCard;

    public override void DoAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        throw new NotImplementedException();
    }
}


