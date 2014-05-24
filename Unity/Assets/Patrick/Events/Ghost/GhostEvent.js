#pragma strict

var ghost : GameObject;
var moveGhost : boolean;
var Script_GhostBehaviour : GhostBehaviour;


function Start () 
{
	ghost.SetActive (false);
	moveGhost = false;
	Script_GhostBehaviour = ghost.GetComponent(GhostBehaviour);
}

function Update ()
{
	if (moveGhost == true)
	{
		Script_GhostBehaviour.ghostController();//Ghost will chase
	}
}

function OnTriggerEnter()
{
	//Sanity -= 1;
	Debug.Log("Triggered");
	ghost.SetActive (true);
}


function OnTriggerExit()
{
	Debug.Log("Trigger Left");
	moveGhost = true;
}