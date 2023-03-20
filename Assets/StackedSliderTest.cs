using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StackedSliderTest : MonoBehaviour
{

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private GameObject _container;

    [SerializeField]
    float minX = float.MaxValue;
    [SerializeField]
    float maxX = float.MinValue;

    [SerializeField]
    float maxXReal;

    [SerializeField]
    bool _showSlider = true;


    [SerializeField]
    Vector3 cameraMin;

    [SerializeField]
    Vector3 cameraMax;

    [SerializeField]
    Vector3 cameraDiff;

    [SerializeField]
    Bounds _containerBounds;

    [SerializeField]
    Transform _visibleBoundsObject;

    [SerializeField]
    Camera camera;



    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        cameraDiff = cameraMax - cameraMin;


        float paddingX = 2.5f;

        var gameObjects = _container.GetComponentsInChildren<Transform>();

        minX = float.MaxValue;
        maxX = float.MinValue;

        _containerBounds = CalculateLocalBounds(_container);
    }



    // Update is called once per frame
    void Update()
    {

        //BUG - we need to determine the proper order for all of this to happen so that everything updates in one frame.

        Bounds _containerBounds = CalculateLocalBounds(_container);
        _showSlider = !IsInsideCameraViewport(_containerBounds);
        _slider.gameObject.SetActive(_showSlider);
        var middlePoint = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
        if (_showSlider == false)
        {
            _container.transform.position = new Vector3(middlePoint.x - (_containerBounds.center.x), middlePoint.y - (_containerBounds.center.y), _container.transform.position.z);
        }
        //Bounding box is off the screen.
        else
        {
            var leftPoint = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            _container.transform.position = new Vector3(leftPoint.x + 0.5f, middlePoint.y - (_containerBounds.center.y), _container.transform.position.z);
        }

        //The math to make it left aligned would be, screen 0 + half of the first game objects bounds + padding


        /*We need to calculate the size of all the cards
        We need to calculate if the size of all the cards would be contained within the cameras view
        
        IF (isContainedInView)
            center the container inside the camera
        ELSE 
            container should be (left or potentially modified via a setting) aligned based on the camera
            and the scrollbar should show.

        */
        /*
        _container.transform.localPosition = new Vector3(Mathf.Lerp(minX, minX - maxX, _slider.value),
            _container.transform.localPosition.y,
            _container.transform.localPosition.z);
        */
    }


    private bool IsInsideCameraViewport(Bounds bounds)
    {
        //BUG - bounds.min and bounds.max are in local space, not world space
        Vector3 minPosition = camera.WorldToViewportPoint(_container.transform.position + bounds.min);
        Vector3 maxPosition = camera.WorldToViewportPoint(_container.transform.position + bounds.max);

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
