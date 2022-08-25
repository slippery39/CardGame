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
    public List<CardGameEntity> Targets { get; set; }
    public List<CardGameEntity> AdditionalChoices { get; set; }
    public CardInstance Spell { get; set; }

    public override void DoAction(CardGame cardGame)
    {
        int target = 0;
        if (Targets!=null && Targets.Any())
        {
            target = Targets.Select(t => t.EntityId).First();
        }
        cardGame.PlayCard(Player, Spell,target, AdditionalChoices);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        //TODO - Also need to check the targets and the Additional Choices to make sure they are valid. 
        return cardGame.CanPlayCard(Spell);
    }
}

public class ActivateAbilityAction : CardGameAction
{
    public CardInstance CardWithAbility { get; set; }
    public ActivatedAbility Ability { get; set; }
    public List<CardGameEntity> AdditionalCostChoices { get; set; }
    public List<CardGameEntity> Targets { get; set; }

    public override void DoAction(CardGame cardGame)
    {
        cardGame.ActivatedAbilitySystem.ActivateAbililty(Player, CardWithAbility, new ActivateAbilityInfo
        {
            Targets = Targets,
            Choices = AdditionalCostChoices
        });
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        //TODO - Check the additional costs selected and targets and other things.
        return cardGame.ActivatedAbilitySystem.CanActivateAbility(Player, CardWithAbility);
    }
}

public class FightAction : CardGameAction
{
    public int LaneIndex { get; set; }

    public override void DoAction(CardGame cardGame)
    {
        cardGame.BattleSystem.Battle(LaneIndex);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        return cardGame.BattleSystem.CanBattle(LaneIndex);
    }
}


/*
public class ResolveChoiceAction : CardGameAction
{
    public override void DoAction(CardGame cardGame)
    {
        cardGame.MakeChoice(choices);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        //all choices selected must exist in the GetValidChoices() method
        ChoiceInfoNeeded.
        return choices.cardGame.ChoiceInfoNeeded.GetValidChoices()
    }
}
*/


