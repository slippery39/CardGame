using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avatar3D : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro text;

    public void SetLifeTotal(int amount)
    {
        text.text = amount.ToString();
    }
}
