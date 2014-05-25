#pragma strict

var ghost : GameObject;
var moveGhost : boolean;
var Script_GhostBehaviour : GhostBehaviour;
var ghostSound : AudioSource;

var wolf : GameObject;
var moveWolf : boolean;
var Script_WolfBehaviour : WolfBehaviour;
var wolfSound : AudioSource;

public var randomEvent : int;
var setSanity_Script : setSanity;

function Start () 
{
	ghost.SetActive (false);
	moveGhost = false;
	wolf.SetActive (false);
	moveWolf = false;
	Script_GhostBehaviour = ghost.GetComponent(GhostBehaviour);
	Script_WolfBehaviour =  wolf.GetComponent(WolfBehaviour);
	ghostSound = GameObject.Find("ghost").GetComponent(AudioSource);
	wolfSound = GameObject.Find("wolf").GetComponent(AudioSource);
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
	if (randomEvent == 1)
	{
		//Debug.Log("Spawn Ghost");
		setSanity_Script.sanity -= 1;
		ghostSound.transform.position = this.transform.position;
		ghostSound.Play();
		ghost.SetActive (true);
	}
	//Wolf
	if (randomEvent == 2)
	{
		//Debug.Log("Spawn Wolf");
		setSanity_Script.sanity -= 1;
		wolfSound.transform.position = this.transform.position;
		wolfSound.Play();
		wolf.SetActive (true);
	}
	
}


function OnTriggerExit()
{
	//Ghost
	if (randomEvent == 1)
	{
		moveGhost = true;
	}
	//Wolf
	if (randomEvent == 2)
	{
		moveWolf = true;
	}
	this.collider.enabled = false;
}