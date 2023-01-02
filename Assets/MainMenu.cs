using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainScreen;
    [SerializeField]
    private GameObject _deckSelection;
    [SerializeField]
    private GameObject _simulation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToDeckSelection()
    {
        _deckSelection.SetActive(true);
        _simulation.SetActive(false);
        _mainScreen.SetActive(false);
    }

    public void GoToSimulation()
    {
        _simulation.SetActive(true);
        _deckSelection.SetActive(false);
        _mainScreen.SetActive(false);
    }
}
