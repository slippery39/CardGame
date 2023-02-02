using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard3D : MonoBehaviour
{
    [SerializeField]
    private Hand3D _hand;
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

        //Setting a players hand.
        _hand.numberOfCards = player.Hand.Cards.Count;
        _hand.UpdateCards();
        var card3Ds = _hand.GetCards();

        for (var i = 0; i < player.Hand.Cards.Count; i++)
        {
            card3Ds[i].SetCardInfo(player.Hand.Cards[i]);
        }       
    }
}
