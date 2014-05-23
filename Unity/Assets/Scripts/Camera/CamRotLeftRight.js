
var speed = 100;

function Update ()
{
	if (Input.GetAxis("CamRot") > 0)
	{
		rot = Vector3(0, Input.GetAxis("Mouse X"), 0);
		transform.Rotate(rot * Time.deltaTime * speed);
	}
}
