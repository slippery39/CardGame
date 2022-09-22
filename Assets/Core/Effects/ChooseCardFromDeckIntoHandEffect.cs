﻿using System.Collections.Generic;
using System.Linq;

public class ChooseCardFromDeckIntoHandEffect : Effect, IEffectWithChoice
{
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;
    public override string RulesText => "Put a #cardType# from your deck into your hand".Replace("#cardType#", Filter.RulesTextString(false).ToLower());
    public string ChoiceMessage { get => "Choose a card to put into your hand"; }

    public List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return CardFilter.ApplyFilter(player.Deck.Cards, Filter);
    }

    public void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.CardDrawSystem.PutIntoHand(player, choices.Cast<CardInstance>().First());
        cardGame.CardDrawSystem.Shuffle(player);
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        return;
    }
}
