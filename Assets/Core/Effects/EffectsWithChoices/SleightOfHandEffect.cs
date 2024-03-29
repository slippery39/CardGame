﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SleightOfHandEffect : EffectWithChoice
{
    public override string RulesText => "Look at the top two cards of your deck. Put one of them into your hand and the other on the bottom of your library";

    public override string ChoiceMessage => "Choose a card to put into your hand.";

    public override int NumberOfChoices { get; set; } = 1;

    private List<CardInstance> _cardsSeen = new List<CardInstance>();

    public SleightOfHandEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Deck.TakeLast(2).ToList();
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        _cardsSeen = player.Deck.TakeLast(2).ToList();

        //Reveal the cards so that the player can make a choice
        _cardsSeen.ForEach(card =>
        {
            card.RevealedToOwner = true;
        });
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
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

