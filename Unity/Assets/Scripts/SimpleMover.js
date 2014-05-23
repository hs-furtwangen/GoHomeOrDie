/// This script moves the character controller forward
/// and sideways based on the arrow keys.
/// It also jumps when pressing space.
/// Make sure to attach a character controller to the same game object.
/// It is recommended that you make only one call to Move or SimpleMove per frame.
var speed = 100.0;
var jumpSpeed = 8.0;
var gravity = 20.0;

private var moveDirection = Vector3.zero;

function IsMoving()
{
	return moveDirection.x != 0 || moveDirection.z != 0;
}

function FixedUpdate()
{
	if (Input.GetKey(KeyCode.W))
		transform.position += transform.up * Time.deltaTime * 5;
	if (Input.GetKey(KeyCode.S))
		transform.position -= transform.up * Time.deltaTime * 5;
	if (Input.GetKey(KeyCode.A))
		transform.position -= transform.right * Time.deltaTime * 5;
	if (Input.GetKey(KeyCode.D))
		transform.position += transform.right * Time.deltaTime * 5;
}
