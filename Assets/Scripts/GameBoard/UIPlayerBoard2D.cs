using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[ExecuteInEditMode]
public class UIPlayerBoard2D : UIPlayerBoard
{
    [SerializeField]
    private UIPlayerAvatar2D _avatar;

    [SerializeField]
    private Transform _lanes;
    [SerializeField]
    private CardsViewer2D _hand;

    [SerializeField]
    private CardsViewer2D _items;

    [SerializeField]
    private UICard2D _deckOnUI; //only shows the top card

    [SerializeField]
    private UICard2D _graveyardOnUI; //only show the top card

    [SerializeField]
    private Player _player;

    public UICard2D DeckOnUI { get => _deckOnUI; set => _deckOnUI = value; }
    public UICard2D GraveyardOnUI { get => _graveyardOnUI; set => _graveyardOnUI = value; }

    void Start()
    {
        GraveyardOnUI.GetComponent<UIGameEntity>().OnClickHandler = new System.Action<UIGameControllerClickEvent>(data =>
        {
            Debug.Log("Is the graveyard being handled correctly?");
            UIGameController.Instance.HandleViewGraveyardClick(_player);
        });
    }

    public override List<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();
        entities.AddRange(this.GetComponentsInChildren<UIGameEntity>(true));
        return entities;
    }

    public override void SetBoard(Player player)
    {
        if (player == null) return;

        _player = player;

        InitUIEntityIds(player);
        UpdateLanes(player);
        UpdateHand(player);
        UpdateMana(player);
        _avatar.SetHealth(player.Health);

        if (player.DiscardPile.Cards.Any())
        {
            GraveyardOnUI.gameObject.SetActive(true);
            GraveyardOnUI.SetCardData(player.DiscardPile.Cards.Last());
        }
        else
        {
            GraveyardOnUI.gameObject.SetActive(false);
        }

        _items.SetCards(player.Items.Cards, "");

        if (player.Deck.Cards.Any())
        {
            DeckOnUI.gameObject.SetActive(true);
            DeckOnUI.SetCardData(player.Deck.Cards.Last());
        }
        else
        {
            DeckOnUI.gameObject.SetActive(false);
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
        _hand.SetCards(player.Hand, "", false, HideHiddenInfo);
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
            var uiGameEntity = uiLanes[i].GetComponent<UIGameEntity>();
            if (uiGameEntity != null)
            {
                if (player.Lanes == null || player.Lanes[i] == null)
                {
                    var j = 0; //test breakpoint;
                }
                uiGameEntity.EntityId = player.Lanes[i].EntityId;
            }
        }

        //Initialize the Player Avatars entity ids
        _avatar.EntityId = player.EntityId;
    }


}
