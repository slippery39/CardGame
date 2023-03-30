using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardViewerModal3D : MonoBehaviour
{
    [SerializeField]
    Camera _camera;

    [SerializeField]
    private Scrollbar _scrollbar;

    [SerializeField]
    private GameObject _container;

    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    Transform _visibleBoundsObject;

    [SerializeField]
    bool _showSlider = true;

    [SerializeField] //TODO - do we need to store local bounds and world space bounds?
    Bounds _containerBounds;

    [SerializeField]
    float _paddingLeftRight = 0.5f; //only applies when the slider is showing.

    [SerializeField]
    float _cardSpacing = 1f;

    [SerializeField]
    TextMeshProUGUI title;

    [Header("Card 3D Settings")]
    [SerializeField] private Card3D _cardPrefab;
    private List<Card3D> _instantiatedCards;

    UI3DController _uiController; 


    public void Initialize(UI3DController controller)
    {
        this._uiController = controller;
        this._cancelButton.onClick.RemoveAllListeners();
        this._cancelButton.onClick.AddListener(() => this._uiController.CloseChoiceWindow());
    }

    void Awake()
    {
        _camera = this.GetComponent<Camera>();
    }

    private void Start()
    {
        DoCardLayout();
    }

    void Update()
    {
        DoCardLayout();
        //BUG - we need to determine the proper order for all of this to happen so that everything updates in one frame.
        _containerBounds = CalculateLocalBounds(_container);
        _showSlider = !IsInsideCameraViewport(_containerBounds);
        _scrollbar.gameObject.SetActive(_showSlider);

        var leftPoint = _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        var rightPoint = _camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, _camera.nearClipPlane));
        var middlePoint = _camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, _camera.nearClipPlane));

        //Bounding box is on the screen.
        if (_showSlider == false)
        {
            _container.transform.position = new Vector3(middlePoint.x - (_containerBounds.center.x), middlePoint.y - (_containerBounds.center.y), _container.transform.position.z);
        }
        //Bounding box is off the screen.
        else
        {
            _container.transform.position = new Vector3(leftPoint.x + _paddingLeftRight, middlePoint.y - (_containerBounds.center.y), _container.transform.position.z);
            //Calculates the position of the container based on the slider value.
            var sliderVal = _scrollbar.value;
            _container.transform.position -= new Vector3(Mathf.Lerp(0, _containerBounds.max.x - _paddingLeftRight -( (rightPoint.x-leftPoint.x)/2), sliderVal), 0, 0);
        }

        var cameraPointDiff = rightPoint - leftPoint;
        var knobSizePerc = Mathf.Clamp(cameraPointDiff.x / _containerBounds.size.x, 0.1f, 0.8f);
        _scrollbar.size = knobSizePerc;
        
        //SLIDER Knob Size
        /*
         * Slider knob size should depend on the % of the container is visible at any one time
         * based on _containerBounds.size and _camera.ScreenToWorldPoint
         */
    }
   
    private void DoCardLayout()
    {
        var cards = _container.GetComponentsInChildren<Card3D>();

        for (var i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            var xPos = (i * (card.GetComponent<Collider>().bounds.size.x + _cardSpacing));
            card.transform.localPosition = new Vector3(xPos, 0, 0); 
        }
    }

    public void Show(List<ICard> cards, string title, bool showCancel = true)
    {
        this.gameObject.SetActive(true);
        SetCards(cards);
        SetTitle(title);

        if (showCancel)
        {
            _cancelButton.gameObject.SetActive(true);
        }
        else
        {
            _cancelButton.gameObject.SetActive(false);

        }

        //TODO - need a title still
    }

    public void SetTitle(string title)
    {
        this.title.text = title;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void SetCards(List<ICard> cards)
    {
        var alreadyInitializedCards = _container.GetComponentsInChildren<Card3D>(true);
        _instantiatedCards = alreadyInitializedCards.ToList();

        for (var i = 0; i < cards.Count; i++)
        {
            Card3D card;
            if (_instantiatedCards.Count <= i)
            {
                card = Instantiate(_cardPrefab);
                _instantiatedCards.Add(card);

            }
            else
            {
                card = _instantiatedCards[i];
                card.gameObject.SetActive(true);
            }
            card.SetCardInfo(cards[i] as CardInstance);
            card.transform.SetParent(_container.transform, false);
        }

        //Hide any additional cards
        for (var i = cards.Count; i < _instantiatedCards.Count; i++)
        {
            _instantiatedCards[i].gameObject.SetActive(false);
        }
    }

    private bool IsInsideCameraViewport(Bounds bounds)
    {
        //BUG - bounds.min and bounds.max are in local space, not world space
        Vector3 minPosition = _camera.WorldToViewportPoint(_container.transform.position + bounds.min);
        Vector3 maxPosition = _camera.WorldToViewportPoint(_container.transform.position + bounds.max);

        if (minPosition.x < 0 || maxPosition.x > 1)
        {
            return false;
        }

        return true;
    }

    private Bounds CalculateLocalBounds(GameObject gameObject, bool includeParentTransform = false)
    {
        Quaternion currentRotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(gameObject.transform.position, Vector3.zero);
        }

        //If the include parent transform flag is set, we want to include the parents transform as part of the bounds
        //This may not be what is intended if the parent is for example an empty game object and doesn't actually show anything on screen
        Bounds bounds;
        if (includeParentTransform)
        {
            bounds = new Bounds(gameObject.transform.position, Vector3.zero);
        }
        else
        {
            bounds = new Bounds(renderers[0].gameObject.transform.position, Vector3.zero);
        }

        //Loop through each renderer accumulating each renderers bounds.
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }


        Vector3 localCenter = bounds.center - gameObject.transform.position;
        bounds.center = localCenter;

        _visibleBoundsObject.transform.localPosition = gameObject.transform.localPosition + bounds.center;
        _visibleBoundsObject.transform.localScale = bounds.size;


        gameObject.transform.rotation = currentRotation;

        return bounds;
    }
}
