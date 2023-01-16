using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragAndDropController : MonoBehaviour
{

    private Card3D _selectedCard;
    private float _originalZPos;

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
                    if (hit.collider.gameObject.GetComponent<Card3D>() == null)
                    {
                        return;
                    }

                    Debug.Log("Drag and drop controller, card 3d found");

                    _selectedCard = hit.collider.gameObject.GetComponent<Card3D>();
                    _originalZPos = _selectedCard.transform.position.z;
                    Cursor.visible = false;
                }
            }
            else
            {
                SetSelectedCardPosition(0f);

                //Check to see what is underneath it;

                RaycastHit[] hits = CastRayMulti();
                hits = hits.Where(h => h.collider.gameObject.GetComponent<DropArea>() != null).ToArray();

                if (!hits.Any())
                {
                    Debug.Log("no hits found");
                    _selectedCard = null;
                    Cursor.visible = true;
                }

                //TODO - filter by enabled drop areas
                var hit = hits.FirstOrDefault();

                if (hit.collider != null)
                {
                    Debug.Log("Testing drag and drop hits..");
                    Debug.Log("is the hit the same as the dragged card?");
                    Debug.Log(hit.collider.gameObject == _selectedCard);
                    Debug.Log("what is the hit?");
                    Debug.Log(hit.collider.gameObject.name);

                    _selectedCard.transform.position = hit.collider.gameObject.transform.position;
                }

                _selectedCard = null;
                Cursor.visible = true;
            }
        }

        //Drag around picked up card
        if (_selectedCard != null)
        {
            SetSelectedCardPosition(0.3f);
        }
    }


    public void SetSelectedCardPosition(float height)
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedCard.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        _selectedCard.transform.position = new Vector3(worldPosition.x, height, worldPosition.z);
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

    private RaycastHit[] CastRayMulti()
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

        RaycastHit[] hits;

        hits = Physics.RaycastAll(worldMousePointerNear, worldMousePointerFar - worldMousePointerNear);

        return hits;
    }
}
