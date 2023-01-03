using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIChooseActionPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]

    //TODO - Build a UI Action Representation Object
    private UIActionChoice _choice1;
    [SerializeField]
    private UIActionChoice _choice2;

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

        //we need a UI element for showing a possible action.

        _choice1.SetAction(_actions[0]);
        _choice2.SetAction(_actions[1]);
    }

    void SetTitle()
    {
        if (_actions.IsNullOrEmpty())
        {
            return;
        }
        _title.text = $"Choose the action you would like to perform : ";
    }
}
