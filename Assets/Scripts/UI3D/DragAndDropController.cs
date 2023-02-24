using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragAndDropController : MonoBehaviour
{

    private Card3D _selectedCard;
    private float _originalZPos;
    private Quaternion _originalRotation;

    [SerializeField]
    private UI3DController _ui3DController;

    private Rigidbody _dragdropRigidBody;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Drag and drop controller, mouse button down");
            if (_selectedCard == null)
            {
                //Pick up the card
                RaycastHit hit = UnityHelper.CastRay();

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<Card3D>() == null)
                    {
                        return;
                    }

                    Debug.Log("Drag and drop controller, card 3d found");

                    _selectedCard = hit.collider.gameObject.GetComponent<Card3D>();

                    _originalZPos = _selectedCard.transform.position.z;
                    _originalRotation = _selectedCard.transform.rotation;
                    Cursor.visible = false;
                    AddDragDropRigidBody(_selectedCard.gameObject);
                    //Disable hover tool tip effects when dragging cards around.
                    var card3DHover = _selectedCard.GetComponent<Card3DHover>();
                    if (card3DHover != null)
                    {
                        HoverController.instance.Disable();
                        card3DHover.enabled = false;
                    }

                }
            }
            else
            {
                SetSelectedCardPosition(0f);
                HoverController.instance.Enable();
                RemoveDragdropRigidBody();
                var card3DHover = _selectedCard.GetComponent<Card3DHover>();
                if (card3DHover != null)
                {
                    card3DHover.enabled = true;
                }

                //Check to see what is underneath it;


                RaycastHit[] hits = UnityHelper.CastRayMulti();
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

        var newWorldPosition = new Vector3(worldPosition.x, height, worldPosition.z);
        var diff = newWorldPosition - _selectedCard.transform.position;

        var rb = _selectedCard.GetComponent<Rigidbody>();
        rb.velocity = 10 * diff;
        rb.rotation = Quaternion.Euler(new Vector3(rb.velocity.z - 270, 0, -rb.velocity.x));

        //_selectedCard.transform.position = new Vector3(worldPosition.x, height, worldPosition.z);
    }
    private Rigidbody AddDragDropRigidBody(GameObject gameObject)
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.angularDrag = 3;
        rb.isKinematic = false;
        _dragdropRigidBody = rb;
        return rb;
    }

    private void RemoveDragdropRigidBody()
    {
        _dragdropRigidBody.gameObject.transform.rotation = _originalRotation;
        Destroy(_dragdropRigidBody);
    }
}
