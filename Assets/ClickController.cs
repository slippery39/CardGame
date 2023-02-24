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
            RaycastHit hit = UnityHelper.CastRay();

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
