using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerAvatar2D : UIGameEntity
{

    [SerializeField]
    private TextMeshProUGUI _healthText;

    [SerializeField]
    private TextMeshProUGUI _manaText;

    [SerializeField]
    private Image _highlight;

    public void SetMana(ManaPool manaPool)
    {
        var colorlessMana = manaPool.CurrentColorlessMana;
        var colorsCount = manaPool.CurrentColoredMana;


        string text = "";

        if (manaPool.TotalColorlessMana > 0) text += $@"Colorless : {colorlessMana} / {manaPool.TotalColorlessMana}";

        foreach (var manaType in colorsCount.Keys)
        {
            if (manaPool.TotalColoredMana[manaType] == 0) continue;

            text += $@"{Environment.NewLine} {manaType.ToString()}:{colorsCount[manaType]} / {manaPool.TotalColoredMana[manaType]}";
        }

        _manaText.text = text;
    }

    public void SetHealth(int amount)
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
