using UnityEngine;

// ReSharper disable once CheckNamespace
public class PlayerInteraction : MonoBehaviour
{
    public Transform ItemParent;

    private float _movementSensitivity;
    private GameObject _mainCamera;
    private bool _movementByMouse;
    private bool _movementByItem;
    private Vector2 _mouseMovementTarget;
    private float _mouseStopDistance;

    private GameObject _moveToItem;
    private float _pickupDistance;

// ReSharper disable once UnusedMember.Local
	void Start ()
	{
	    _movementSensitivity = 1.3f;
	    _mainCamera = GameObject.Find("/Main Camera");
        _movementByMouse = false;
        _movementByItem = false;
        _mouseMovementTarget = new Vector2();
	    _mouseStopDistance = 0.15f;
	    _pickupDistance = _mouseStopDistance*2;
	}
	
// ReSharper disable once UnusedMember.Local
	void Update ()
	{
	    float moveX = Input.GetAxis("Horizontal")*_movementSensitivity*Time.deltaTime;
	    float moveY = Input.GetAxis("Vertical")*_movementSensitivity*Time.deltaTime;

	    Vector2 movementVector = Vector2.zero;

// ReSharper disable CompareOfFloatsByEqualityOperator
	    if (moveX != 0 || moveY != 0)
// ReSharper restore CompareOfFloatsByEqualityOperator
	    {
	        transform.Translate(moveX, moveY, 0);
	        _mainCamera.transform.Translate(moveX, moveY, 0);
	        _movementByMouse = false;
	        _movementByItem = false;
	        _moveToItem = null;
	    }

	    if (Input.GetMouseButtonDown(0))
	    {
            _movementByMouse = true;
            _mouseMovementTarget = _mainCamera.camera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
	    }

	    if (Input.GetAxis("ItemPickup") > 0)
	    {
	        for (int i = 0; i < ItemParent.childCount; i++)
	        {
                Transform item = ItemParent.GetChild(i);

	            if (Vector2.Distance(item.position, transform.position) < _pickupDistance)
	            {
                    //ToDo: Send Item to inventory
					item.GetComponent<LootItem>().PickUp();
                    Destroy(item.gameObject);
	            }

	        }
	    }

        if (_movementByItem && _moveToItem != null && !_movementByMouse)
	    {
            movementVector = _moveToItem.transform.position - transform.position;
	    }

	    if (_movementByMouse)
	    {
	        movementVector = (_mouseMovementTarget - (Vector2) transform.position);
	    }

	    if (movementVector.magnitude < _mouseStopDistance)
	    {
	        _movementByMouse = false;

	        if (_movementByItem)
	        {
                //ToDo: Send Item to inventory
				_moveToItem.GetComponent<LootItem>().PickUp();
	            Destroy(_moveToItem);
	            _movementByItem = false;
	            _moveToItem = null;
	        }
	    }
	    else
	    {
	        transform.Translate(movementVector.normalized*_movementSensitivity*Time.deltaTime);
	        _mainCamera.transform.Translate(movementVector.normalized*_movementSensitivity*Time.deltaTime);
	    }
        
	}

    //called by an item that is clicked
    public void ItemMovement(GameObject item)
    {
        _moveToItem = item;
        _movementByItem = true;
        _movementByMouse = false;
    }
}
