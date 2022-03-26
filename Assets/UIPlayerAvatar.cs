using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerAvatar : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _healthText;

    [SerializeField]
    private TextMeshPro _manaText;

    public void SetMana(int amount)
    {
        _manaText.text = $@"Mana : {amount}";
    }

    public void SetHealth (int amount)
    {
        _healthText.text = amount.ToString();
    }
}
