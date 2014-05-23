var speed = 30.0;

private var moveDirection = Vector3.zero;

function FixedUpdate()
{
	moveDirection = Vector3(Input.GetAxis("Horizontal"), 0,
		Input.GetAxis("Vertical"));
	moveDirection = transform.TransformDirection(moveDirection);
	moveDirection *= speed;

	// Move the camera
	transform.position += moveDirection * Time.deltaTime;
}
