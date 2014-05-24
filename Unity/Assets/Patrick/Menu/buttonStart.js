#pragma strict

var playNormal : Texture2D;
var playHover  : Texture2D;

function OnMouseEnter()
{
	guiTexture.texture = playHover;
}

function OnMouseExit()
{
	guiTexture.texture = playNormal;
}

function OnMouseDown()
{
	guiTexture.texture = playHover;
	Application.LoadLevel(1);
}