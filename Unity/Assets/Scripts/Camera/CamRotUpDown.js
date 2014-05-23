
var speed = 100;
var upperLimit = 90;
var lowerLimit = 10;

function Update ()
{
	var my = Input.GetAxis("Mouse Y");
	if ((Input.GetAxis("CamRot") > 0))
	{
		if (((my < 0) && (transform.localEulerAngles.x <= upperLimit)) ||
			((my > 0) && (transform.localEulerAngles.x >= lowerLimit)))
		{
			rot = Vector3(-my, 0, 0);
			transform.Rotate(rot * Time.deltaTime * speed);
		}
	}
}
