using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerAvatar : UIGameEntity
{
    [SerializeField]
    private TextMeshPro _healthText;

    [SerializeField]
    private TextMeshPro _manaText;

    [SerializeField]
    private SpriteRenderer _highlight;

    public void SetMana(int amount)
    {
        _manaText.text = $@"Mana : {amount}";
    }

    public void SetHealth (int amount)
    {
        _healthText.text = amount.ToString();
    }

    public override void Highlight()
    {
        _highlight.gameObject.SetActive(true);
    }

    public override void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
