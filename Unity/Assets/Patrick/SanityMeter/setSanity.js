#pragma strict

var mainSprite : Sprite;
var sprites :  Sprite[];
//var PlayerInteraction_Script : PlayerInteraction;
public static var sanity : int;

function Start()
{
	sanity = 6;
}

function Update ()
{
	switch (sanity) 
	{
	case 0:
	mainSprite = sprites[6];
	    break;
	case 1:
	mainSprite = sprites[5];
	    break;
	case 2:
	mainSprite = sprites[4];
	    break;
	case 3:
	mainSprite = sprites[3];
	    break;
	case 4:
	mainSprite = sprites[2];
	    break;
	case 5:
	mainSprite = sprites[1];
	    break;
	case 6:
	mainSprite = sprites[0];
//	PlayerInteraction_Script.animation.Play("Death");
	    break;
	}
	GetComponent(SpriteRenderer).sprite = mainSprite;	
	Debug.Log(sanity);
}

   