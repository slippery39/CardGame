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

    private ResolvingActionInfo _currentResolvingAction;

    public DefaultResolvingSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void Add(CardInstance cardInstance, CardGameEntity target)
    {
        var resolvingCardInstance = new ResolvingCardInstanceActionInfo
        {
            Source = cardInstance,
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

    private void CompleteResolve()
    {
        cardGame.CurrentGameState = GameState.WaitingForAction;

        //Handle any post resolve effects.
        //For example, spells go to the graveyard after they resolve.
        if (_currentResolvingAction is ResolvingCardInstanceActionInfo)
        {
            var resolvingCardInstance = _currentResolvingAction as ResolvingCardInstanceActionInfo;

            if (resolvingCardInstance.Source.IsOfType<SpellCardData>())
            {
                var spell = resolvingCardInstance.Source;

                //Temporary hack to get flashback working quickly.
                if (resolvingCardInstance.SourceZone.ZoneType == ZoneType.Discard)
                {
                    cardGame.ZoneChangeSystem.MoveToZone(spell, cardGame.GetOwnerOfCard(spell).Exile);
                }
                else
                {
                    cardGame.ZoneChangeSystem.MoveToZone(spell, cardGame.GetOwnerOfCard(spell).DiscardPile);
                }
            }
        }

        _currentResolvingAction = null;
        _effectsToResolve = null;
        cardGame.StateBasedEffectSystem.CheckStateBasedEffects();
    }

    //Resolves effects one by one until it hits an effect that requires the player to make a choice.
    public void Continue()
    {
        if (!IsResolvingEffect)
        {
            CompleteResolve();
            return;
        }

        var player = cardGame.GetOwnerOfCard(_currentResolvingAction.Source);

        while (IsResolvingEffect)
        {
            var effect = _effectsToResolve.Dequeue();
            if (effect is IEffectWithChoice)
            {
                var effectWithChoice = effect as IEffectWithChoice;

                //If there are no valid choices to make, then automatically resolve it.
                if (effectWithChoice.GetValidChoices(cardGame,player).Any()== false)
                {
                    continue;
                }
                effectWithChoice.ChoiceSetup(cardGame, player, _currentResolvingAction.Source);

                //wait for the choice to be made.
                cardGame.PromptPlayerForChoice(player, effectWithChoice);
                return;
            }
            else
            {
                //Apply effects one at a time.
                cardGame.EffectsProcessor.ApplyEffect(player, _currentResolvingAction.Source, effect, _currentResolvingAction.Targets);
            }
        }

        if (!IsResolvingEffect)
        {
            CompleteResolve();
        }
    }

    private void ResolveEffects(ResolvingActionInfo info, IEnumerable<Effect> effects)
    {
        var effectsWithChoices = effects.Where(e => e is IEffectWithChoice);

        var player = cardGame.GetOwnerOfCard(info.Source);

        if (!effectsWithChoices.Any())
        {
            cardGame.EffectsProcessor.ApplyEffects(player, info.Source, effects.ToList(), info.Targets);
            return;
        }

        _effectsToResolve = new Queue<Effect>(effects);
        _effectsToResolve.Reverse();
        Continue();
    }

    public void ResolveNext()
    {

        //If for whatever reason 
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
        _currentResolvingAction = resolvingThing;

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
                        ResolveEffects(resolvingThing, resolvingAbility.Ability.Effects);
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
            else if (resolvingCardInstance.Source.CurrentCardData is SpellCardData)
            {
                ResolveEffects(resolvingCardInstance, resolvingCardInstance.Source.Effects);
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

