using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickController : MonoBehaviour
{
    [SerializeField]
    private UI3DController _ui3DController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Pick up the card

            var cameras = GameObject.FindObjectsOfType<Camera>();

            //check if we have a UI camera available.
            var uiCamera = cameras.Where(c => c.gameObject.tag == "UI Camera").FirstOrDefault();

            RaycastHit hit;
            if (uiCamera == null)
            {
                //Grab the hit from our main camera.
                hit = UnityHelper.CastRay();
            }
            else
            {
                //Grab the hit from our ui camera.
                hit = UnityHelper.CastRay(uiCamera);
            }

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<UIGameEntity3D>() == null)
                {
                    return;
                }

                var clickedEntity = hit.collider.gameObject.GetComponent<UIGameEntity3D>();

                if (clickedEntity != null)
                {
                    _ui3DController.HandleSelection(clickedEntity.EntityId);
                }
            }
        }
    }
}
