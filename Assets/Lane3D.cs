using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane3D : MonoBehaviour
{
    [SerializeField] Card3D _card;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUnitInLane(CardInstance card)
    {
        if (card == null)
        {
            _card.gameObject.SetActive(false);
        }
        else
        {
            _card.gameObject.SetActive(true);
            _card.SetCardInfo(card);
        }
    }
}
