using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeveloperPanel : MonoBehaviour
{
    [SerializeField]
    private UI3DController _ui3DController;

    [SerializeField]
    private TextMeshProUGUI _uiStateText;

    public void Initialize(UI3DController uI3DController)
    {
        _ui3DController = uI3DController;
    }

    public void Update()
    {
        if (_ui3DController == null)
        {
            return;
        }

        var state = _ui3DController.GameUIStateMachine.CurrentState;
        var stateName = state?.GetType().Name;

        _uiStateText.text = "UI State : " + stateName;
    }
}
