using System.Collections.Generic;
using System.Linq;

public class ChooseCardFromDeckIntoHandEffect : EffectWithChoice
{
    public override string RulesText => "Needs Updated Rules Text due to TargetInfo updates"; //"Put a #cardType# from your deck into your hand".Replace("#cardType#", Filter.RulesTextString(false).ToLower());
    public override string ChoiceMessage { get => "Choose a card to put into your hand"; }
    public override int NumberOfChoices { get; set; } = 1;

    public ChooseCardFromDeckIntoHandEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return CardFilter.ApplyFilter(player.Deck.Cards, Filter);
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.CardDrawSystem.PutIntoHand(player, choices.Cast<CardInstance>().First());
        cardGame.CardDrawSystem.Shuffle(player);
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
    }
}

