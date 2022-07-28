using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CardGame
{
    #region Private Fields
    private List<Player> _players;
    private int _activePlayerId = 1;
    private int _numberOfLanes = 5;
    private int _startingPlayerHealth = 100;
    private int _startingHandSize = 5;
    private int _nextEntityId = 1;
    private int _nextPlayerId = 1;

    private List<CardGameEntity> _registeredEntities;

    private IBattleSystem _battleSystem;
    private IDamageSystem _damageSystem;
    private IHealingSystem _healingSystem;
    private ISpellCastingSystem _spellCastingSystem;
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
    #endregion


    #region Public Properties
    public Player Player1 { get => _players.Where(p => p.PlayerId == 1).FirstOrDefault(); }
    public Player Player2 { get => _players.Where(p => p.PlayerId == 2).FirstOrDefault(); }
    public List<Player> Players { get => _players; set => _players = value; }
    public int ActivePlayerId { get => _activePlayerId; set => _activePlayerId = value; }
    public Player ActivePlayer { get => _players.Where(p => p.PlayerId == ActivePlayerId).FirstOrDefault(); }
    public Player InactivePlayer { get => _players.Where(p => p.PlayerId != ActivePlayerId).FirstOrDefault(); }
    public ICardGameLogger Logger { get => _cardGameLogger; }
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
    public ISpellCastingSystem SpellCastingSystem { get => _spellCastingSystem; set => _spellCastingSystem = value; }
    public IResolvingSystem ResolvingSystem { get => _resolvingSystem; set => _resolvingSystem = value; }
    public IContinuousEffectSystem ContinuousEffectSystem { get => _continuousEffectSystem; set => _continuousEffectSystem = value; }

    public IActivatedAbilitySystem ActivatedAbilitySystem { get => _activatedAbilitySystem; set => _activatedAbilitySystem = value; }
    public IDiscardSystem DiscardSystem { get => _discardSystem; set => _discardSystem = value; }

    public IModificationsSystem ModificationsSystem { get => _modificationsSystem; set => _modificationsSystem = value; }
    public GameState CurrentGameState { get; set; }
    public Effect ChoiceInfoNeeded { get; set; } //Get this working with discards effects with, then see what we should evolve it to.
    internal IAdditionalCostSystem AdditionalCostSystem { get => _additionalCostSystem; set => _additionalCostSystem = value; }

    public ICountersSystem CountersSystem { get; set; }
    public IPlayerModificationSystem PlayerAbilitySystem { get; set; }

    #endregion
    #endregion

    public CardGame()
    {
        _battleSystem = new DefaultBattleSystem(this);
        _damageSystem = new DefaultDamageSystem(this);
        _healingSystem = new DefaultHealingSystem(this);
        _spellCastingSystem = new DefaultSpellCastingSystem(this);
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

        CountersSystem = new DefaultCountersSystem(this);
        ItemSystem = new DefaultItemSystem(this);
        PlayerAbilitySystem = new PlayerModificationSystem(this);

        _cardGameLogger = new UnityCardGameLogger();

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
        AddRandomCardsToDeck();

        //Need to use the card draw system to draw the opening hand.
        _cardDrawSystem.DrawOpeningHand(Player1);
        _cardDrawSystem.DrawOpeningHand(Player2);
    }

    public Player GetOwnerOfCard(CardInstance unitInstance)
    {
        var owner = _players.Where(p => p.PlayerId == unitInstance.OwnerId).FirstOrDefault();

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
        if (CurrentGameState != GameState.WaitingForChoice)
        {
            return;
        }

        //This is mainly to get chrome mox working.
        GetCardsInPlay().ForEach(card =>
        {
            card.GetAbilitiesAndComponents<IOnResolveChoiceMade>().ForEach(component =>
                component.OnResolveChoiceMade(this, entitiesSelected[0], ChoiceInfoNeeded)
            );
        });


        //For now we are just handling discard choices....
        //In the future we might have other choices, like choosing a creature to sacrifice or perhaps some other choice like choosing a type to destroy?
        DiscardSystem.Discard(ActivePlayer, entitiesSelected);

        CurrentGameState = GameState.WaitingForAction;
    }

    public CardInstance GetCardById(int entityId)
    {
        var card = GetEntities<CardGameEntity>().Where(e => e.EntityId == entityId && e is CardInstance).Cast<CardInstance>().FirstOrDefault();

        if (card == null)
        {
            Logger.Log($"Could not find card, invalid entity id {entityId}");
        }

        return card;
    }

    public bool CanPlayCard(CardInstance card)
    {
        if (card == null)
        {
            return false;
        }

        var owner = GetOwnerOfCard(card);

        if (ActivePlayer != owner)
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

        //TODO - check if the card is in one of the cast zones

        var castZoneOfCard = GetZoneOfCard(card).ZoneType;

        if (!castZones.Contains(castZoneOfCard))
        {
            return false;
        }
        //Other checks: is the card in the players hand. Otherwise they cannot play it.

        if (card.IsOfType<ManaCardData>())
        {
            return ManaSystem.CanPlayManaCard(ActivePlayer, card);
            //check if we can the mana card.
        }
        else
        {
            if (!ManaSystem.CanPlayCard(owner, card))
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
                    Log($"Cannot pay the additional cost for ${card.Name}");
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
                    Log($"Cannot pay the additional cost for ${card.Name}");
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


    //PlayCard needs an ActionInfo
    public void PlayCard(Player player, CardInstance cardToPlay, int targetId, List<CardGameEntity> costChoices)
    {

        if (!CanPlayCard(cardToPlay.EntityId))
        {
            Log("Unable to play the card");
            return;
        }

        if (cardToPlay.CurrentCardData is ManaCardData)
        {
            ManaSystem.PlayManaCard(player, cardToPlay);
        }
        else if (cardToPlay.CurrentCardData is UnitCardData)
        {
            var validTargets = _targetSystem.GetValidTargets(player, cardToPlay);

            var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

            if (targetAsEntity != null)
            {
                ManaSystem.SpendMana(player, cardToPlay.ManaCost);
                AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });
                ResolvingSystem.Add(cardToPlay, targetAsEntity);
                _stateBasedEffectSystem.CheckStateBasedEffects();
            }
            else
            {
                Log("Invalid Lane chosen for summoning unit");
                return;
            }
        }
        else if (cardToPlay.CurrentCardData is SpellCardData)
        {
            if (!_targetSystem.CardNeedsTargets(player, cardToPlay))
            {
                ManaSystem.SpendMana(player, cardToPlay.ManaCost);
                AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });
                player.Modifications.GetOfType<IOnSpellCast>().ForEach(mod =>
                {
                    mod.OnSpellCast(this, cardToPlay, GetZoneOfCard(cardToPlay)); //etc...
                });
                ResolvingSystem.Add(cardToPlay, null);
                //Do we need our state based effects here?
                _stateBasedEffectSystem.CheckStateBasedEffects();
            }
            else
            {
                var validTargets = _targetSystem.GetValidTargets(player, cardToPlay);

                var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

                if (targetAsEntity != null)
                {
                    ManaSystem.SpendMana(player, cardToPlay.ManaCost);
                    AdditionalCostSystem.PayAdditionalCost(player, cardToPlay, cardToPlay.AdditionalCost, new CostInfo { EntitiesChosen = costChoices });
                    player.Modifications.GetOfType<IOnSpellCast>().ForEach(mod =>
                    {
                        mod.OnSpellCast(this, cardToPlay, GetZoneOfCard(cardToPlay));//etc...
                    });
                    ResolvingSystem.Add(cardToPlay, targetAsEntity);
                    //Do we need our state based effects here?
                    _stateBasedEffectSystem.CheckStateBasedEffects();
                }
            }
        }
        else if (cardToPlay.IsOfType<ItemCardData>())
        {
            ManaSystem.SpendMana(player, cardToPlay.ManaCost);
            ResolvingSystem.Add(cardToPlay, null);
            _stateBasedEffectSystem.CheckStateBasedEffects();

        }
    }


    public void PromptPlayerForChoice(Player player, Effect effectThatNeedsChoice)
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

    public CardInstance GetCardThatHasResponseAbility(RespondToCastAbility ability)
    {
        //Check players hands;
        var cardsInHand = Player1.Hand.Cards.Union(Player2.Hand.Cards).ToList();

        foreach (var card in cardsInHand)
        {
            if (card.Abilities.Contains(ability))
            {
                return card;
            }
        }

        return null; //card cannot be found.
    }

    public bool IsInPlay(CardInstance card)
    {
        var zone = GetZoneOfCard(card);
        return new List<ZoneType> { ZoneType.InPlay, ZoneType.Items }.Contains(zone.ZoneType);
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
        Logger.Log(message);
    }
    private void AddRandomUnitsToLane(Player player)
    {
        CardDatabase db = new CardDatabase();
        var unitsOnly = db.GetAll().Where(c => c.GetType() == typeof(UnitCardData)).ToList();

        var rng = new Random();

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

        var rng = new Random();

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
        var cardsToSelectFrom = cardDB.GetAll().Where(card =>
       card.Abilities.GetOfType<StaticAbility>().Any());
        //var cardsToSelectFrom = cardDB.GetAll().Where(card => card.Name == "Deep Analysis");
        var cardsToAdd = 45;

        //Testing out if we can instantiate an affinity deck.


        var decklist = Decklist.ConvertToDeck(Decklist.RGValakut2011());

        decklist.ForEach(card =>
        {
            AddCardToGame(player, card, player.Deck);
        });

        player.Deck.Shuffle();
        return;


        //OLD Randomize code

        for (int i = 0; i < cardsToAdd; i++)
        {
            AddCardToGame(player, cardsToSelectFrom.Randomize().ToList()[0], player.Deck);
        }

        for (int i = 0; i < 60 - cardsToAdd; i++)
        {
            AddCardToGame(player, cardDB.GetCardData(manaName), player.Deck);
        }

        player.Deck.Shuffle();
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
