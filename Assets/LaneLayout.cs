using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaneLayout : MonoBehaviour
{
    //Kind of a constant.
    private int _cardsPerRow = 5;

    [SerializeField]
    private float paddingRight = 0.25f;

    [SerializeField]
    private float paddingBottom = 0.3f;

    [SerializeField]
    private float laneScaling = 1;

    void Update()
    {
        var lanes = GetComponentsInChildren<UILane>();


        for (var i = 0; i < lanes.Length; i++)
        {
            var width = lanes[i].GetComponent<Collider>().bounds.size.x;
            var height = lanes[i].GetComponent<Collider>().bounds.size.y;

            var x = (i % _cardsPerRow) * (width + paddingRight);
            var y = Mathf.Floor(i / _cardsPerRow) * (-height - paddingBottom);

            lanes[i].transform.localPosition = new Vector3(x, y, 0f);
            lanes[i].transform.localScale = new Vector3(laneScaling, laneScaling, laneScaling);
        }
    }
}
