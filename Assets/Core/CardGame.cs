using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UniRx;
using UnityEngine;

public class CardGame
{
    #region Private Fields
    private List<Player> _players;
    private int _activePlayerId = 1;
    private int _numberOfLanes = 5;
    private int _startingPlayerHealth = 100;
    private int _startingHandSize = 5;
    //need to make sure this is serialized or else we wont be able to properly id tokens.
    [JsonProperty]
    private int _nextEntityId = 1;
    private int _nextPlayerId = 1;

    private List<CardGameEntity> _registeredEntities;

    private IBattleSystem _battleSystem;
    private IDamageSystem _damageSystem;
    private IHealingSystem _healingSystem;
    private IZoneChangeSystem _zoneChangeSystem;
    private IStateBasedEffectSystem _stateBasedEffectSystem;
    private ICardGameLogger _cardGameLogger;
    private IUnitPumpSystem _unitPumpSystem;
    private ICardDrawSystem _cardDrawSystem;
    private IManaSystem _manaSystem;
    private IUnitSummoningSystem _unitSummoningSystem;
    private ITargetSystem _targetSystem;
    private IEffectsProcessor _effectsProcessor;
    private ITurnSystem _turnSystem;
    private ISacrificeSystem _sacrificeSystem;
    private IDestroySystem _destroySystem;
    private IDiscardSystem _discardSystem;

    private IResolvingSystem _resolvingSystem;
    private IContinuousEffectSystem _continuousEffectSystem;
    private IActivatedAbilitySystem _activatedAbilitySystem;
    private IModificationsSystem _modificationsSystem;
    private IAdditionalCostSystem _additionalCostSystem;

    private IEventLogSystem _eventLogSystem;

    //Debug purposes only
    private bool _isCopy = false;
    #endregion


    #region Public Properties
    [JsonIgnore]
    public Player Player1 { get => _players.Where(p => p.PlayerId == 1).FirstOrDefault(); }
    [JsonIgnore]
    public Player Player2 { get => _players.Where(p => p.PlayerId == 2).FirstOrDefault(); }
    public List<Player> Players { get => _players; set => _players = value; }
    public int ActivePlayerId { get => _activePlayerId; set => _activePlayerId = value; }
    [JsonIgnore]
    public Player ActivePlayer { get => _players.Where(p => p.PlayerId == ActivePlayerId).FirstOrDefault(); }
    [JsonIgnore]
    public Player InactivePlayer { get => _players.Where(p => p.PlayerId != ActivePlayerId).FirstOrDefault(); }

    public int SpellsCastThisTurn { get; set; } = 0;
    public ICardGameLogger Logger { get => _cardGameLogger; }
    public List<CardGameEntity> RegisteredEntities { get => _registeredEntities; set => _registeredEntities = value; }
    #region Systems
    public IBattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }
    public IDamageSystem DamageSystem { get => _damageSystem; set => _damageSystem = value; }
    public IHealingSystem HealingSystem { get => _healingSystem; set => _healingSystem = value; }
    public IZoneChangeSystem ZoneChangeSystem { get => _zoneChangeSystem; set => _zoneChangeSystem = value; }
    public IStateBasedEffectSystem StateBasedEffectSystem { get => _stateBasedEffectSystem; set => _stateBasedEffectSystem = value; }
    public IUnitPumpSystem UnitPumpSystem { get => _unitPumpSystem; set => _unitPumpSystem = value; }
    public ICardDrawSystem CardDrawSystem { get => _cardDrawSystem; set => _cardDrawSystem = value; }

    public IItemSystem ItemSystem { get; set; }

    public IManaSystem ManaSystem { get => _manaSystem; set => _manaSystem = value; }
    public IUnitSummoningSystem UnitSummoningSystem { get => _unitSummoningSystem; set => _unitSummoningSystem = value; }
    public ITargetSystem TargetSystem { get => _targetSystem; set => _targetSystem = value; }
    public IEffectsProcessor EffectsProcessor { get => _effectsProcessor; set => _effectsProcessor = value; }
    public ITurnSystem TurnSystem { get => _turnSystem; set => _turnSystem = value; }
    public ISacrificeSystem SacrificeSystem { get => _sacrificeSystem; set => _sacrificeSystem = value; }
    public IDestroySystem DestroySystem { get => _destroySystem; set => _destroySystem = value; }
    public IResolvingSystem ResolvingSystem { get => _resolvingSystem; set => _resolvingSystem = value; }
    public IContinuousEffectSystem ContinuousEffectSystem { get => _continuousEffectSystem; set => _continuousEffectSystem = value; }

    public IActivatedAbilitySystem ActivatedAbilitySystem { get => _activatedAbilitySystem; set => _activatedAbilitySystem = value; }
    public IDiscardSystem DiscardSystem { get => _discardSystem; set => _discardSystem = value; }

    public IModificationsSystem ModificationsSystem { get => _modificationsSystem; set => _modificationsSystem = value; }
    public GameState CurrentGameState { get; set; }
    public IEffectWithChoice ChoiceInfoNeeded { get; set; } //Get this working with discards effects with, then see what we should evolve it to.
    internal IAdditionalCostSystem AdditionalCostSystem { get => _additionalCostSystem; set => _additionalCostSystem = value; }

    public ICountersSystem CountersSystem { get; set; }
    public IPlayerModificationSystem PlayerAbilitySystem { get; set; }

    public IWinLoseSystem WinLoseSystem { get; set; }
    public IEventLogSystem EventLogSystem { get => _eventLogSystem; set => _eventLogSystem = value; }

    #endregion
    #endregion

    #region Observables

    /// <summary>
    /// Game state should change after resolving any effect on the stack.
    /// Note, do not subscribe to this one, this should really only be used by core classes.
    /// </summary>

    public ReplaySubject<CardGame> OnGameStateChanged;
    public IObservable<CardGame> GameStateChangedObservable => OnGameStateChanged.AsObservable();
    #endregion

    public CardGame()
    {
        InitGame();
        //Log("Default Constructor has been called in CardGame!");
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        //Debug.Log("OnDeserialized has fired!");
    }


    private void InitGame()
    {
        _battleSystem = new DefaultBattleSystem(this);
        _damageSystem = new DefaultDamageSystem(this);
        _healingSystem = new DefaultHealingSystem(this);
        _zoneChangeSystem = new DefaultZoneChangeSystem(this);
        _stateBasedEffectSystem = new DefaultStateBasedEffectSystem(this);
        _unitPumpSystem = new DefaultUnitPumpSystem(this);
        _cardDrawSystem = new DefaultCardDrawSystem(this);
        _manaSystem = new DefaultManaSystem(this);
        _unitSummoningSystem = new DefaultUnitSummoningSystem(this);
        _targetSystem = new DefaultTargetSystem(this);
        _effectsProcessor = new DefaultEffectsProcessor(this);
        _turnSystem = new DefaultTurnSystem(this);
        _sacrificeSystem = new DefaultSacrificeSystem(this);
        _destroySystem = new DefaultDestroySystem(this);
        _resolvingSystem = new DefaultResolvingSystem(this);
        _continuousEffectSystem = new DefaultContinousEffectSystem(this);
        _activatedAbilitySystem = new DefaultActivatedAbilitySystem(this);
        _discardSystem = new DefaultDiscardSystem(this);
        _modificationsSystem = new DefaultModificationSystem(this);
        _additionalCostSystem = new DefaultAdditionalCostSystem(this);
        WinLoseSystem = new DefaultWinLoseSystem(this);

        CountersSystem = new DefaultCountersSystem(this);
        ItemSystem = new DefaultItemSystem(this);
        PlayerAbilitySystem = new PlayerModificationSystem(this);

        _cardGameLogger = new UnityCardGameLogger();

        _eventLogSystem = new EventLogSystem(this);

        _registeredEntities = new List<CardGameEntity>();
        _players = new List<Player>();


        //TODO - GameState Stuff:
        CurrentGameState = GameState.WaitingForAction;


        AddPlayerToGame(new Player(_numberOfLanes)
        {
            PlayerId = 1,
            Health = _startingPlayerHealth
        });

        AddPlayerToGame(new Player(_numberOfLanes)
        {
            PlayerId = 2,
            Health = _startingPlayerHealth
        });

        OnGameStateChanged = new ReplaySubject<CardGame>(10);

    }


    public void SetupDecks(Decklist player1Decklist, Decklist player2Decklist)
    {
        player1Decklist.ToDeck().ForEach(card =>
        {
            AddCardToGame(Player1, card, Player1.Deck);
        });


        player2Decklist.ToDeck().ForEach(card =>
        {
            AddCardToGame(Player2, card, Player2.Deck);
        });
    }

    public void StartGame()
    {
        Player1.Deck.Shuffle();
        Player2.Deck.Shuffle();
        _cardDrawSystem.DrawOpeningHand(Player1);
        _cardDrawSystem.DrawOpeningHand(Player2);

        OnGameStateChanged.OnNext(Copy());
    }

    public CardGame Copy(bool noEventsOrLogs = false)
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        });

        File.WriteAllText("tempCardGameState", json);

        var copy = JsonConvert.DeserializeObject<CardGame>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });

        if (noEventsOrLogs)
        {
            copy.EventLogSystem = new EmptyEventLogSystem();
            copy._cardGameLogger = new EmptyLogger();
        }

        copy._isCopy = true;

        return copy;


    }

    public Player GetOwnerOfCard(CardInstance unitInstance)
    {
        var owner = _players.Where(p => p.PlayerId == unitInstance.OwnerId).FirstOrDefault();

        //Might be a double sided card. Check the front card for the owner
        if (owner == null)
        {
            owner = _players.Where(p => p.PlayerId == unitInstance.FrontCard.OwnerId).FirstOrDefault();
        }

        if (owner == null)
        {
            throw new Exception("Could not find owner for card");
        }

        return owner;
    }

    public int GetNextEntityId()
    {
        return _nextEntityId++;
    }
    public int GetNextPlayerId()
    {
        return _nextPlayerId++;
    }

    public IZone GetZoneOfCard(CardInstance card)
    {
        //Edge case for double sided cards.. If this card doesn't yet exist in a zone then grab the Front cards zone.
        if (card.FrontCard != null)
        {
            return GetZoneOfCard(card.FrontCard);
        }

        return GetZones().Where(zone => zone.Cards.Select(c => c.EntityId).Contains(card.EntityId)).FirstOrDefault();
    }

    public void HandleTriggeredAbilities(IEnumerable<CardInstance> units, TriggerType triggerType)
    {
        foreach (var unit in units)
        {
            var abilities = unit.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == triggerType);

            foreach (var ab in abilities)
            {
                ResolvingSystem.Add(ab, unit);
            }
        }
    }

    public void AddCardToGame(Player player, BaseCardData data, IZone zone)
    {
        var cardInstance = new CardInstance(this, data);
        cardInstance.OwnerId = player.PlayerId;
        RegisterEntity(cardInstance);

        if (cardInstance.BackCard != null)
        {
            RegisterEntity(cardInstance.BackCard);
        }

        zone.Add(cardInstance);
    }

    public void AddPlayerToGame(Player player)
    {
        _players.Add(player);
        RegisterEntity(player);
        player.Lanes.ForEach(lane =>
        {
            RegisterEntity(lane);
        });
        //TODO add other zones as needed.
    }

    public void NextTurn()
    {
        _turnSystem.EndTurn();
        _stateBasedEffectSystem.CheckStateBasedEffects();
        _turnSystem.StartTurn();
        _stateBasedEffectSystem.CheckStateBasedEffects();

    }

    private void RegisterEntity(CardGameEntity entity)
    {
        entity.EntityId = GetNextEntityId();

        _registeredEntities.Add(entity);
    }

    internal bool IsInZone(CardInstance unit, ZoneType zoneType)
    {
        var zone = GetZoneOfCard(unit);
        return zone.ZoneType == zoneType;
    }

    internal void MakeChoice(List<CardInstance> entitiesSelected)
    {
        //Right now as of August 16 2022, Careful Study, Sleight of Hand and Chrome Mox use this...
        if (CurrentGameState != GameState.WaitingForChoice)
        {
            return;
        }

        var effectChoice = ChoiceInfoNeeded;

        effectChoice.OnChoicesSelected(this, ActivePlayer, entitiesSelected.Cast<CardGameEntity>().ToList());

        //This is mainly to get chrome mox working.
        GetCardsInPlay().ForEach(card =>
        {
            if (entitiesSelected.Any())
            {
                card.GetAbilitiesAndComponents<IOnResolveChoiceMade>().ForEach(component =>
                    component.OnResolveChoiceMade(this, entitiesSelected[0], ChoiceInfoNeeded)
                );
            }
        });
        ResolvingSystem.Continue();
    }

    /// <summary>
    /// Gets a card by the specified entity id. If a card is not found, it will return null.
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public CardInstance GetCardById(int entityId)
    {
        var card = GetEntities<CardGameEntity>().Where(e => e.EntityId == entityId && e is CardInstance).Cast<CardInstance>().FirstOrDefault();
        return card;
    }


    public bool CanPlayCard(CardInstance card, bool checkIfActivePlayer = true, List<ICastModifier> modifiers = null)
    {
        if (card == null)
        {
            return false;
        }

        var owner = GetOwnerOfCard(card);

        if (checkIfActivePlayer && ActivePlayer != owner)
        {
            return false;
        }

        //Can only play the card if it is in the owners hand.
        //We have hard coded in flashback here.
        var castZones = new List<ZoneType> { ZoneType.Hand };

        //figure out where the card can be cast from

        var modCastZoneComponents =
             card.GetAbilitiesAndComponents<IModifyCastZones>()
            .Union(owner.Modifications.GetOfType<IModifyCastZones>()).ToList();

        foreach (var modCastZoneComponent in modCastZoneComponents)
        {
            castZones = modCastZoneComponent.ModifyCastZones(this, card, castZones);
        }

        var castZoneOfCard = GetZoneOfCard(card).ZoneType;

        if (!castZones.Contains(castZoneOfCard))
        {
            return false;
        }

        if (card.IsOfType<ManaCardData>())
        {
            return ManaSystem.CanPlayManaCard(ActivePlayer, card);
        }
        else
        {
            if (!ManaSystem.CanPlayCard(owner, card, modifiers))
            {
                return false;
            }
        }

        if (card.IsOfType<SpellCardData>())
        {
            if (card.AdditionalCost != null)
            {
                if (card.AdditionalCost.CanPay(this, owner, card) == false)
                {
                    //Log($"Cannot pay the additional cost for ${card.Name}");
                    return false;
                }
            }


        }
        else if (card.IsOfType<ItemCardData>())
        {
            if (card.AdditionalCost != null)
            {
                if (card.AdditionalCost.CanPay(this, owner, card) == false)
                {
                    //Log($"Cannot pay the additional cost for ${card.Name}");
                    return false;
                }
            }
        }


        return true; //if it gets to this point it has passed all the checks, and it is ok to be played.
    }

    internal bool CanPlayCard(int entityId)
    {
        if (CurrentGameState != GameState.WaitingForAction)
        {
            return false;
        }

        var card = GetEntities<CardGameEntity>().Where(e => e.EntityId == entityId && e is CardInstance).Cast<CardInstance>().FirstOrDefault();

        return CanPlayCard(card);
    }

    public void ProcessAction(CardGameAction action)
    {
        //Action may come from anywhere, we need to make sure all the references match up to what we have in our card game before going any further.

        //Which Things would need to be updated?
        action.Player = Players.Where(e => e.EntityId == action.Player?.EntityId).FirstOrDefault();
        action.SourceCard = GetEntities<CardInstance>().Where(e => e.EntityId == action.SourceCard?.EntityId).FirstOrDefault();
        action.Targets = action.Targets?.Select(t => GetEntities<CardGameEntity>().FirstOrDefault(e => e?.EntityId == t?.EntityId)).ToList();
        action.CardToPlay = GetEntities<CardInstance>().Where(e => e.EntityId == action.CardToPlay?.EntityId).FirstOrDefault();
        action.AdditionalChoices = action.AdditionalChoices?.Select(t => GetEntities<CardGameEntity>().FirstOrDefault(e => e?.EntityId == t?.EntityId)).ToList();

        Log("Processing Action...");
        if (action.IsValidAction(this))
        {
            Log("Action is Valid...");
            action.DoAction(this);
        }
        else
        {
            Log("Could not perform action");
        }
    }

    //TODO - Refactor PlayCard to be able to Take in ActionInfo
    //TODO - Add the cast modifiers to the resolving system.
    public void PlayCard(Player player, CardGameAction action, bool checkIfActivePlayer = true)
    {

        var cardToPlay = action.CardToPlay;
        var costChoices = action.AdditionalChoices;
        var targetId = action.Targets?.FirstOrDefault()?.EntityId;

        if (cardToPlay == null)
        {
            Log("Error, trying to play a null card?");
            return;
        }

        if (!CanPlayCard(cardToPlay, checkIfActivePlayer))
        {
            Log("Unable to play the card");
            return;
        }

        //NOTE - If we are playing the back part of the double sided card, then we transform the card before playing it.
        //Potential TODO - Allow us to create different OnPlay rules for different types of cards.
        if (cardToPlay.FrontCard != null)
        {
            cardToPlay.FrontCard.TransformToCardData(cardToPlay.CurrentCardData);
            cardToPlay = cardToPlay.FrontCard;
        }

        if (cardToPlay.CurrentCardData is ManaCardData)
        {
            ManaSystem.PlayManaCard(player, cardToPlay);
            _stateBasedEffectSystem.CheckStateBasedEffects();
        }
        else if (cardToPlay.CurrentCardData is UnitCardData)
        {
            var validTargets = _targetSystem.GetValidTargets(player, cardToPlay);

            var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

            if (targetAsEntity != null)
            {
                ManaSystem.SpendMana(player, cardToPlay.ManaCost);
                AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });
                ResolvingSystem.Add(action, cardToPlay);
                _stateBasedEffectSystem.CheckStateBasedEffects();
            }
            //TODO - We should check this before we do the double sided card stuff.
            else
            {
                Log("Invalid Lane chosen for summoning unit");
                return;
            }
        }
        else if (cardToPlay.CurrentCardData is SpellCardData)
        {
            //TODO - Check if we even need to check this here?  All this stuff might be able to be handled by our Actions now.
            if (!_targetSystem.CardNeedsTargets(player, cardToPlay))
            {
                var manaSpent = cardToPlay.ManaCost;

                if (action.CastModifiers.Any())
                {
                    var mana = new Mana(manaSpent);
                    mana.AddFromString(action.CastModifiers[0].GetCost(cardToPlay));
                    manaSpent = mana.ToManaString();
                }

                ManaSystem.SpendMana(player, manaSpent);
                AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });
                player.Modifications.GetOfType<IOnSpellCast>().ForEach(mod =>
                {
                    mod.OnSpellCast(this, cardToPlay, GetZoneOfCard(cardToPlay)); //etc...
                });

                ResolvingSystem.Add(action, cardToPlay);

                //Do we need our state based effects here?
                _stateBasedEffectSystem.CheckStateBasedEffects();
            }
            else
            {
                //TODO - This Validation should happen on a different layer
                var validTargets = _targetSystem.GetValidTargets(player, cardToPlay);

                var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

                if (targetAsEntity != null)
                {
                    var manaSpent = cardToPlay.ManaCost;

                    if (action.CastModifiers.Any())
                    {
                        var mana = new Mana(manaSpent);
                        mana.AddFromString(action.CastModifiers[0].GetCost(cardToPlay));
                        manaSpent = mana.ToManaString();
                    }

                    ManaSystem.SpendMana(player, manaSpent);
                    AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });

                    //TODO - This should go to the ResolvingSystem.
                    player.Modifications.GetOfType<IOnSpellCast>().ForEach(mod =>
                    {
                        mod.OnSpellCast(this, cardToPlay, GetZoneOfCard(cardToPlay));//etc...
                    });
                    ResolvingSystem.Add(action, cardToPlay);
                    //Do we need our state based effects here?
                    _stateBasedEffectSystem.CheckStateBasedEffects();
                }
            }
        }
        else if (cardToPlay.IsOfType<ItemCardData>())
        {
            ManaSystem.SpendMana(player, cardToPlay.ManaCost);
            ResolvingSystem.Add(action, cardToPlay);
            _stateBasedEffectSystem.CheckStateBasedEffects();
        }

        //Add the event that the player has played a card.
        _eventLogSystem.AddEvent($"{player.Name} has played {cardToPlay.Name}.");
    }


    public void PromptPlayerForChoice(Player player, IEffectWithChoice effectThatNeedsChoice)
    {
        CurrentGameState = GameState.WaitingForChoice;//GameState.WaitingForChoice;
        ChoiceInfoNeeded = effectThatNeedsChoice;
    }


    //Grab a master list of all zones in the game.
    public List<IZone> GetZones()
    {
        //TODO - generalize this?

        List<IZone> zones = new List<IZone>();

        zones.Add(Player1.Hand);
        zones.Add(Player2.Hand);

        zones.AddRange(Player1.Lanes);
        zones.AddRange(Player2.Lanes);

        zones.Add(Player1.Deck);
        zones.Add(Player2.Deck);

        zones.Add(Player1.DiscardPile);
        zones.Add(Player2.DiscardPile);

        zones.Add(ResolvingSystem.Stack);

        zones.Add(Player1.Exile);
        zones.Add(Player2.Exile);

        zones.Add(Player1.Items);
        zones.Add(Player2.Items);

        return zones;
    }

    public List<T> GetEntities<T>() where T : CardGameEntity
    {
        return _registeredEntities.Where(e => e is T).Cast<T>().ToList();
    }
    public List<CardInstance> GetUnitsInPlay()
    {

        var player1Units = Player1.GetUnitsInPlay();
        var player2Units = Player2.GetUnitsInPlay();

        return player1Units.Concat(player2Units).ToList();
    }

    public CardInstance GetCardThatHasResponseAbility(IRespondToCast ability)
    {
        //Check players hands;
        var cardsInHand = Player1.Hand.Union(Player2.Hand).ToList();

        foreach (var card in cardsInHand)
        {
            if (card.Abilities.Contains((CardAbility)ability))
            {
                return card;
            }
        }

        return null; //card cannot be found.
    }

    public void TestFillGraveyard()
    {
        var db = new CardDatabase();
        var cards = db.GetAll();
        for (var i = 0; i < 20; i++)
        {
            var randomCard = cards.Randomize().First();
            AddCardToGame(Player1, randomCard, Player1.DiscardPile);
            randomCard = cards.Randomize().First();
            AddCardToGame(Player2, randomCard, Player2.DiscardPile);
        }
    }

    public bool IsInPlay(CardInstance card)
    {
        var zone = GetZoneOfCard(card);
        return new List<ZoneType> { ZoneType.InPlay }.Contains(zone.ZoneType);
    }

    public List<CardInstance> GetCardsInPlay()
    {
        return Player1.GetCardsInPlay().Union(Player2.GetCardsInPlay()).ToList();
    }

    public List<CardInstance> GetCardsInPlay(Player player)
    {
        return player.GetCardsInPlay();
    }

    //For general console logging purposes.
    public void Log(string message)
    {
        if (Logger == null)
        {
            //default to unity if null;
            Debug.Log(message);
            return;
        }
        Logger.Log(message);
    }
    private void AddRandomUnitsToLane(Player player)
    {
        CardDatabase db = new CardDatabase();
        var unitsOnly = db.GetAll().Where(c => c.GetType() == typeof(UnitCardData)).ToList();

        var rng = new System.Random();

        foreach (Lane lane in player.Lanes)
        {
            var randomIndex = rng.Next(0, unitsOnly.Count());
            AddCardToGame(player, unitsOnly[randomIndex], lane);
        }
    }

    private void AddRandomCardsToHand(Player player)
    {
        CardDatabase db = new CardDatabase();
        var cards = db.GetAll().Where(c => c.Name == "City of Brass").ToList();

        var rng = new System.Random();

        for (int i = 0; i < _startingHandSize; i++)
        {
            AddCardToGame(player, cards.Randomize().ToList()[0], player.Hand);
        }
    }

    void BuildDeck(Player player, CardColor deckColor, string manaName)
    {
        var cardDB = new CardDatabase();

        //var cardsToSelectFrom = cardDB.GetAll().Where(card => card.Colors.Contains(deckColor) || card.Colors.Contains(CardColor.Colorless));
        //var cardsToSelectFrom = cardDB.GetAll().Where(card => card is SpellCardData).ToList();
        // var cardsToSelectFrom = cardDB.GetAll().Where(card => card.GetAbilities<ActivatedAbility>().Any() && card.Colors.Contains(CardColor.Blue));
        //var cardsToSelectFrom = cardDB.GetAll();
        var cardsToSelectFrom = cardDB.GetAll().Where(card => card.Name == "Oracle of Mul Daya");
        //Testing out if we can instantiate an affinity deck.


        var decklist = FamousDecks.RGValakut2011().ToDeck();

        decklist.ForEach(card =>
        {
            AddCardToGame(player, card, player.Deck);
        });

        player.Deck.Shuffle();
        return;
    }

    void AddRandomCardsToDeck()
    {
        var cardDB = new CardDatabase();
        var cards = cardDB.GetAll().ToList();

        var manaNameLookup = new Dictionary<CardColor, string>()
        {
            { CardColor.White, "Plains" },
            { CardColor.Black, "Swamp" },
            { CardColor.Blue,"Island" },
            {CardColor.Green,"Forest" },
            {CardColor.Red,"Mountain" }
        };

        var deckColor = new List<CardColor>() { CardColor.White, CardColor.Blue, CardColor.Black, CardColor.Red, CardColor.Green }
        .Randomize().ToList()[0];

        BuildDeck(Player1, deckColor, manaNameLookup[deckColor]);

        var deckColor2 = new List<CardColor>() { CardColor.White, CardColor.Blue, CardColor.Black, CardColor.Red, CardColor.Green }
        .Randomize().ToList()[0];
        BuildDeck(Player2, deckColor2, manaNameLookup[deckColor2]);

    }
}
