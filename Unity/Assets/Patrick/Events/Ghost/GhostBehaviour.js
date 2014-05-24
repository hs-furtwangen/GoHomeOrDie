#pragma strict

public var Char : GameObject;

function ghostController()
{
	if (this.transform.position.x >= Char.transform.position.x)
		{this.transform.position.x -= 0.01;}
	else
		{this.transform.position.x += 0.01;}
		
	if (this.transform.position.y <= Char.transform.position.y)
		{this.transform.position.y += 0.01;}
	else
		{this.transform.position.y -= 0.01;}
	
	transform.LookAt(Char.transform);
}

function OnCollisionEnter(collision : Collision) 
{
	//Sanity -= 2;
	Destroy (gameObject);
}