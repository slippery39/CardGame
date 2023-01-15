using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropController : MonoBehaviour
{

    private Card3D _selectedCard;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Drag and drop controller, mouse button down");
            if (_selectedCard == null)
            {
                //Pick up the card
                RaycastHit hit = CastRay();

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponentInParent<Card3D>() == null)
                    {
                        return;
                    }

                    Debug.Log("Drag and drop controller, card 3d found");

                    _selectedCard = hit.collider.gameObject.GetComponentInParent<Card3D>();
                    Cursor.visible = false;
                }
            }
            else
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedCard.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                _selectedCard.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

                _selectedCard = null;
                Cursor.visible = true;
            }
        }

        //Drag around picked up card
        if (_selectedCard != null)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedCard.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            _selectedCard.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        }
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePositionFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);

        Vector3 screenMousePositionNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 worldMousePointerFar = Camera.main.ScreenToWorldPoint(screenMousePositionFar);
        Vector3 worldMousePointerNear = Camera.main.ScreenToWorldPoint(screenMousePositionNear);

        RaycastHit hit;

        Physics.Raycast(worldMousePointerNear, worldMousePointerFar - worldMousePointerNear, out hit);

        return hit;
    }
}
