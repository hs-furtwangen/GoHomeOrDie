#pragma strict

var mainTexture : Texture2D;
var textures :  Texture2D[];
var changeInterval : float = 0.33;

function Update ()
{
	if (textures.length == 0)
	{return;}
	var index : int = Time.time / changeInterval;
	index = index%textures.length;
	renderer.material.mainTexture = textures[index];
}
   