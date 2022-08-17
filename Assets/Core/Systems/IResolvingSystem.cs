﻿using System;
using System.Collections.Generic;
using System.Linq;

public interface IResolvingSystem
{
    public void Add(CardInstance cardInstance, CardGameEntity target);
    public IZone Stack { get; }
    public void Add(CardAbility ability, CardInstance source);

    public void Add(CardAbility ability, CardInstance source, CardGameEntity target);

    public void Cancel(ResolvingActionInfo action);

    public bool IsResolvingEffect { get; }

    public void Continue();

    public void ResolveNext();
}


public class ResolvingStack : IZone
{
    public CardGame cardGame;

    public string Name => "Stack";

    public List<CardInstance> Cards { get; set; }

    public ZoneType ZoneType => ZoneType.Stack;

    public ResolvingStack()
    {
        Cards = new List<CardInstance>();
    }

    public void Add(CardInstance card)
    {
        Cards.Add(card);
    }

    public void Remove(CardInstance card)
    {
        Cards.Remove(card);
    }
}

public class DefaultResolvingSystem : IResolvingSystem
{
    private CardGame cardGame;

    //Can hold card instances and abilities
    private List<ResolvingActionInfo> _internalStack = new List<ResolvingActionInfo>();
    //Can only hold card instances.
    private IZone stackZone = new ResolvingStack();
    public IZone Stack { get { return stackZone; } }

    public bool IsResolvingEffect => _effectsToResolve != null && _effectsToResolve.Any();
    private Queue<Effect> _effectsToResolve;

    private ResolvingCardInstanceActionInfo _currentResolvingCardInstance;

    public DefaultResolvingSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void Add(CardInstance cardInstance, CardGameEntity target)
    {
        var resolvingCardInstance = new ResolvingCardInstanceActionInfo
        {
            CardInstance = cardInstance,
            Targets = new List<CardGameEntity> { target },
            SourceZone = cardGame.GetZoneOfCard(cardInstance),
        };

        cardGame.ZoneChangeSystem.MoveToZone(cardInstance, stackZone);

        _internalStack.Add(resolvingCardInstance);

        //Search each players hands for RespondToCastAbilities
        //And resolve those abilities if necessary.
        //TODO - possibly we only need to look for the opponents cards.
        var playersHands = cardGame.Player1.Hand.Cards.Union(cardGame.Player2.Hand.Cards);
        playersHands.Where(card => card.Abilities.GetOfType<RespondToCastAbility>().Any()).ToList()
        .ForEach(card =>
            {
                card.GetAbilitiesAndComponents<RespondToCastAbility>().ForEach(ab =>
                {
                    ab.BeforeSpellResolve(cardGame, resolvingCardInstance);
                });
            });



        //Our abilities auto resolve.
        this.ResolveNext();
    }

    public void Add(CardAbility ability, CardInstance source)
    {
        var resolvingAbility = new ResolvingAbilityActionInfo
        {
            Ability = ability,
            Owner = cardGame.GetOwnerOfCard(source),
            Source = source,
            SourceZone = cardGame.GetZoneOfCard(source)
        };

        _internalStack.Add(resolvingAbility);

        //Our abilities auto resolve.
        this.ResolveNext();
    }

    //This is  very similar to the add aboce.
    public void Add(CardAbility ability, CardInstance source, CardGameEntity target)
    {
        var resolvingAbility = new ResolvingAbilityActionInfo
        {
            Ability = ability,
            Owner = cardGame.GetOwnerOfCard(source),
            Source = source,
            Targets = new List<CardGameEntity> { target },
            SourceZone = cardGame.GetZoneOfCard(source)
        };

        _internalStack.Add(resolvingAbility);

        //Our abilities auto resolve.
        this.ResolveNext();
    }

    public void Cancel(ResolvingActionInfo action)
    {
        _internalStack.Remove(action);

        var caAction = action as ResolvingCardInstanceActionInfo;

        if (caAction != null)
        {
            cardGame.ZoneChangeSystem.MoveToZone(caAction.CardInstance, cardGame.GetOwnerOfCard(caAction.CardInstance).DiscardPile);
        }
        cardGame.Log($"has been cancelled (by mana leak probably");
    }

    //Resolves effects one by one until it hits an effect that requires the player to make a choice.
    public void Continue()
    {
        if (!IsResolvingEffect)
        {
            cardGame.CurrentGameState = GameState.WaitingForAction;
            _currentResolvingCardInstance = null;
            _effectsToResolve = null;
            return;
        }

        var player = cardGame.GetOwnerOfCard(_currentResolvingCardInstance.CardInstance);

      
        var effect = _effectsToResolve.Dequeue();
        if (effect is IEffectWithChoice)
        {
            var effectWithChoice = effect as IEffectWithChoice;
            effectWithChoice.ChoiceSetup(cardGame, player, _currentResolvingCardInstance.CardInstance);

            //wait for the choice to be made.
            cardGame.PromptPlayerForChoice(player, effect);
            return;
        }
        else
        {
            //Apply effects one at a time.
            cardGame.EffectsProcessor.ApplyEffect(player, _currentResolvingCardInstance.CardInstance, effect, _currentResolvingCardInstance.Targets);
        }

        if (!IsResolvingEffect)
        {
            cardGame.CurrentGameState = GameState.WaitingForAction;
        }
    }

    public void ResolveNext()
    {

        if (IsResolvingEffect)
        {
            Continue();
            return;
        }

        if (_internalStack.Count == 0)
        {
            return;
        }

        //We will need the ability to partially resolve spells (i.e. spells with choice effects like Careful study)
        //but not fully resolve it until the choices have been chosen
        var nextIndex = _internalStack.Count - 1;
        var resolvingThing = _internalStack[nextIndex];
        _internalStack.RemoveAt(nextIndex);

        //TODO - need to change this to resolving triggered ability? or just resolving ability in general?
        if (resolvingThing is ResolvingAbilityActionInfo)
        {
            var resolvingAbility = (ResolvingAbilityActionInfo)resolvingThing;

            switch (resolvingAbility.Ability)
            {
                case TriggeredAbility:
                    {
                        var triggeredAbility = (TriggeredAbility)resolvingAbility.Ability;
                        cardGame.EffectsProcessor.ApplyEffects(resolvingAbility.Owner, resolvingAbility.Source, triggeredAbility.Effects, new List<CardGameEntity>());
                        return;
                    }
                case ActivatedAbility:
                    {
                        var activatedAbility = (ActivatedAbility)resolvingAbility.Ability;
                        cardGame.EffectsProcessor.ApplyEffects(resolvingAbility.Owner, resolvingAbility.Source, activatedAbility.Effects, resolvingAbility.Targets);

                        var player = cardGame.GetOwnerOfCard(resolvingAbility.Source);

                        var effectsWithChoices = activatedAbility.Effects.Where(e => e is DiscardCardEffect && e.TargetType == TargetType.PlayerSelf);

                        if (effectsWithChoices.Any())
                        {
                            cardGame.PromptPlayerForChoice(player, effectsWithChoices.First());
                        }
                        return;
                    }
                default:
                    {
                        throw new Exception("Cannot resolve unknown ability type.");
                    }
            }

        }
        else if (resolvingThing is ResolvingCardInstanceActionInfo)
        {
            var resolvingCardInstance = (ResolvingCardInstanceActionInfo)resolvingThing;
            //Handle the resolving of a unit
            if (resolvingCardInstance.CardInstance.CurrentCardData is UnitCardData)
            {
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.UnitSummoningSystem.SummonUnit(player, resolvingCardInstance.CardInstance, resolvingThing.Targets.First().EntityId);
            }
            //Handle the resolving of a spell
            else if (resolvingCardInstance.CardInstance.CurrentCardData is SpellCardData)
            {
                //if doesn't need a choice:
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);

                var spellCardData = resolvingCardInstance.CardInstance.CurrentCardData as SpellCardData;
                var effectsWithChoices = spellCardData.Effects.Where(e => (e is DiscardCardEffect || e is SleightOfHandEffect) && e.TargetType == TargetType.PlayerSelf);

                if (!effectsWithChoices.Any())
                {
                    cardGame.SpellCastingSystem.CastSpell(player, resolvingCardInstance.CardInstance, resolvingCardInstance.Targets, resolvingCardInstance);
                    return;
                }

                //TODO Cast Spell needs to be a process

                //Effects Flow with Choices

                //If we are selecting a choice as a part of resolving an effect, then the resolving system will always be in a 
                //partially resolving state.

                //Check to see if any choices are needed for the effect
                //If not, we can resolve the whole spell
                //If so, resolve everything until the next effect with a choice
                //Save the current spot in our resolving spell here.
                //Prompt the card game that the player needs to make a choice.
                //When the user makes the choice, continue on resolving the spell.

                
                _effectsToResolve = new Queue<Effect>(spellCardData.Effects);
                _effectsToResolve.Reverse();

                //TODO - Left off here,  
                while (_effectsToResolve.Any())
                {
                    _currentResolvingCardInstance = resolvingCardInstance;
                    var effect = _effectsToResolve.Dequeue();
                    if (effect is IEffectWithChoice)
                    {
                        var effectWithChoice = effect as IEffectWithChoice;
                        effectWithChoice.ChoiceSetup(cardGame, player, resolvingCardInstance.CardInstance);
                        //wait for the choice to be made.
                        cardGame.PromptPlayerForChoice(player, effect);
                        return;
                    }
                    else
                    {
                        //Apply effects one at a time.
                        cardGame.EffectsProcessor.ApplyEffect(player, resolvingCardInstance.CardInstance, effect, resolvingCardInstance.Targets);
                    }
                }

            }
            else if (resolvingCardInstance.CardInstance.CurrentCardData is ItemCardData)
            {
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.ItemSystem.PlayItem(player, resolvingCardInstance.CardInstance, resolvingCardInstance.Targets, resolvingCardInstance);
            }

        }
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }
}

