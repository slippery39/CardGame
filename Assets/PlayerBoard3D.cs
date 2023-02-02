using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard3D : MonoBehaviour
{
    [SerializeField]
    private Hand3D _hand;
    [SerializeField]
    private Graveyard3D _graveyard;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBoard(Player player)
    {
        Debug.Log("Setting board 3d for player : " + player.Name);
        //Hand
        SetHand(player.Hand.Cards);
        //Graveyard
        SetDiscardPile(player.DiscardPile.Cards);

        //Items

        //Deck

        //Life Total

        //Mana Counts

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

    private void SetDiscardPile(List<CardInstance> cards)
    {
        //Setting a players graveyard.
        _graveyard._numberOfCards = cards.Count;
        _graveyard.UpdateCards();
        var card3Ds = _graveyard.GetCards();

        for (var i = 0; i < cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(cards[i]);
        }
    }
}
