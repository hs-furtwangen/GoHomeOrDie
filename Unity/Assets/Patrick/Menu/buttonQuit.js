#pragma strict

var quitNormal : Texture2D;
var quitHover  : Texture2D;

function OnMouseEnter()
{
	guiTexture.texture = quitHover;
}

function OnMouseExit()
{
	guiTexture.texture = quitNormal;
}

function OnMouseDown()
{
	guiTexture.texture = quitHover;
	Application.Quit();
}