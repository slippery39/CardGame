using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mana3D : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void SetManaAmount(int current, int total)
    {
        //We want to automatically hide the mana icon if it doesn't exist in the mana pool.
        if (current == 0 && total == 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);
        text.text = $"{current}/{total}";
    }
}
