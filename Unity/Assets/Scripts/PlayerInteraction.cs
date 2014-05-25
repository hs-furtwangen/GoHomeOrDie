using UnityEngine;

// ReSharper disable once CheckNamespace
public class PlayerInteraction : MonoBehaviour
{
    public Transform ItemParent;
	public AudioSource[] movementSounds;
	public AudioSource[] movementWaterSounds;

    private float _movementSensitivity;
    private GameObject _mainCamera;
    private bool _movementByMouse;
    private bool _movementByItem;
    private Vector2 _mouseMovementTarget;
    private float _mouseStopDistance;

    private GameObject _moveToItem;
    private float _pickupDistance;
    private GameObject _mapGenerator;
    private MapGenerator _mapGeneratorScript;
    private Animator _anim;

	private AudioSource curMovementSound = null;

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
        _mapGenerator = GameObject.FindGameObjectWithTag("MapGenerator");
	    _mapGeneratorScript = _mapGenerator.GetComponent<MapGenerator>();
	    _anim = gameObject.GetComponent<Animator>();
	}
	
// ReSharper disable once UnusedMember.Local
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerAnimation("death");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerAnimation("pickup");
        }

        gameObject.GetComponent<SpriteRenderer>().sortingOrder = _mapGeneratorScript.GetTileZIndex(transform.position.y);
	    if(GameState.TheState == GameState.State.playing)
		{
			PlayerAction();
		}
		else if(GameState.TheState == GameState.State.paused)
		{
			PlayerAnimation("stop");
			GuiControl();
		}

		// Always center camera on player
		var oldPos = _mainCamera.transform.position;
		_mainCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, oldPos.z);
	}

    //called by an item that is clicked
    public void ItemMovement(GameObject item)
    {
        _moveToItem = item;
        _movementByItem = true;
        _movementByMouse = false;
    }

	public void PlayerAction()
	{
	    bool animationBlock = _anim.GetBool("AnimationBlock");

		float moveX = Input.GetAxis("Horizontal")*_movementSensitivity*Time.deltaTime;
		float moveY = Input.GetAxis("Vertical")*_movementSensitivity*Time.deltaTime;

		// Play sound
		if(moveX != 0 || moveY != 0)
			if(curMovementSound == null || !curMovementSound.isPlaying) {
				AudioSource sound;
				if(_mapGeneratorScript.checkTile(_mapGeneratorScript.getGridCoordinates((Vector2)this.transform.position), 0) == MapGenerator.TileType.Water)
					sound = movementWaterSounds[Random.Range(0, movementWaterSounds.Length-1)];
				else
					sound = movementSounds[Random.Range(0, movementSounds.Length-1)];
				sound.transform.position = this.transform.position;
				sound.Play();
				curMovementSound = sound;
			}
		
		Vector2 movementVector = Vector2.zero;
		
		// ReSharper disable CompareOfFloatsByEqualityOperator
		if ((moveX != 0 || moveY != 0) && GameState.State.playing == GameState.TheState && !animationBlock)
			// ReSharper restore CompareOfFloatsByEqualityOperator
		{
		    if (_mapGeneratorScript.isPassable(new Vector2(moveX, moveY) + (Vector2) transform.position))
		    {
		        transform.Translate(moveX, moveY, 0);
		        _mainCamera.transform.Translate(moveX, moveY, 0);
		    }
		    _movementByMouse = false;
			_movementByItem = false;
			_moveToItem = null;
			
			_anim.SetBool("Moving", true);
			if (Mathf.Abs(moveX) > Mathf.Abs(moveY) )
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
		
		if (Input.GetMouseButtonDown(0) && GameState.State.playing == GameState.TheState && !animationBlock)
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
                    PlayerAnimation("pickup");
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
			// Play sound
			if(curMovementSound == null || !curMovementSound.isPlaying) {
				AudioSource sound;
				if(_mapGeneratorScript.checkTile(_mapGeneratorScript.getGridCoordinates((Vector2)this.transform.position), 0) == MapGenerator.TileType.Water)
					sound = movementWaterSounds[Random.Range(0, movementWaterSounds.Length-1)];
				else
					sound = movementSounds[Random.Range(0, movementSounds.Length-1)];
				sound.transform.position = this.transform.position;
				sound.Play();
				curMovementSound = sound;
			}


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
		
		if (movementVector.magnitude < _mouseStopDistance )
		{
			_movementByMouse = false;
			
			if (_movementByItem && _moveToItem != null)
			{
                PlayerAnimation("pickup");
				_moveToItem.GetComponent<LootItem>().PickUp();
				Destroy(_moveToItem);
				_movementByItem = false;
				_moveToItem = null;
			}
		}
		else
		{
		    if (_mapGeneratorScript.isPassable((movementVector.normalized*_movementSensitivity*Time.deltaTime) + (Vector2) transform.position) && !animationBlock)
		    {
                transform.Translate(movementVector.normalized * _movementSensitivity * Time.deltaTime);
                _mainCamera.transform.Translate(movementVector.normalized * _movementSensitivity * Time.deltaTime);
		    }
		}
	}

	public void GuiControl()
	{}

    private void PlayerAnimation(string direction)
    {
        

        switch(direction)
        {
            case "stop":
                _anim.SetBool("Moving", false);
                _anim.SetBool("WalkLeft", false);
                _anim.SetBool("WalkRight", false);
                _anim.SetBool("WalkUp", false);
                _anim.SetBool("WalkDown", false);
                break;
            case "left":
                _anim.SetBool("Moving", true);
                _anim.SetBool("WalkLeft", true);
                _anim.SetBool("WalkRight", false);
                _anim.SetBool("WalkUp", false);
                _anim.SetBool("WalkDown", false);
                break;
            case "right":
                _anim.SetBool("Moving", true);
                _anim.SetBool("WalkLeft", false);
                _anim.SetBool("WalkRight", true);
                _anim.SetBool("WalkUp", false);
                _anim.SetBool("WalkDown", false);
                break;
            case "up":
                _anim.SetBool("Moving", true);
                _anim.SetBool("WalkLeft", false);
                _anim.SetBool("WalkRight", false);
                _anim.SetBool("WalkUp", true);
                _anim.SetBool("WalkDown", false);
                break;
            case "down":
                _anim.SetBool("Moving", true);
                _anim.SetBool("WalkLeft", false);
                _anim.SetBool("WalkRight", false);
                _anim.SetBool("WalkUp", false);
                _anim.SetBool("WalkDown", true);
                break;
            case "death":
                _anim.SetBool("AnimationBlock", true);
                _anim.SetTrigger("Death");
                break;
            case "pickup":
                _anim.SetBool("AnimationBlock", true);
                _anim.SetTrigger("Pickup");
                break;
        }
    }

    public void AnimationEnd()
    {
        _anim.SetBool("AnimationBlock", false);
    }
}
/*
 * case "stop":
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
            case "death":
                anim.SetTrigger("Death");
                break;
*/
