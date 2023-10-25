using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField]
    private UI3DController _uiController;

    [SerializeField]
    private float _pushAmount = 0.1f;

    private Vector3 _initiallocalPosition;

    public void OnMouseUpAsButton()
    {
        Debug.Log("Mouse click!");
        _uiController.EndTurn();
        transform.localPosition = _initiallocalPosition;
    }

    public void OnMouseUp()
    {
        transform.localPosition = _initiallocalPosition;
    }


    public void OnMouseDown()
    {
        this.transform.localPosition += Vector3.down * _pushAmount;
    }

    private void Awake()
    {
        _initiallocalPosition = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
