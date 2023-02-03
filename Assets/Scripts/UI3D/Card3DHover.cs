using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card3DHover : MonoBehaviour
{
    void OnMouseEnter()
    {
        HoverController.instance.ShowCardTooltip(this.GetComponent<Card3D>());
    }

    void OnMouseExit()
    {
        HoverController.instance.HideCardTooltip();
    }
}
