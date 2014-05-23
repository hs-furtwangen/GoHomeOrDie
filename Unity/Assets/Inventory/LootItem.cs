using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootItem : MonoBehaviour {
	
	public GameObject inventoryGUI;
	
	public string name;
	public Texture icon;
	public string description;
	private List<ItemCreatorClass> inventoryList = new List<ItemCreatorClass>();


	ItemCreatorClass icc;
	void Start () 
	{
		icc = new ItemCreatorClass(name, icon, description);
		inventoryList = inventoryGUI.GetComponent<InventoryGUI>().inventoryList;
	}


	public void PickUp()
	{
		for(int i = 0; i < InventoryGUI.inventorySize; i++)
		{

			if(inventoryList[i] == null)
			{
				inventoryList[i] = icc;
				break;
			}
		}
	}
}
