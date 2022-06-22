﻿using System;
using System.Collections.Generic;
using System.Linq;

public interface IResolvingSystem
{
    public void Add(CardGame cardGame, CardInstance cardInstance, CardGameEntity target);
    public IZone Stack { get; }
    public void Add(CardGame cardGame, CardAbility ability, CardInstance source);

    public void Add(CardGame cardGame, CardAbility ability, CardInstance source, CardGameEntity target);
    public void ResolveNext(CardGame cardGame);
}


public class ResolvingStack : IZone
{
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
    //Can hold card instances and abilities
    private List<ResolvingEntity> _internalStack = new List<ResolvingEntity>();
    //Can only hold card instances.
    private IZone stackZone = new ResolvingStack();

    public IZone Stack { get { return stackZone; } }

    public void Add(CardGame cardGame, CardInstance cardInstance, CardGameEntity target)
    {
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, cardInstance, stackZone);
        //TODO - remove card instance from zone
        var resolvingCardInstance = new ResolvingCardInstance { CardInstance = cardInstance, Targets = new List<CardGameEntity> { target } };
        //remove the card instance from its zone
        _internalStack.Add(resolvingCardInstance);

        //Our abilities auto resolve.
        this.ResolveNext(cardGame);
    }

    public void Add(CardGame cardGame, CardAbility ability, CardInstance source)
    {
        var resolvingAbility = new ResolvingAbility
        {
            Ability = ability,
            Owner = cardGame.GetOwnerOfCard(source),
            Source = source
        };

        _internalStack.Add(resolvingAbility);

        //Our abilities auto resolve.
        this.ResolveNext(cardGame);
    }

    //This is  very similar to the add aboce.
    public void Add(CardGame cardGame, CardAbility ability, CardInstance source, CardGameEntity target)
    {
        var resolvingAbility = new ResolvingAbility
        {
            Ability = ability,
            Owner = cardGame.GetOwnerOfCard(source),
            Source = source
        };

        _internalStack.Add(resolvingAbility);

        //Our abilities auto resolve.
        this.ResolveNext(cardGame);
    }

    public void ResolveNext(CardGame cardGame)
    {
        if (_internalStack.Count == 0)
        {
            return;
        }

        var nextIndex = _internalStack.Count - 1;
        var resolvingThing = _internalStack[nextIndex];
        _internalStack.RemoveAt(nextIndex);

        //TODO - need to change this to resolving triggered ability? or just resolving ability in general?
        if (resolvingThing is ResolvingAbility)
        {
            var resolvingAbility = (ResolvingAbility)resolvingThing;

            switch (resolvingAbility.Ability)
            {
                case TriggeredAbility:
                    {
                        var triggeredAbility = (TriggeredAbility)resolvingAbility.Ability;
                        cardGame.EffectsProcessor.ApplyEffects(cardGame, resolvingAbility.Owner, resolvingAbility.Source, triggeredAbility.Effects, new List<CardGameEntity>());
                        return;
                    }
                case ActivatedAbility:
                    {
                        var activatedAbility = (ActivatedAbility)resolvingAbility.Ability;
                        cardGame.EffectsProcessor.ApplyEffects(cardGame, resolvingAbility.Owner, resolvingAbility.Source, new List<Effect> { activatedAbility.AbilityEffect }, new List<CardGameEntity>());

                        var player = cardGame.GetOwnerOfCard(resolvingAbility.Source);

                        //How to grab compound effects?

                        List<Effect> actualEffects;

                        if (activatedAbility.AbilityEffect is CompoundEffect)
                        {
                            actualEffects = ((CompoundEffect)activatedAbility.AbilityEffect).Effects;
                        }
                        else
                        {
                            actualEffects = new List<Effect> { activatedAbility.AbilityEffect };
                        }
                        var effectsWithChoices = actualEffects.Where(e => e is DiscardCardEffect && e.TargetType == TargetType.Self);

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
        else if (resolvingThing is ResolvingCardInstance)
        {
            var resolvingCardInstance = (ResolvingCardInstance)resolvingThing;
            //Handle the resolving of a unit
            if (resolvingCardInstance.CardInstance.CurrentCardData is UnitCardData)
            {
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.UnitSummoningSystem.SummonUnit(cardGame, player, resolvingCardInstance.CardInstance, resolvingThing.Targets.First().EntityId);

            }
            //Handle the resolving of a spell
            else if (resolvingCardInstance.CardInstance.CurrentCardData is SpellCardData)
            {
                //if doesn't need a choice:
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.SpellCastingSystem.CastSpell(cardGame, player, resolvingCardInstance.CardInstance, resolvingCardInstance.Targets);

                var spellCardData = resolvingCardInstance.CardInstance.CurrentCardData as SpellCardData;

                //if does need a choice upon resolution, the game needs to account for that.
                //For now we are just considering discard effects
                var effectsWithChoices = spellCardData.Effects.Where(e => e is DiscardCardEffect && e.TargetType == TargetType.Self);

                if (effectsWithChoices.Any())
                {
                    cardGame.PromptPlayerForChoice(player, effectsWithChoices.First());
                }
                //TODO - Handle choices that must be made upon resolving spells.

            }

        }
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects(cardGame);
    }
}

public class ResolvingEntity
{
    public Player Owner { get; set; }
    public CardInstance Source { get; set; }
    public List<CardGameEntity> Targets { get; set; }
}

public class ResolvingAbility : ResolvingEntity
{
    public CardAbility Ability { get; set; }
}
public class ResolvingCardInstance : ResolvingEntity
{
    public CardInstance CardInstance { get; set; }
}