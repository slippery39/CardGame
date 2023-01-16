using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{

    public bool enabled = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrop(GameObject gameObject)
    {
        var onDropHandler = this.GetComponent<IOnDropHandler>();

        if (onDropHandler != null)
        {
        }
    }
}
