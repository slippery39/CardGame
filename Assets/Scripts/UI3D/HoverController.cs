using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField] private Canvas hoverCanvas;
    [SerializeField] private Card3D hoverCard;

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
        Debug.Log("Show card tool tip" + originalCard.name);
        hoverCanvas.gameObject.SetActive(true);

        //Set our hover cards model to be a clone of the original one that is calling this.
        var clonedCard3D = Instantiate<Card3D>(originalCard);
        //We don't want the hover card casting any shadows
        clonedCard3D.CardMesh.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        hoverCard.GetComponent<Card3D>().SetCardModel(clonedCard3D.GetCardModel());
    }

    public void HideCardTooltip()
    {
        hoverCanvas.gameObject.SetActive(false);
    }
}
