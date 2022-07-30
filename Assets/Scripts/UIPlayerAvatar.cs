using System;
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

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetMana(int amount)
    {
        _manaText.text = $@"Mana : {amount}";
    }

    public void Update()
    {
        _manaText.renderer.sortingOrder = _renderer.sortingOrder + 1;
        _healthText.renderer.sortingOrder = _renderer.sortingOrder + 1;
    }

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
