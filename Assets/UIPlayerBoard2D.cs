using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class UIPlayerBoard2D : UIPlayerBoard
{
    [SerializeField]
    private UIPlayerAvatar2D _avatar;

    [SerializeField]
    private Transform _lanes;
    [SerializeField]
    private ZoneViewer2D _hand;

    [SerializeField]
    private ZoneViewer2D _items;

    [SerializeField]
    private UICard2D _deckOnUI; //only shows the top card

    [SerializeField]
    private UICard2D _graveyardOnUI; //only show the top card

    [SerializeField]
    private Player _player;

    public override List<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();
        entities.AddRange(this.GetComponentsInChildren<UIGameEntity>(true));
        return entities;
    }

    public override void SetPlayer(Player player)
    {
        _player = player;
        InitUIEntityIds(_player);
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null) return;
        SetBoard(_player);
    }

    private void SetBoard(Player player)
    {
        if (player == null) return;

        InitUIEntityIds(player);
        UpdateLanes(player);
        UpdateHand(player);
        UpdateMana(player);
        _avatar.SetHealth(player.Health);

        if (player.DiscardPile.Cards.Any())
        {
            _graveyardOnUI.gameObject.SetActive(true);
            _graveyardOnUI.SetCardData(player.DiscardPile.Cards.Last());
        }
        else
        {
            _graveyardOnUI.gameObject.SetActive(false);
        }

        _items.SetZone(player.Items);

        if (player.Deck.Cards.Any())
        {
            _deckOnUI.gameObject.SetActive(true);
            _deckOnUI.SetCardData(player.Deck.Cards.Last());
        }
        else
        {
            _deckOnUI.gameObject.SetActive(false);
        }
    }

    private void UpdateLanes(Player player)
    {
        var uiLanes = _lanes.GetComponentsInChildren<UILane2D>(true);
        var gameLanes = player.Lanes;
        for (int i = 0; i < gameLanes.Count; i++)
        {
            //If there is no card in the game state for a lane, just hide the card.
            if (gameLanes[i].IsEmpty())
            {
                uiLanes[i].SetEmpty();
                continue;
            }

            uiLanes[i].SetCard(gameLanes[i].UnitInLane);
        }
    }

    private void UpdateHand(Player player)
    {
        var uiCards = _hand.GetComponentsInChildren<UICard2D>(true);
        var hand = player.Hand;
        for (int i = 0; i < uiCards.Length; i++)
        {
            var uiCard = uiCards[i];
            //If there is no card in the game state for a lane, just hide the card.
            if (hand.Cards.Count <= i || hand.Cards[i] == null)
            {
                uiCard.gameObject.SetActive(false);
                continue;
            }
            else
            {
                uiCard.gameObject.SetActive(true);
            }
            if (HideHiddenInfo)
            {
                uiCard.SetAsUnknownCard();
            }
            else
            {
                uiCard.SetCardData(hand.Cards[i]);
            }
        }
    }
    private void UpdateMana(Player player)
    {
        _avatar.SetMana(player.ManaPool);
    }

    private void InitUIEntityIds(Player player)
    {
        //Map the lanes on the board to lanes in the game.
        var uiLanes = _lanes.GetComponentsInChildren<UILane2D>(true);
        for (int i = 0; i < uiLanes.Length; i++)
        {
            uiLanes[i].EntityId = player.Lanes[i].EntityId;
        }

        //Initialize the Player Avatars entity ids
        _avatar.EntityId = player.EntityId;
    }


}
