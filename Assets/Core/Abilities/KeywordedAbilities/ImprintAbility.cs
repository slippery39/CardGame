using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Make it for Chrome Mox right now, but we might need to expand it later..
//Would probably be an ImprintAbility<T> where T is the Effect?
//Maybe it would be a double generic <ActivatedAbility,Effect>
//<StaticAbility,Effect>
public class ImprintAbility : CardAbility, IOnSummon, IOnResolveChoiceMade//generic interface for ContainsAbility
{
    /// <summary>
    /// The card that the imprint ability is on.
    /// </summary>
    CardInstance SourceCard { get; set; }
    CardInstance Imprint { get; set; }
    AddTempManaEffect ImprintEffect { get; set; }
    ActivatedAbility ActivatedAbility { get; set; }

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
        SourceCard = source;

        if (owner.Hand.Cards.Count > 0)
        {
            //Technically Imprint Exiles the card.
            cardGame.PromptPlayerForChoice(owner, _imprintDiscardEffect);
        }

        //Need a callback to when the 
    }
    public void OnResolveChoiceMade(CardGame cardGame, CardInstance choice, Effect effectWithChoice)
    {
        //This should only happen once.
        if (effectWithChoice == _imprintDiscardEffect)
        {
            cardGame.Log($"Imprint has been resolved : {choice.Name}");
            Imprint = choice;
            ImprintEffect = new AddTempManaEffect();
            ImprintEffect.ManaToAdd = "1" + choice.Colors.ToManaString();

            ActivatedAbility = new ActivatedAbility
            {
                ManaCost = "0",
                OncePerTurn = true,
                Effects = new List<Effect>
                {
                    ImprintEffect
                }
            };

            SourceCard.Abilities.Add(ActivatedAbility);
        }
    }
}


public interface IOnResolveChoiceMade
{
    void OnResolveChoiceMade(CardGame cardGame, CardInstance choice, Effect effectWithChoice);
}

