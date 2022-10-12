using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO - need different classifications of actions,
//i.e. PlayCardAction for all the actions that have us playing cards from different zones.
public abstract class CardGameAction
{
    public CardInstance SourceCard { get; set; }
    public CardInstance CardToPlay { get; set; }
    public Player Player { get; set; }
    public List<ICastModifier> CastModifiers { get; set; } = new List<ICastModifier>();
    private bool HasModifiers => CastModifiers != null && CastModifiers.Count > 0;
    public virtual List<CardGameEntity> Targets { get; set; }
    public List<CardGameEntity> AdditionalChoices { get; set; }
    public abstract void DoAction(CardGame cardGame);
    public abstract bool IsValidAction(CardGame cardGame);


    //TODO - Create an ActivatedAbilityAction
    public static CardGameAction CreateAction(CardInstance card, ActivatedAbility ability)
    {
        return new ActivateAbilityAction
        {
            SourceCard = card,
            CardWithAbility = card,
            Player = card.GetOwner(),
            Ability = ability
        };
    }

    public virtual string ToUIString()
    {
        //TODO - Create rules texts here.
        //If casting a regular card, remove all "casting" activated abilties from it and any cast modifiers.
        //If casting with cast modifier, include all regular text + the cast modifier text
        //If casting as activated ability, only include the activated ability text.
        if (SourceCard != null)
        {
            return SourceCard.RulesText;
        }
        else
        {
            return "[ACTION TEXT NEEDED]";
        }
    }

    public static CardGameAction CreatePlayCardAction(CardInstance cardToPlay, ICastModifier modifier = null)
    {
        //Check the type of the card to play.
        //We need to make the appropriate action type based on the type of the card
        if (cardToPlay.IsOfType<ManaCardData>())
        {
            return new PlayManaAction
            {
                Player = cardToPlay.GetOwner(),
                SourceCard = cardToPlay,
                CardToPlay = cardToPlay
            };
        }
        else if (cardToPlay.IsOfType<UnitCardData>())
        {
            return new PlayUnitAction
            {
                Player = cardToPlay.GetOwner(),
                SourceCard = cardToPlay,
                CardToPlay = cardToPlay
            };
        }
        else if (cardToPlay.IsOfType<SpellCardData>())
        {
            var castModifiers = new List<ICastModifier>() { modifier };
            if (modifier == null){
                castModifiers = new List<ICastModifier>();
            }
            return new PlaySpellAction
            {
                Player = cardToPlay.GetOwner(),
                SourceCard = cardToPlay,
                CardToPlay = cardToPlay,
                CastModifiers = castModifiers
            };
        }
        else if (cardToPlay.IsOfType<ItemCardData>())
        {
            return new PlaySpellAction
            {
                Player = cardToPlay.GetOwner(),
                SourceCard = cardToPlay,
                CardToPlay = cardToPlay
            };
        }

        throw new Exception($"Card type not properly programmed in for {cardToPlay.Name}");
    }
}

public class PlayUnitAction : CardGameAction
{
    public Lane Lane { get; set; }
    public override List<CardGameEntity> Targets { get { return new List<CardGameEntity>{ Lane }; } set{ } }
    public override bool IsValidAction(CardGame cardGame)
    {
        return Lane != null && cardGame.CanPlayCard(CardToPlay) && Lane.IsEmpty();
    }

    public override void DoAction(CardGame cardGame)
    {
        cardGame.PlayCard(Player, this); //before, i think we need to pass in the Lane.EntityId somehow --(Player, CardToPlay, Lane.EntityId, null);
    }

    public override string ToUIString()
    {
        return String.Join("\r\n", SourceCard.Abilities.Where(ab =>
         {
             var actAb = ab as ActivatedAbility;
             if (actAb != null)
             {
                 return actAb.ActivationZone == ZoneType.InPlay;
             }
             return true;
         })
        .Select(ab => ab.RulesText)
        );

    }
}

public class PlayManaAction : CardGameAction
{
    public override void DoAction(CardGame cardGame)
    {
        cardGame.PlayCard(Player,this);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        return cardGame.CanPlayCard(CardToPlay);
    }

    public override string ToUIString()
    {
        return SourceCard.RulesText;
    }
}

public class PlaySpellAction : CardGameAction
{
    public override string ToUIString()
    {
        return String.Join("\r\n",CardToPlay.Effects.Select(m => m.RulesText));   
    }
    public override void DoAction(CardGame cardGame)
    {
        int target = 0;
        if (Targets != null && Targets.Any())
        {
            target = Targets.Select(t => t.EntityId).First();
        }


       cardGame.PlayCard(Player, this);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
       return cardGame.CanPlayCard(CardToPlay,true,CastModifiers);
    }
}

public class ActivateAbilityAction : CardGameAction
{
    public CardInstance CardWithAbility { get; set; }
    public ActivatedAbility Ability { get; set; }
    public List<CardGameEntity> AdditionalCostChoices { get; set; }
    public override void DoAction(CardGame cardGame)
    {
        cardGame.ActivatedAbilitySystem.ActivateAbililty(Player, CardWithAbility, new ActivateAbilityInfo
        {
            Targets = Targets,
            Choices = AdditionalCostChoices
        });
    }

    public override string ToUIString()
    {
        return Ability.RulesText;
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


public class ResolveChoiceAction : CardGameAction
{
    public List<CardInstance> Choices { get; set; }
    public override void DoAction(CardGame cardGame)
    {
        cardGame.MakeChoice(Choices);
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        //All choices must exist in the GetValidChoices method.
        //TODO - Must also have selected the correct amount of choices.
        return Choices.Except(cardGame.ChoiceInfoNeeded.GetValidChoices(cardGame, Player)).Count() == 0;
    }
}

public class NextTurnAction : CardGameAction
{
    public override void DoAction(CardGame cardGame)
    {
        cardGame.NextTurn();
    }

    public override bool IsValidAction(CardGame cardGame)
    {
        //We should not be able to do next turn if we are waiting for a choice to be made, only an action.
        return cardGame.CurrentGameState == GameState.WaitingForAction;
    }
}



