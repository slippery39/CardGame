using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard3D : MonoBehaviour
{
    [SerializeField]
    private Hand3D _hand;
    [SerializeField]
    private Deck3D _deck;
    [SerializeField]
    private Graveyard3D _graveyard;
    [SerializeField]
    private Lanes3D _lanes;
    [SerializeField]
    private Items3D _items;
    [SerializeField]
    private ManaPool3D _manaPool;

    [SerializeField]
    private Exile3D _exile;

    [SerializeField]
    private Avatar3D _avatar;
    [SerializeField]
    private int _playerId;

    public int PlayerId { get => _playerId; }
    public Hand3D Hand { get => _hand;}
    public Graveyard3D Graveyard { get => _graveyard; }
    public Lanes3D Lanes { get => _lanes;}
    public Items3D Items { get => _items; }
    public ManaPool3D ManaPool { get => _manaPool;  }
    public Avatar3D Avatar { get => _avatar; }
    public Deck3D Deck { get => _deck; set => _deck = value; }

    public void SetBoard(Player player)
    {
        Debug.Log("Setting board 3d for player : " + player.Name);

        _playerId = player.PlayerId;
        //Avatar
        SetAvatar(player);
        //Hand
        SetHand(player.Hand.Cards);
        //Deck
        SetDeck(player.Deck.Cards);
        //Graveyard
        SetDiscardPile(player.DiscardPile.Cards);
        //Lanes
        SetLanes(player.Lanes);
        //Items
        SetItems(player.Items.Cards);
        //Deck
        //Life Total
        SetLifeTotal(player.Health);
        //Mana Counts
        SetManaPool(player.ManaPool);

        SetExile(player.Exile.Cards);

    }

    //TODO - place this in the avatar component itself.
    private void SetAvatar(Player player)
    {
        var entity = _avatar.GetComponent<UIGameEntity3D>();
        if (entity == null)
        {
            entity = _avatar.gameObject.AddComponent<UIGameEntity3D>();
        }
        entity.EntityId = player.EntityId;
    }

    private void SetExile(List<CardInstance> cards)
    {
        _exile.numberOfCards = cards.Count;
        _exile.UpdateCards();


        var card3Ds = _exile.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
            UIGameEntity3D.AddToCard3D(card3Ds[i], cards[i]);
        }
    }

    private void SetDeck(List<CardInstance> cards)
    {
        _deck._numberOfCards = cards.Count;
        _deck.UpdateCards();

        var card3Ds = _deck.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
            UIGameEntity3D.AddToCard3D(card3Ds[i], cards[i]);
        }
    }
    //TODO - use ICard instead?
    private void SetHand(List<CardInstance> cards)
    {
        //Setting a players hand.
        _hand.numberOfCards = cards.Count;
        _hand.UpdateCards();
        var card3Ds = _hand.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
            UIGameEntity3D.AddToCard3D(card3Ds[i], cards[i]);
        }
    }

    private void SetDiscardPile(List<CardInstance> cards)
    {
        //Setting a players graveyard.
        _graveyard._numberOfCards = cards.Count;
        _graveyard.UpdateCards();
        var card3Ds = _graveyard.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
            UIGameEntity3D.AddToCard3D(card3Ds[i], cards[i]);
        }
    }

    private void SetLanes(List<Lane> lanes)
    {
        var lane3Ds = _lanes.GetLanes();
        for (var i = 0; i < lanes.Count; i++)
        {
            UIGameEntity3D.AddToLane(lane3Ds[i], lanes[i]);
            lane3Ds[i].SetUnitInLane(lanes[i].UnitInLane);
        }
    }

    private void SetItems(List<CardInstance> cards)
    {
        //Setting a players items
        _items.numberOfCards = cards.Count;
        _items.UpdateCards();
        var card3Ds = _items.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
            UIGameEntity3D.AddToCard3D(card3Ds[i], cards[i]);
        }
    }


    private void SetLifeTotal(int amount)
    {
        _avatar.SetLifeTotal(amount);
    }


    private void SetManaPool(ManaPool pool)
    {
        _manaPool.SetManaPool(pool);
    }
}
