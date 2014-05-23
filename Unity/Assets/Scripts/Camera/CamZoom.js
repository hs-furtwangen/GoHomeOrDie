var speed = 1000.0;
var upperLimit = 90.0;
var lowerLimit = 0;

private var moveDirection = Vector3.zero;

function Update ()
{
	if (((Input.GetAxis("Mouse ScrollWheel") > 0) && (transform.localPosition.z < -lowerLimit)) ||
		((Input.GetAxis("Mouse ScrollWheel") < 0) && (transform.localPosition.z > -upperLimit)))
	{
		moveDirection = Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		transform.position += moveDirection * Time.deltaTime;
	}
}
