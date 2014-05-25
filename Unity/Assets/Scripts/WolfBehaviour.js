#pragma strict

var Char : GameObject;
var speed : float;//Wolf running speed
var knockBack : float;//Knockback distance
var hitWolf : boolean;
var counter : int;
var startTime;
var timer : int;
var setSanity_Script : setSanity;

function Start()
{
	Char = GameObject.Find("Player").gameObject;
	speed = 0.3;
	knockBack = 0.15;
	hitWolf = false;
	counter = 0;
}

function wolfControler()
{
		this.transform.position.x -= speed;
		
		if (hitWolf == true)//Knockback
		{
			Char.transform.position.y += knockBack;
			Char.transform.position.x -= knockBack;
			counter ++;
			
			Char.GetComponent(SpriteRenderer).color.a = 0.5;//Changes color for 3sec (if char was hit)
			
			if (counter == 10)
			{hitWolf = false;}
		}
		TimerStart();
}

function OnCollisionEnter(collision : Collision) 
{
	//Sound
	setSanity_Script.sanity -= 2;
	hitWolf = true;
}


function TimerStart()//Wolf despawn after 10 sec
{
	startTime = Time.time; 
	timer = Time.time; 
	
	if(timer > 3)
	{ 
		Char.GetComponent(SpriteRenderer).color.a = 1.0;
	}
	 
	if(timer > 10)
	{ 
		//Destroy(transform.parent.gameObject);
	}
}

