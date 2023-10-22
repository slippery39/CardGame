using System.Collections.Generic;
using System.Linq;

public class GrabFromTopOfDeckEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var str = $"draw up to {Amount} #cardType# from the top {CardsToLookAt} cards of your deck";

            if (CardsToLookAt == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} card of your deck";
            }
            else if (CardsToLookAt == Amount)
            {
                str = $"draw all #cardType# from the top {CardsToLookAt} cards of your deck";
            }
            else if (Amount == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} cards of your deck";
            }

            var needsPlural = Amount > 1;

            if (Filter != null)
            {
                return str.Replace("#cardType#", Filter.RulesTextString(needsPlural).ToLower());
            }
            else
            {
                var cardStr = needsPlural ? "cards" : "card";
                return str.Replace("#cardType#", cardStr);
            }
        }
    }
    public int CardsToLookAt { get; set; }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.None;
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        cardGame.CardDrawSystem.GrabFromTopOfDeck(player, Filter, CardsToLookAt, Amount);
    }
}

public class ChooseFromTopOfDeckEffect : EffectWithChoice
{
    public override string RulesText
    {
        get
        {
            var str = $"draw up to {Amount} #cardType# from the top {CardsToLookAt} cards of your deck";

            if (CardsToLookAt == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} card of your deck";
            }
            else if (CardsToLookAt == Amount)
            {
                str = $"draw all #cardType# from the top {CardsToLookAt} cards of your deck";
            }
            else if (Amount == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} cards of your deck";
            }

            var needsPlural = Amount > 1;

            if (Filter != null)
            {
                return str.Replace("#cardType#", Filter.RulesTextString(needsPlural).ToLower());
            }
            else
            {
                var cardStr = needsPlural ? "cards" : "card";
                return str.Replace("#cardType#", cardStr);
            }
        }
    }
    public int CardsToLookAt { get; set; }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public override string ChoiceMessage => RulesText;

    public override int NumberOfChoices { get => Amount; set { Amount = value; } }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        return;
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.Cards.TakeLast(3).ToList();
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        var cardsSeen = GetValidChoices(cardGame, player);
        foreach (var card in cardsSeen)
        {
            if (!choices.Contains(card))
            {
                player.Deck.MoveToBottom(card);
            }
        }

        cardGame.CardDrawSystem.PutIntoHand(player, choices[0] as CardInstance);
    }
}




