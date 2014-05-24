#pragma strict

var movTexture : MovieTexture;

function Start () 
{	//Play Movie
	renderer.material.mainTexture = movTexture; 
	movTexture.Play();
}

function Update () 
{

  if(Input.GetKeyDown(KeyCode.Escape)) 
  {
    if (movTexture.isPlaying) 
    {
    	movTexture.Pause();
    }
    else
    {
    	movTexture.Play();
    }
  }
  
  if (!movTexture.isPlaying) 
  {
  	Application.LoadLevel(2);
  }  

}

