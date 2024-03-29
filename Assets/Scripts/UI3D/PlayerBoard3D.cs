using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBoard3D : MonoBehaviour
{

    [Header("Player Zones")]
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


    [Header("Other Variables")]
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

    [Header("Reference Variables")]
    [SerializeField]
    private UI3DController _uiController;

    public void Initialize(UI3DController _uiController)
    {
        this._uiController = _uiController;
    }

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
        SetDiscardPile(player.DiscardPile.Cards,player);
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
        }
        //Quick fix for issue where the cards aren't properly flipped at the start.
        //This indicates an issue with the way we handle our zones though.
        //We may want to change the way we do things here.
        _deck.UpdateCards();
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
        }
    }

    private void SetDiscardPile(List<CardInstance> cards,Player owner)
    {
        //Setting a players graveyard.
        _graveyard._numberOfCards = cards.Count;
        _graveyard.UpdateCards();
        _graveyard.OnClickHandler = () =>
        {
            _uiController.ViewChoiceWindow(cards, owner.Name + "'s Graveyard");
        };
        
        var card3Ds = _graveyard.GetCards();      

        for (var i = 0; i < cards.Count; i++)
        {           
            card3Ds[i].SetCardInfo(cards[i],true,false);            
        }

        //Highlight the graveyard if a card exists with an action.
        //Might be better to have an actual highlight exist around the graveyard instead 
        //of piggy backing on existing cards
        bool hasAction = cards.Exists(c => c.GetAvailableActions().Any());
        if (hasAction)
        {
            card3Ds[0].Highlight();
        }
        else
        {
            card3Ds[0].StopHighlight();
        }
    }

    private void SetLanes(List<Lane> lanes)
    {
        var lane3Ds = _lanes.GetLanes();
        var playerId = _uiController.PlayerId;
        var activePlayerId = _uiController.CurrentUICardGame.ActivePlayerId;

        for (var i = 0; i < lanes.Count; i++)
        {
            UIGameEntity3D.AddToLane(lane3Ds[i], lanes[i]);
            lane3Ds[i].SetUnitInLane(lanes[i].UnitInLane,lanes[i].CanBattle() && playerId == activePlayerId);
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
