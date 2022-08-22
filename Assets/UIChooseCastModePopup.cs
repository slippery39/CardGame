using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIChooseCastModePopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private UICard2D _choice1;
    [SerializeField]
    private UICard2D _choice2;

    private CardInstance _cardWithCastModes;  

    public void SetCard(CardInstance card)
    {
        _cardWithCastModes = card;
        SetChoices();
        SetTitle();
    }

    void SetChoices()
    {
        if (_cardWithCastModes == null)
        {
            return;
        }

        _choice1.SetCardData(_cardWithCastModes);
        _choice2.SetCardData(_cardWithCastModes.BackCard);
    }

    void SetTitle()
    {
        if (_cardWithCastModes == null)
        {
            return;
        }
        _title.text = $"Choose how you would like to play {_cardWithCastModes.Name}";
    }
}
