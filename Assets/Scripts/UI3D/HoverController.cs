using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField] private Canvas hoverCanvas;

    public static HoverController instance;

    void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Initializing Hovercontroller singleton");
            instance = this;
        }
    }
    public void ShowCardTooltip(Card3D originalCard)
    {
        hoverCanvas.gameObject.SetActive(true);
    }

    public void HideCardTooltip()
    {
        hoverCanvas.gameObject.SetActive(false);
    }
}
