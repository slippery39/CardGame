using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public bool isEnabled = true;
    void OnDrop(GameObject gameObject)
    {
        var onDropHandler = this.GetComponent<IOnDropHandler>();

        if (onDropHandler != null)
        {
        }
    }
}
