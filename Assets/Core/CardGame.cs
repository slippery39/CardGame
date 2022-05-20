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

    private IResolvingSystem _resolvingSystem;
    private IContinuousEffectSystem _continuousEffectSystem;
    private IActivatedAbilitySystem _activatedAbilitySystem;

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


    #endregion
    #endregion

    public CardGame()
    {
        _registeredEntities = new List<CardGameEntity>();
        _players = new List<Player>();

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

        //Create Random Cards in Each Lane 
        //AddRandomUnitsToLane(Player1);
        //AddRandomUnitsToLane(Player2);

        AddRandomCardsToHand(Player1);
        AddRandomCardsToHand(Player2);
        AddRandomCardsToDeck();

        _battleSystem = new DefaultBattleSystem();
        _damageSystem = new DefaultDamageSystem();
        _healingSystem = new DefaultHealingSystem();
        _spellCastingSystem = new DefaultSpellCastingSystem();
        _zoneChangeSystem = new DefaultZoneChangeSystem();
        _stateBasedEffectSystem = new DefaultStateBasedEffectSystem();
        _unitPumpSystem = new DefaultUnitPumpSystem();
        _cardDrawSystem = new DefaultCardDrawSystem();
        _manaSystem = new DefaultManaSystem();
        _unitSummoningSystem = new DefaultUnitSummoningSystem();
        _targetSystem = new DefaultTargetSystem();
        _effectsProcessor = new DefaultEffectsProcessor();
        _turnSystem = new DefaultTurnSystem();
        _sacrificeSystem = new DefaultSacrificeSystem();
        _destroySystem = new DefaultDestroySystem();
        _resolvingSystem = new DefaultResolvingSystem();
        _continuousEffectSystem = new DefaultContinousEffectSystem();
        _activatedAbilitySystem = new DefaultActivatedAbilitySystem();

        _cardGameLogger = new UnityCardGameLogger();
        //TODO - some sort of check to make sure all systems are initialized?
        //maybe have 
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
            var abilities = unit.GetAbilities<TriggeredAbility>().Where(ab => ab.TriggerType == triggerType);

            foreach (var ab in abilities)
            {
                ResolvingSystem.Add(this, ab, unit);
            }
        }
    }

    public void AddCardToGame(Player player, BaseCardData data, IZone zone)
    {
        var cardInstance = new CardInstance(data);
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
        _turnSystem.EndTurn(this);
        _stateBasedEffectSystem.CheckStateBasedEffects(this);
        _turnSystem.StartTurn(this);
        _stateBasedEffectSystem.CheckStateBasedEffects(this);

    }

    private void RegisterEntity(CardGameEntity entity)
    {
        entity.EntityId = GetNextEntityId();
        _registeredEntities.Add(entity);
    }

    /*
    public void ResolveNext()
    {
        ResolvingSystem.ResolveNext(this);
    }
    */

    public void PlayCardFromHand(Player player, CardInstance cardFromHand, int targetId)
    {

        if (cardFromHand.CurrentCardData is ManaCardData)
        {
            if (!ManaSystem.CanPlayManaCard(this, player, cardFromHand))
            {
                Log("$Cannot play mana");
                return;
            }

            //var manaCard = (ManaCardData)cardFromHand.CurrentCardData;
            ManaSystem.PlayManaCard(this, player, cardFromHand);
        }
        if (cardFromHand.CurrentCardData is UnitCardData)
        {
            if (!ManaSystem.CanPlayCard(this, player, cardFromHand))
            {
                Log($"Not enouugh mana to play {cardFromHand.Name}");
                return;
            }
            var validTargets = _targetSystem.GetValidTargets(this, player, cardFromHand);

            var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

            if (targetAsEntity != null)
            {
                ManaSystem.SpendMana(this, player, cardFromHand.ConvertedManaCost);
                ResolvingSystem.Add(this, cardFromHand, targetAsEntity);
                _stateBasedEffectSystem.CheckStateBasedEffects(this);
            }
            else
            {
                Log("Invalid Lane chosen for summoning unit");
                return;
            }
        }
        else if (cardFromHand.CurrentCardData is SpellCardData)
        {
            if (!_targetSystem.SpellNeedsTargets(this, player, cardFromHand) && ManaSystem.CanPlayCard(this, player, cardFromHand))
            {
                ManaSystem.SpendMana(this, player, cardFromHand.ConvertedManaCost);
                ResolvingSystem.Add(this, cardFromHand, null);
                _stateBasedEffectSystem.CheckStateBasedEffects(this);
            }
            else if (ManaSystem.CanPlayCard(this, player, cardFromHand))
            {
                var validTargets = _targetSystem.GetValidTargets(this, player, cardFromHand);

                var targetAsEntity = validTargets.FirstOrDefault(tar => tar.EntityId == targetId);

                if (targetAsEntity != null)
                {
                    ManaSystem.SpendMana(this, player, cardFromHand.ConvertedManaCost);
                    ResolvingSystem.Add(this, cardFromHand, targetAsEntity);
                    _stateBasedEffectSystem.CheckStateBasedEffects(this);
                }
            }
            else
            {
                Log($"Not enough mana to play {cardFromHand.Name}");
            }
        }
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

        return zones;
    }

    public List<T> GetEntities<T>() where T : CardGameEntity
    {
        return _registeredEntities.Where(e => e is T).Cast<T>().ToList();
    }
    public List<CardInstance> GetUnitsInPlay()
    {

        var player1Units = Player1.Lanes.Select(lane => lane.UnitInLane).Where(unit => unit != null);
        var player2Units = Player2.Lanes.Select(lane => lane.UnitInLane).Where(unit => unit != null);

        return player1Units.Concat(player2Units).ToList();
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
        var cards = db.GetAll().Where(c => c.Name == "Black Lotus").ToList();

        var rng = new Random();

        for (int i = 0; i < _startingHandSize; i++)
        {
            AddCardToGame(player, cards.Randomize().ToList()[0], player.Hand);
        }
    }

    void AddRandomCardsToDeck()
    {
        var cardDB = new CardDatabase();
        var cards = cardDB.GetAll().ToList();

        for (int i = 0; i < 60; i++)
        {
            AddCardToGame(Player1, cards.Randomize().ToList()[0], Player1.Deck);
        }

        for (int i = 0; i < 60; i++)
        {
            AddCardToGame(Player2, cards.Randomize().ToList()[0], Player2.Deck);
        }
    }
}
