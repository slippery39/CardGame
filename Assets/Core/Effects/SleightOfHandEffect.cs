using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEffectWithChoice
{
    List<CardInstance> GetValidChoices(CardGame cardGame, Player player);
    void ChoiceSetup(CardGame cardGame, Player player, CardInstance source);
    void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices);   

    string ChoiceMessage { get; }
}

public interface IMultiChoiceEffect : IEffectWithChoice
{
    void MakeChoice(CardGame cardGame, Player player, CardGameEntity choice);
    int NumberOfChoices { get; set; }

    List<CardInstance> Choices { get; }
}

public class TellingTimeEffect : Effect, IMultiChoiceEffect
{

    private List<CardInstance> _chosenCards;
    public string ChoiceMessage
    {
        get
        {
            if (_chosenCards.Count() == 0)
            {
                return "Choose a card to put in your hand.";
            }
            if (_chosenCards.Count() == 1)
            {
                return "Choose a card to put on top of your deck";
            }
            if (_chosenCards.Count() == 2)
            {
                return "Choose a card to put on the bottom of your deck";
            }
            return "should never see this message";
        }
    }

    public int NumberOfChoices { get; set; } = 3;
    public List<CardInstance> Choices => _chosenCards;

    public override string RulesText => "Look at the top 3 cards of your deck. Put one into your hand, one on top of your deck, and the other on the bottom of your deck.";

    public List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.Cards.TakeLast(3).Where(c => !_chosenCards.Contains(c)).ToList();
    }
    public void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        _chosenCards = new List<CardInstance>();
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public void MakeChoice(CardGame cardGame,Player player,CardGameEntity choice)
    {
        if (_chosenCards.Contains(choice))
        {
            return;
        }
        else
        {
            _chosenCards.Add(choice as CardInstance);
        }
    }

    public void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        //First choice gets put into your hand,
        //Second choice gets put on the top of your deck.
        //Last choice gets put on the bottom of the deck.

        var cardToPutIntoHand = _chosenCards[0];
        var cardToPutOnTop = _chosenCards[1];
        var cardToPutOnBottom = _chosenCards[2];

        cardGame.CardDrawSystem.PutIntoHand(player, cardToPutIntoHand);
        player.Deck.MoveToBottom(cardToPutOnBottom);
        //the remaining card will stay on top by default.        
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        throw new NotImplementedException();
    }
}

public class RampantGrowthChoiceEffect : Effect, IEffectWithChoice
{
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;
    public override string RulesText => "Put a mana from your deck into play";

    public string ChoiceMessage { get => "Choose a mana to card to put into play from your deck"; }

    public List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.Cards.Where(c => c.IsOfType<ManaCardData>()).ToList();
    }

    public void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.ManaSystem.PlayManaCardFromEffect(player, choices.Cast<CardInstance>().First(), true);
        cardGame.CardDrawSystem.Shuffle(player);
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        return;
    }
}

public class SleightOfHandEffect : Effect, IEffectWithChoice
{
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;
    public override string RulesText => "Look at the top two cards of your deck. Put one of them into your hand and the other on the bottom of your library";

    public string ChoiceMessage => "Choose a card to put into your hand.";

    private List<CardInstance> _cardsSeen = new List<CardInstance>();

    public List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.Cards.TakeLast(2).ToList();
    }

    public void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        _cardsSeen = player.Deck.Cards.TakeLast(2).ToList();

        //Reveal the cards so that the player can make a choice
        _cardsSeen.ForEach(card =>
        {
            card.RevealedToOwner = true;
        });
    }

    public void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        //Put the choices into your hand
        foreach (var entity in choices)
        {
            var card = entity as CardInstance;
            if (card == null) continue;
            cardGame.Log($"{card.Name} has been put into {player.Name}'s hand");
            cardGame.CardDrawSystem.PutIntoHand(player, card);
            card.RevealedToOwner = false;
            _cardsSeen.Remove(card);
        };

        cardGame.Log(_cardsSeen.Count().ToString());

        //Put the remaining on the bottom.
        foreach (var card in _cardsSeen)
        {
            cardGame.Log($"{card.Name} has been moved to the bottom of the deck");
            card.RevealedToOwner = false;
            player.Deck.MoveToBottom(card);
        }
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        //This should apply the actual effect after the cards are chosen....
        //Basically choice setup should be called before Apply, then once the cards are chosen we call apply with the chosen cards.
        return;
    }
}

