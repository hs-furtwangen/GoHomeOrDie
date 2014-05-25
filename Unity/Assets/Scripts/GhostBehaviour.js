#pragma strict

var Char : GameObject;
var speed : float;
var setSanity_Script : setSanity;

function Start()
{
	Char = GameObject.Find("Player").gameObject;
	speed = 0.01;
}

function ghostControler()
{
	if (this.transform.position.x >= Char.transform.position.x)
		{this.transform.position.x -= speed;}
	else
		{this.transform.position.x += speed;}
		
	if (this.transform.position.y <= Char.transform.position.y)
		{this.transform.position.y += speed;}
	else
		{this.transform.position.y -= speed;}
}


function OnCollisionEnter(collision : Collision) 
{
	//Sound
	//Debug.Log("BOOOOO");
	setSanity_Script.sanity -= 2;
	Destroy(transform.parent.gameObject);
}