using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card3D : MonoBehaviour
{

    [SerializeField] private GameObject _cardMesh;
    [SerializeField] private GameObject _cardModelContainer;
    public GameObject CardMesh { get => _cardMesh; }

    public Bounds GetBounds()
    {
        return _cardMesh.GetComponent<Renderer>().bounds;
    }

    public void SetCardModel(GameObject model)
    {
        Destroy(_cardModelContainer);
        _cardModelContainer = model;
        _cardModelContainer.transform.SetParent(this.transform, false);
    }

    public GameObject GetCardModel()
    {
        return _cardModelContainer;
    }


}
