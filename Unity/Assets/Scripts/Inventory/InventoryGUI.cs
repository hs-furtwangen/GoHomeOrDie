using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour {


	private bool showInventory = false;
	private Rect InventoryWindowRect = new Rect((Screen.width *0.5f)-(Screen.width*0.4f)/2, Screen.height*0.9f , Screen.width*0.4f, Screen.height*0.1f);

	static public  int inventorySize = 7;
	//private Dictionary<int, string> inventoryNameDictionary;
	//public static Dictionary<int, Texture2D> inventoryNameDictionary = new Dictionary<int, Texture2D>()
	public List<ItemCreatorClass> inventoryList = new List<ItemCreatorClass>()
	{
		{null},
		{null},
		{null},
		{null},
		{null},
		{null},
		{null}
	};

	//ItemClass itemObject = new ItemClass();


	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.I))
		{
			if(showInventory)
			{
				showInventory = false;
				GameState.TheState = GameState.State.playing;
			}
			else
			{
				showInventory = true;
				GameState.TheState = GameState.State.paused;

			}

		}

	}

	void OnGUI()
	{
		if (showInventory) 
		{
			InventoryWindowRect = GUI.Window(0, InventoryWindowRect, InventoryWindowMethod, "");
		}


	}

	void InventoryWindowMethod(int windowId)
	{
		for (int i = 0; i < inventorySize; i++)
		{
			if(inventoryList[i] != null)
			{
				if(GUI.Button( new Rect((Screen.height*0.1f)*i, 0, Screen.height*0.1f,Screen.height*0.1f), inventoryList[i].icon))
				{
					//Mit dem Object was machen
					UseItem(i);
				}
			}
			else
			{
				GUI.Button( new Rect((Screen.height*0.1f)*i, 0, Screen.height*0.1f,Screen.height*0.1f), string.Empty);
			}
		}
	}

	void UseItem(int i)
	{
		inventoryList[i] = null;
	}

}
