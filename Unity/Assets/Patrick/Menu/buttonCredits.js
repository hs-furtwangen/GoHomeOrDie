#pragma strict

var creditsNormal : Texture2D;
var creditsHover  : Texture2D;

function OnMouseEnter()
{
	guiTexture.texture = creditsHover;
}

function OnMouseExit()
{
	guiTexture.texture = creditsNormal;
}

function OnMouseDown()
{
	guiTexture.texture = creditsHover;
	//Application.LoadLevel();
}