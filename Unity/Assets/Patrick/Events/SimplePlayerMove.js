#pragma strict


function Update () 
{
	var _speed = 5;

	var forwards = Input.GetKey("w");
	var backwards = Input.GetKey("s");
	var left = Input.GetKey("a");
	var right = Input.GetKey("d");

	if (forwards == true)
	{
		transform.Translate(Vector3.up * _speed * Time.deltaTime);
	}
	if (backwards == true)
	{
		transform.Translate(Vector3.down * _speed * Time.deltaTime);
	}
	if (left == true)
	{
		transform.position.x -= 0.1;
	}
	if (right == true)
	{
		transform.position.x += 0.1;
	}
}