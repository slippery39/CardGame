using System.Collections.Generic;
using System.Linq;

public class ThoughtseizeEffect : EffectWithChoice
{
    public override TargetType TargetType { get; set; } = TargetType.Opponent;
    public override string RulesText => "Look at your opponents hand. Choose 1 non mana card from it and discard it";

    public override string ChoiceMessage => "Choose a card to discard.";

    public override int NumberOfChoices { get; set; } = 1;

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return cardGame.InactivePlayer.Hand.Where(c => !c.IsOfType<ManaCardData>()).ToList();
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        foreach (var card in cardGame.InactivePlayer.Hand)
        {
            card.RevealedToAll = true;
        }
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        //Discard the choices
        choices.ForEach(choice =>
        {
            var card = choice as CardInstance;
            cardGame.DiscardSystem.Discard(cardGame.InactivePlayer, card);
        });
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        //This should apply the actual effect after the cards are chosen....
        //Basically choice setup should be called before Apply, then once the cards are chosen we call apply with the chosen cards.
        return;
    }
}

