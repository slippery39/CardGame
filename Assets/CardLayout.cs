using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CardLayout : MonoBehaviour
{
    private int _maxRows = 1;
    private int _maxColumns = 10;

    [SerializeField]
    private float paddingRight = 0.25f;

    [SerializeField]
    private float cardScaling = 1;


    void Start()
    {

    }

    // Update is called once per frame


    void Update()
    {
        var cards = GetComponentsInChildren<UICard>();

        for (var i = 0; i < cards.Length; i++)
        {
            var width = cards[i].GetComponent<Collider>().bounds.size.x;
            cards[i].transform.localPosition = new Vector3(i * (width + paddingRight), 0f, 0f);
            cards[i].transform.localScale = new Vector3(cardScaling, cardScaling, cardScaling);
        }
    }
}
