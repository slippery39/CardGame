using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIActionChoice3D : MonoBehaviour
{
    [SerializeField]
    private int _choiceIndex;
    public void OnMouseUpAsButton()
    {
        Debug.Log($"UI Cast Mode Choice Clicked for : {_choiceIndex}");
        UI3DController.Instance.HandleCastChoice(_choiceIndex);
    }
}
