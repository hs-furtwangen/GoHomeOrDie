using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour {

    public GUISkin myCustomSkinThing;
    public Texture GuiBackground;
    public Texture GuiItemBackgroundUp;
    public Texture GuiItemBackgroundDown;
    public Texture GuiItemBackgroundFocus;
    public Texture DummyContent;

    private Texture[] _buttonBackground;
    private bool[] _buttonActive;

	private bool showInventory = false;

	static public  int inventorySize = 10;
	//private Dictionary<int, string> inventoryNameDictionary;
	//public static Dictionary<int, Texture2D> inventoryNameDictionary = new Dictionary<int, Texture2D>()
	public List<ItemCreatorClass> InventoryContent = new List<ItemCreatorClass>()
	{
		{null},
        {null},
        {null},
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
        _buttonBackground = new Texture[10];
        _buttonActive = new bool[10];
        //GUI.skin = myCustomSkinThing;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.I))
		{
            Debug.Log("bla");
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
            for (int i = 0; i < 10; i++)
            {
                var currentRect = new Rect(Screen.width / 2 - 160 + (i * 32), Screen.height - 34, 32, 32);
                if (currentRect.Contains(Event.current.mousePosition))
                {
                    _buttonBackground[i] = GuiItemBackgroundFocus;
                }
                else if (InventoryContent[i] != null)
                {
                    _buttonBackground[i] = GuiItemBackgroundDown;
                    if (GUI.Button(currentRect, InventoryContent[i].icon))
                    {
                        UseItem(i);
                    }
                }
                else
                {
                    _buttonBackground[i] = GuiItemBackgroundUp;
                }


                GUI.DrawTexture(currentRect, _buttonBackground[i]);
            }

            GUI.DrawTexture(new Rect(Screen.width / 2 - 180, Screen.height - 36, 360, 36), GuiBackground);
		}
	}

	void InventoryWindowMethod(int windowId)
	{
		for (int i = 0; i < inventorySize; i++)
		{
			if(InventoryContent[i] != null)
			{
				if(GUI.Button( new Rect((Screen.height*0.1f)*i, 0, Screen.height*0.1f,Screen.height*0.1f), InventoryContent[i].icon))
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
		InventoryContent[i] = null;
	}
}
