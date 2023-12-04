using System.Collections.Generic;
using System.Linq;

//Make it for Chrome Mox right now, but we might need to expand it later..
//Would probably be an ImprintAbility<T> where T is the Effect?
//Maybe it would be a double generic <ActivatedAbility,Effect>
//<StaticAbility,Effect>
public class ImprintAbility : CardAbility, IOnSummon, IOnResolveChoiceMade//generic interface for ContainsAbility
{
    public int SourceCardId { get; set; }
    AddTempManaEffect ImprintEffect { get; set; }
    bool hasImprinted { get; set; } = false;

    //Temporary for testing;
    private List<CardInstance> sourceCards = new List<CardInstance>();

    private DiscardCardEffect _imprintDiscardEffect = new DiscardCardEffect
    {
        Amount = 1
    };
    public override string RulesText => "Imprint - Add mana of the imprinted card";
    //Discard a card as it comes into play.
    //Remember this discard  
    public void OnSummoned(CardGame cardGame, CardInstance source)
    {
        var owner = cardGame.GetOwnerOfCard(source);
        SourceCardId = source.EntityId;

        if (owner.Hand.Any())
        {
            //Technically Imprint Exiles the card.
            cardGame.PromptPlayerForChoice(owner, _imprintDiscardEffect, source);
        }
        else
        {
            //If they don't have a hand do an imprint of nothing.
            hasImprinted = true;
        }
    }
    public void OnResolveChoiceMade(CardGame cardGame, CardInstance choice, IEffectWithChoice effectWithChoice, CardInstance sourceCard)
    {
        //ISSUE HERE -> after cloning the ChoiceInfoNeededSource and the SourceCard may not be the same.
        //The source card needs to be grabbed from the game itself, not from this object.
        //This should only happen once.
        if (!hasImprinted && cardGame.ChoiceInfoNeededSource.EntityId == SourceCardId)
        {
            cardGame.Log($"Imprint has been resolved : {choice.Name}");
            ImprintEffect = new AddTempManaEffect();
            ImprintEffect.ManaToAdd = "1" + choice.Colors.ToManaString();

            var etbAbility = new TriggeredAbility(TriggerType.SelfEntersPlay, ImprintEffect);
            var startTurnAbility = new TriggeredAbility(TriggerType.AtTurnStart, ImprintEffect);
         
            sourceCard.Abilities.Add(etbAbility);
            sourceCard.Abilities.Add(startTurnAbility);

            sourceCards.Add(sourceCard);
            hasImprinted = true;
        }
    }

    public override CardAbility Clone()
    {
        var clone = base.Clone() as ImprintAbility;
        clone.hasImprinted = hasImprinted;
        return clone as CardAbility;
    }
}


public interface IOnResolveChoiceMade
{
    void OnResolveChoiceMade(CardGame cardGame, CardInstance choice, IEffectWithChoice effectWithChoice, CardInstance sourceCard);
}

