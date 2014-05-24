using UnityEngine;
using System.Collections;

public class ItemCreatorClass
{
	public string name;
	public Texture icon;
	public string description;
	
	public ItemCreatorClass(string _name, Texture _icon, string _description)
	{
		name = _name;
		icon = _icon;
		description = _description;
		
	}
}