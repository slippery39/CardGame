using System.Collections.Generic;
using System.Linq;

public class RampantGrowthChoiceEffect : EffectWithChoice
{
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;
    public override string RulesText => "Put a mana from your deck into play";

    public override string ChoiceMessage { get => "Choose a mana to card to put into play from your deck"; }
    public override int NumberOfChoices { get; set; } = 1;

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.Cards.Where(c => c.IsOfType<ManaCardData>()).ToList();
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.ManaSystem.PlayManaCardFromEffect(player, choices.Cast<CardInstance>().First(), true);
        cardGame.CardDrawSystem.Shuffle(player);
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        
    }
}

