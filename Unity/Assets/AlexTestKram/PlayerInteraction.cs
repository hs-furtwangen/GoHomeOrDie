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
        var anim = gameObject.GetComponent<Animator>();

// ReSharper disable CompareOfFloatsByEqualityOperator
	    if (moveX != 0 || moveY != 0)
// ReSharper restore CompareOfFloatsByEqualityOperator
	    {
	        transform.Translate(moveX, moveY, 0);
	        _mainCamera.transform.Translate(moveX, moveY, 0);
	        _movementByMouse = false;
	        _movementByItem = false;
	        _moveToItem = null;

	        anim.SetBool("Moving", true);
	        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
	        {
	            if (moveX > 0)
	            {
	                PlayerAnimation("right");
	            }
	            else if (moveX < 0)
	            {
	                PlayerAnimation("left");
	            }
	        }
	        else if (Mathf.Abs(moveY) > Mathf.Abs(moveX))
	        {
                if (moveY > 0)
                {
                    PlayerAnimation("up");
                }
                else if (moveY < 0)
                {
                    PlayerAnimation("down");
                }
            }

	    }
	    else
	    {
            PlayerAnimation("stop");
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

	    if (_movementByMouse || _movementByItem)
	    {
            Vector2[] compass = {Vector2.up, -Vector2.up, -Vector2.right, Vector2.right};
            var maxDot = -Mathf.Infinity;
            var ret = Vector2.zero;

	        foreach (Vector2 dir in compass)
	        {
                var t = Vector2.Dot(movementVector, dir);
                if (t > maxDot)
                {
                    ret = dir;
                    maxDot = t;
                }
	        }

	        if (ret == Vector2.up)
	        {
	            PlayerAnimation("up");
	        }
            else if (ret == -Vector2.up)
            {
                PlayerAnimation("down");
            }
            else if (ret == -Vector2.right)
            {
                PlayerAnimation("left");
            }
            else if (ret == Vector2.right)
            {
                PlayerAnimation("right");
            }
	    }

	    if (movementVector.magnitude < _mouseStopDistance)
	    {
	        _movementByMouse = false;

	        if (_movementByItem && _moveToItem != null)
	        {
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

    private void PlayerAnimation(string direction)
    {
        var anim = gameObject.GetComponent<Animator>();

        switch(direction)
        {
            case "stop":
                anim.SetBool("Moving", false);
                anim.SetBool("WalkLeft", false);
                anim.SetBool("WalkRight", false);
                anim.SetBool("WalkUp", false);
                anim.SetBool("WalkDown", false);
                break;
            case "left":
                anim.SetBool("Moving", true);
                anim.SetBool("WalkLeft", true);
                anim.SetBool("WalkRight", false);
                anim.SetBool("WalkUp", false);
                anim.SetBool("WalkDown", false);
                break;
            case "right":
                anim.SetBool("Moving", true);
                anim.SetBool("WalkLeft", false);
                anim.SetBool("WalkRight", true);
                anim.SetBool("WalkUp", false);
                anim.SetBool("WalkDown", false);
                break;
            case "up":
                anim.SetBool("Moving", true);
                anim.SetBool("WalkLeft", false);
                anim.SetBool("WalkRight", false);
                anim.SetBool("WalkUp", true);
                anim.SetBool("WalkDown", false);
                break;
            case "down":
                anim.SetBool("Moving", true);
                anim.SetBool("WalkLeft", false);
                anim.SetBool("WalkRight", false);
                anim.SetBool("WalkUp", false);
                anim.SetBool("WalkDown", true);
                break;
        }
    }
}
