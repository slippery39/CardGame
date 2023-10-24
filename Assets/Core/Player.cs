﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player : CardGameEntity, IDeepCloneable<Player>
{
    #region Private Fields
    private int _playerId;
    private int _health;
    private List<Lane> _lanes;
    private Hand _hand;
    private DiscardPile _discardPile;
    private Deck _deck;
    private Zone _exile;
    private IZone _items;
    private ManaPool _manaPool;

    private string _name;
    #endregion

    //Used for Deserializing.
    //[JsonConstructor]
    public Player()
    {

    }


    public Player(int numberOfLanes)
    {
        _manaPool = new ManaPool();
        Hand = new Hand();
        Lanes = new List<Lane>();
        DiscardPile = new DiscardPile();
        Deck = new Deck();
        Exile = new Zone(ZoneType.Exile, "Exile");
        Items = new Zone(ZoneType.InPlay, "Items");
        InitLanes(numberOfLanes);
    }

    #region Public Properties
    public int PlayerId { get => _playerId; set => _playerId = value; }
    public int Health { get => _health; set => _health = value; }
    public List<Lane> Lanes { get => _lanes; set => _lanes = value; }
    public Hand Hand { get => _hand; set => _hand = value; }
    public DiscardPile DiscardPile { get => _discardPile; set => _discardPile = value; }
    public Deck Deck { get => _deck; set => _deck = value; }
    //public int Mana { get => _mana; set => _mana = value; }
    public Zone Exile { get => _exile; set => _exile = value; }
    public int ManaPlayedThisTurn { get; set; } = 0;
    public int TotalManaThatCanBePlayedThisTurn
    {
        get
        {
            var manaThatCanBePlayed = 1;
            var manaPlayedModifications = Modifications.GetOfType<IModifyManaPerTurn>();

            foreach (var mod in manaPlayedModifications)
            {
                manaThatCanBePlayed = mod.ModifyManaPerTurn(null, this, manaThatCanBePlayed);
            }
            return manaThatCanBePlayed;
        }
    }

    public ManaPool ManaPool { get => _manaPool; set { _manaPool = value; } }
    public override string Name { get => $@"Player {PlayerId}"; set { _name = value; } }

    public IZone Items { get => _items; set => _items = value; }
    public bool IsLoser { get; set; } = false;
    public bool DrawnCardWithNoDeck { get; internal set; } = false;

    #endregion

    #region Public Methods

    public Player DeepClone(CardGame cardGame)
    {
        var clone = (Player)MemberwiseClone();

        clone.ContinuousEffects = ContinuousEffects.Clone();
        //The Clone Method is dangerous and may not be working in the correct way... somehow it turned my snapcaster mage modification into a null..
        //not sure why...keep an eye out for any other weird errors regarding this clone object.
        //maybe do a sanity check on all lists checking for nulls?
        clone.Modifications = Modifications.Select(m => m.Clone()).ToList();

        clone.EntityId = EntityId;
        clone.Name = Name;

        clone.Lanes = Lanes.DeepClone(cardGame).ToList();
        clone.Lanes.ForEach(x =>
        {
            if (x.UnitInLane != null)
            {
                x.UnitInLane.CurrentZone = x;
            }
        });

        clone.Hand = Hand.Clone();
        clone.Hand.Cards = Hand.Cards.DeepClone(cardGame).ToList();
        clone.Hand.Cards.ForEach(card => card.CurrentZone = clone.Hand);

        clone.DiscardPile = DiscardPile.Clone();
        clone.DiscardPile.Cards = DiscardPile.Cards.DeepClone(cardGame).ToList();
        clone.DiscardPile.Cards.ForEach(card => card.CurrentZone = clone.DiscardPile);

        clone.Deck = Deck.Clone();
        clone.Deck.Cards = Deck.Cards.DeepClone(cardGame).ToList();
        clone.Deck.Cards.ForEach(card => card.CurrentZone = clone.Deck);

        clone.Exile = Exile.Clone();
        clone.Exile.Cards = Exile.Cards.DeepClone(cardGame).ToList();
        clone.Exile.Cards.ForEach(card => card.CurrentZone = clone.Exile);


        clone.Items = new Zone(ZoneType.InPlay, "Items");
        clone.Items.Cards.AddRange(Items.Cards.DeepClone(cardGame).ToList());
        clone.Items.Cards.ForEach(card => card.CurrentZone = clone.Items);

        //TODO - do an actual clone here.
        ManaPool = ManaPool.DeepClone();
        //Need to clone the mana pool
        //Need to clone all the cards inside.

        return clone;
    }

    public List<CardGameEntity> GetAllEntities()
    {
        var entities = Lanes.Cast<CardGameEntity>()
              .Concat(GetCardsInPlay())
              .Concat(Hand)
              .Concat(DiscardPile)
              .Concat(Deck)
              .Concat(Exile)
              .Cast<CardGameEntity>().ToList();

        var backEntities = new List<CardGameEntity>();

        //Edge case for back cards
        var cardInstances = entities.GetOfType<CardInstance>().ToList();
        foreach (var cardInstance in cardInstances)
        {
            if (cardInstance.BackCard != null)
            {
                entities.Add(cardInstance.BackCard);
            }
        }

        return entities.Concat(backEntities).ToList();
    }

    public List<CardInstance> GetUnitsInPlay()
    {
        return Lanes.Where(l => l.IsEmpty() == false).Select(l => l.UnitInLane).ToList();
    }

    public List<CardInstance> GetCardsInPlay()
    {
        var cardsInPlay = GetUnitsInPlay();
        cardsInPlay.AddRange(Items.Cards);
        return cardsInPlay;
    }

    public List<CardGameAction> GetAvailableActions(CardGame cardGame)
    {
        var handActions = Hand.Cards.SelectMany(c => c.GetAvailableActionsWithTargets());
        //TODO - should grab all actions with targets
        var inPlayActions = GetCardsInPlay().SelectMany(c => c.GetAvailableActionsWithTargets()).ToList();

        var discardPileActions = DiscardPile.Cards.SelectMany(c => c.GetAvailableActionsWithTargets()).ToList();

        var fightActions = Lanes.Where(l => cardGame.BattleSystem.CanBattle(l.LaneIndex)).Select(l =>
        {
            return new FightAction
            {
                Player = this,
                LaneIndex = l.LaneIndex
            };
        });

        var actions = handActions.Concat(inPlayActions).Concat(fightActions).ToList();

        foreach (var action in actions)
        {
            if ((action is PlayManaAction || action is PlayUnitAction || action is PlaySpellAction) && (action.CardToPlay == null))
            {
                //Debug.Log("Found an in valid action!");
            }
        }

        var endOfTurnAction = new List<CardGameAction>() { new NextTurnAction() };


        return handActions.Concat(inPlayActions).Concat(fightActions).Concat(discardPileActions).Concat(endOfTurnAction).ToList();
    }

    #endregion

    #region Private Methods
    void InitLanes(int numberOfLanes)
    {
        for (int i = 0; i < numberOfLanes; i++)
        {
            Lanes.Add(new Lane()
            {
                LaneId = (i + 1),
                LaneIndex = i
            });
        }
    }

    public List<Lane> GetEmptyLanes()
    {
        return _lanes.Where(l => l.IsEmpty()).ToList();
    }
    #endregion
}

