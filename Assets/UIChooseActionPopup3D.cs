using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIChooseActionPopup3D : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]

    //TODO - Build a UI Action Representation Object
    private Card3D _choice1;
    [SerializeField]
    private Card3D _choice2;

    private List<CardGameAction> _actions;
    public void SetCard(List<CardGameAction> actions)
    {
        _actions = actions;
        SetChoices();
        SetTitle();
    }

    void SetChoices()
    {
        if (_actions.IsNullOrEmpty())
        {
            return;
        }
        _choice1.SetCardInfo(_actions[0]);
        _choice2.SetCardInfo(_actions[1]);
    }

    void SetTitle()
    {
        if (_actions.IsNullOrEmpty())
        {
            return;
        }
        _title.text = $"Choose your action";
    }
}
