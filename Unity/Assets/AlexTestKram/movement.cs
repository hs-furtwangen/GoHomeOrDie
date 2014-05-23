using UnityEngine;

public class movement : MonoBehaviour
{
    public Transform itemParent;

    private float movementSensitivity;
    private GameObject mainCamera;
    private bool movementByMouse;
    private bool movementByItem;
    private Vector2 mouseMovementTarget;
    private float mouseStopDistance;

    private GameObject moveToItem;
    private float pickupDistance;

	// Use this for initialization
	void Start ()
	{
	    movementSensitivity = 1.3f;
	    mainCamera = GameObject.Find("/Main Camera");
        movementByMouse = false;
        movementByItem = false;
        mouseMovementTarget = new Vector2();
	    mouseStopDistance = 0.15f;
	    pickupDistance = mouseStopDistance*2;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float moveX = Input.GetAxis("Horizontal")*movementSensitivity*Time.deltaTime;
	    float moveY = Input.GetAxis("Vertical")*movementSensitivity*Time.deltaTime;

	    Vector2 movementVector = Vector2.zero;

	    if (moveX != 0 || moveY != 0)
	    {
	        this.transform.Translate(moveX, moveY, 0);
	        mainCamera.transform.Translate(moveX, moveY, 0);
	        movementByMouse = false;
	    }

	    if (Input.GetMouseButtonDown(0))
	    {
            movementByMouse = true;
            mouseMovementTarget = mainCamera.camera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
	    }

	    if (Input.GetAxis("ItemPickup") > 0)
	    {
	        Transform closestItem = null;

	        for (int i = 0; i < itemParent.childCount; i++)
	        {
                Transform item = itemParent.GetChild(i);

	            if (Vector2.Distance(item.position, this.transform.position) < pickupDistance)
	            {
                    //ToDo: Send Item to inventory
                    Destroy(item.gameObject);
	            }

	        }
	    }

        if (movementByItem && moveToItem != null && !movementByMouse)
	    {
            movementVector = (Vector2)(moveToItem.transform.position - this.transform.position);
	    }

	    if (movementByMouse)
	    {
	        movementVector = (mouseMovementTarget - (Vector2) this.transform.position);
	    }

	    if (movementVector.magnitude < mouseStopDistance)
	    {
	        movementByMouse = false;

	        if (movementByItem)
	        {
                //ToDo: Send Item to inventory
	            Destroy(moveToItem);
	            movementByItem = false;
	            moveToItem = null;
	        }
	    }
	    else
	    {
	        this.transform.Translate(movementVector.normalized*movementSensitivity*Time.deltaTime);
	        mainCamera.transform.Translate(movementVector.normalized*movementSensitivity*Time.deltaTime);
	    }
        
	}

    //called by an item that is clicked
    public void ItemMovement(GameObject item)
    {
        moveToItem = item;
        movementByItem = true;
        movementByMouse = false;
    }
}
