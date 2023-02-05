using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaPool3D : MonoBehaviour
{
    [SerializeField] Mana3D _colorlessMana;
    [SerializeField] Mana3D _anyMana;
    [SerializeField] Mana3D _whiteMana;
    [SerializeField] Mana3D _blueMana;
    [SerializeField] Mana3D _blackMana;
    [SerializeField] Mana3D _redMana;
    [SerializeField] Mana3D _greenMana;


    public void SetManaPool(ManaPool pool)
    {
        _colorlessMana.SetManaAmount(pool.CurrentColorlessMana, pool.TotalColorlessMana);
        _anyMana.SetManaAmount(pool.CurrentColoredMana[ManaType.Any], pool.TotalColoredMana[ManaType.Any]);

        _whiteMana.SetManaAmount(pool.CurrentColoredMana[ManaType.White], pool.TotalColoredMana[ManaType.White]);
        _blueMana.SetManaAmount(pool.CurrentColoredMana[ManaType.Blue], pool.TotalColoredMana[ManaType.Blue]);
        _blackMana.SetManaAmount(pool.CurrentColoredMana[ManaType.Black], pool.TotalColoredMana[ManaType.Black]);
        _redMana.SetManaAmount(pool.CurrentColoredMana[ManaType.Red], pool.TotalColoredMana[ManaType.Red]);
        _greenMana.SetManaAmount(pool.CurrentColoredMana[ManaType.Green], pool.TotalColoredMana[ManaType.Green]);
    }

}
