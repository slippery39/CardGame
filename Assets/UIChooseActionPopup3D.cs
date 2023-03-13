using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChooseActionPopup3D : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private Card3D _choice1;
    [SerializeField]
    private Card3D _choice2;

    [SerializeField]
    private Button _cancelButton;

    private List<CardGameAction> _actions;

    public void Initialize(UI3DController _ui3DController)
    {
        _cancelButton.onClick.AddListener(() =>
        {
            _ui3DController.GameUIStateMachine.ToIdle();
        });
    }
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

        var h = 0;
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
