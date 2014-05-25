using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootItem : MonoBehaviour {
	
	private GameObject inventoryGUI;
	
	public string itemname;
	private Texture icon;
    public Sprite iconSprite;
	public string description;
	private List<ItemCreatorClass> inventoryList = new List<ItemCreatorClass>();


	ItemCreatorClass icc;
	void Start ()
	{
        var icon = new Texture2D((int)iconSprite.rect.width, (int)iconSprite.rect.height);

        var pixels = iconSprite.texture.GetPixels((int)iconSprite.textureRect.x,
        (int)iconSprite.textureRect.y,
        (int)iconSprite.textureRect.width,
        (int)iconSprite.textureRect.height);

        icon.SetPixels(pixels);
        icon.Apply();

        inventoryGUI = GameObject.FindGameObjectWithTag("Player");
		icc = new ItemCreatorClass(itemname, icon, description);
        inventoryList = inventoryGUI.GetComponent<InventoryGUI>().InventoryContent;
	}


	public void PickUp()
	{
		for(int i = 0; i < InventoryGUI.InventorySize; i++)
		{

			if(inventoryList[i] == null)
			{
				inventoryList[i] = icc;
				break;
			}
		}
	}
}
