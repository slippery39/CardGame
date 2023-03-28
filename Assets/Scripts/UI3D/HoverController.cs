using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField] private Canvas hoverCanvas;
    [SerializeField] private Card3D hoverCard;

    //Custom enabled flag
    private bool _enabled = true;

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
        if (!_enabled)
        {
            return;
        }
        //Unknowm cards should not have a hover over.
        var gameEntity = originalCard.GetComponent<UIGameEntity3D>();
        if (gameEntity && gameEntity.EntityId == -1)
        {
            return;
        }
        hoverCanvas.gameObject.SetActive(true);

        if (originalCard.GetComponent<UIGameEntity3D>()!=null)

        hoverCard.SetCardInfo(originalCard, false);
    }

    public void HideCardTooltip()
    {
        hoverCanvas.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this._enabled = true;
    }

    public void Disable()
    {        
        HideCardTooltip();
        this._enabled = false;
    }
}
