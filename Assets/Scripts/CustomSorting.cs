using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomSorting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //note the typo here.
        this.GetComponent<Renderer>().sortingLayerName = "CustomUIOverLay";
        this.GetComponent<Renderer>().sortingOrder = 9999;
    }
}
