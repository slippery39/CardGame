using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerBoard : MonoBehaviour
{
    [SerializeField]
    private UIPlayerAvatar _avatar;
    [SerializeField]
    private Transform _lanes;
    [SerializeField]
    private Transform _hand;
    [SerializeField]
    private ZoneViewer _graveyard;
    [SerializeField]
    private ZoneViewer _items;

    [SerializeField]
    private ZoneViewer _deck;

    [SerializeField]
    private Player _player;

    public void SetPlayer(Player player)
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

    void SetBoard(Player player)
    {
        if (player == null) return;

        InitUIEntityIds(player);
        UpdateLanes(player);
        UpdateHand(player);
        UpdateMana(player);
        _avatar.SetHealth(player.Health);
        _graveyard.SetZone(player.DiscardPile);
        _items.SetZone(player.Items);
        _deck.SetZone(player.Deck, true);
    }

    private void InitUIEntityIds(Player player)
    {
        //Map the lanes on the board to lanes in the game.
        var uiLanes = _lanes.GetComponentsInChildren<UILane>(true);
        for (int i = 0; i < uiLanes.Length; i++)
        {
            uiLanes[i].EntityId = player.Lanes[i].EntityId;
        }

        //Initialize the Player Avatars entity ids
        _avatar.EntityId = player.EntityId;
    }

    public List<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();
        entities.AddRange(this.GetComponentsInChildren<UIGameEntity>(true));
        return entities;
    }

    private void UpdateLanes(Player player)
    {
        var uiLanes = _lanes.GetComponentsInChildren<UILane>(true);
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
        var uiCards = _hand.GetComponentsInChildren<UICard>(true);
        var hand = player.Hand;
        for (int i = 0; i < uiCards.Length; i++)
        {
            //If there is no card in the game state for a lane, just hide the card.
            if (hand.Cards.Count <= i || hand.Cards[i] == null)
            {
                uiCards[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                uiCards[i].gameObject.SetActive(true);
            }
            uiCards[i].GetComponent<UICard>().SetCardData(hand.Cards[i]);
        }
    }

    private void UpdateMana(Player player)
    {
        _avatar.SetMana(player.ManaPool);
    }
}
