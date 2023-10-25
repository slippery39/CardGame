using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStateDescriptionLabel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _label;

    public void SetLabel(string text)
    {
        _label.text = text;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
