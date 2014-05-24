using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootItem : MonoBehaviour {
	
	private GameObject inventoryGUI;
	
	public string itemname;
	public Texture icon;
	public string description;
	private List<ItemCreatorClass> inventoryList = new List<ItemCreatorClass>();


	ItemCreatorClass icc;
	void Start ()
	{
	    inventoryGUI = GameObject.FindGameObjectWithTag("Player");
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
