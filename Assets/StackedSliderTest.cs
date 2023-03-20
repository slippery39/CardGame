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



    // Start is called before the first frame update
    void Start()
    {
        var camera = this.GetComponent<Camera>();
        cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        cameraDiff = cameraMax - cameraMin;


        float paddingX = 2.5f;

        var gameObjects = _container.GetComponentsInChildren<Transform>();

        minX = float.MaxValue;
        maxX = float.MinValue;

        foreach (Transform t in gameObjects)
        {
            if (t.transform == this.transform)
            {
                continue;
            }

            if (t.transform.localPosition.x < minX)
            {
                minX = t.transform.localPosition.x;
            }
            if (t.transform.localPosition.x - cameraDiff.x + paddingX > maxX)
            {
                maxX = t.transform.localPosition.x - cameraDiff.x + paddingX;
                maxXReal = t.transform.localPosition.x;
            }
        }

        if (maxXReal > cameraDiff.x)
        {
            _showSlider = true;
        }
        else
        {
            _showSlider = false;
        }

        _containerBounds = CalculateLocalBounds(_container);
    }



    // Update is called once per frame
    void Update()
    {
        _slider.gameObject.SetActive(_showSlider);
        CalculateLocalBounds(_container);
        /*
        _container.transform.localPosition = new Vector3(Mathf.Lerp(minX, minX - maxX, _slider.value),
            _container.transform.localPosition.y,
            _container.transform.localPosition.z);
        */
    }



    private Bounds CalculateLocalBounds(GameObject gameObject, bool includeParentTransform = false)
    {
        Quaternion currentRotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length  == 0)
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
