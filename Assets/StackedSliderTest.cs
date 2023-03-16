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
    private float _scrollAmount = 30f;

    [SerializeField]
    float minX = float.MaxValue;
    [SerializeField]
    float maxX = float.MinValue;


    // Start is called before the first frame update
    void Start()
    {
        var camera = this.GetComponent<Camera>();
        Vector3 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 cameraDiff = cameraMax - cameraMin;


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
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        //We need to find trhe min position out of all the items in the container
        //as well as the max position out of all the items in the container

        //We will lerp the containers position based off of these values.



        //TODO - Logic to make it so no scrollbar shows if it doesn't go past the screen.
        _container.transform.localPosition = new Vector3(Mathf.Lerp(minX, minX-maxX, _slider.value),
            _container.transform.localPosition.y,
            _container.transform.localPosition.z);
    }
}
