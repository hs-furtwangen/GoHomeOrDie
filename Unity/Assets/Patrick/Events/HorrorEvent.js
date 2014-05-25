#pragma strict

var ghost : GameObject;
var moveGhost : boolean;
var Script_GhostBehaviour : GhostBehaviour;

var wolf : GameObject;
var moveWolf : boolean;
var Script_WolfBehaviour : WolfBehaviour;

public var randomEvent : int;

function Start () 
{
	ghost.SetActive (false);
	moveGhost = false;
	wolf.SetActive (false);
	moveWolf = false;
	Script_GhostBehaviour = ghost.GetComponent(GhostBehaviour);
	Script_WolfBehaviour =  wolf.GetComponent(WolfBehaviour);
}

function Update ()
{
	//Ghost
	if (moveGhost == true)
	{
		Script_GhostBehaviour.ghostControler();//Ghost will chase
	}
	//Wolf
	if (moveWolf == true)
	{
		Script_WolfBehaviour.wolfControler();//Wolf will attack
	}
}

function OnTriggerEnter()
{
	randomEvent = Random.Range(1, 2);
	
	//Ghost
	if (randomEvent == 0)
	{
		Debug.Log("Spawn Ghost");
		//Sanity -= 1;
		//Play ghost sound
		ghost.SetActive (true);
	}
	//Wolf
	if (randomEvent == 1)
	{
		Debug.Log("Spawn Wolf");
		//Sanity -= 1;
		//Play wolf sound
		wolf.SetActive (true);
	}
	
}


function OnTriggerExit()
{
	//Ghost
	if (randomEvent == 0)
	{
		moveGhost = true;
	}
	//Wolf
	if (randomEvent == 1)
	{
		moveWolf = true;
	}
	this.collider.enabled = false;
}

//Problems
//Wolf kills ghosts for some reason...

