using System;
using System.Collections.Generic;
using System.Linq;

public interface IResolvingSystem
{
    public void Add(CardInstance cardInstance, CardGameEntity target);
    public IZone Stack { get; }
    public void Add(CardAbility ability, CardInstance source);

    public void Add(CardAbility ability, CardInstance source, CardGameEntity target);

    public void Cancel(ResolvingActionInfo action);

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

    public void ResolveNext()
    {
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

                        var effectsWithChoices = activatedAbility.Effects.Where(e => e is DiscardCardEffect && e.TargetType == TargetType.Self);

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
                cardGame.SpellCastingSystem.CastSpell(player, resolvingCardInstance.CardInstance, resolvingCardInstance.Targets, resolvingCardInstance);

                var spellCardData = resolvingCardInstance.CardInstance.CurrentCardData as SpellCardData;

                //if does need a choice upon resolution, the game needs to account for that.
                //For now we are just considering discard effects
                //TODO - the way we are doing this will cause bugs in the future. We need a way of "pausing" the resolving of the spell until all of the choices are
                //done.
                var effectsWithChoices = spellCardData.Effects.Where(e => e is DiscardCardEffect && e.TargetType == TargetType.Self);

                if (effectsWithChoices.Any())
                {
                    cardGame.PromptPlayerForChoice(player, effectsWithChoices.First());
                }
                //TODO - Properly Handle choices that must be made upon resolving spells.
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

