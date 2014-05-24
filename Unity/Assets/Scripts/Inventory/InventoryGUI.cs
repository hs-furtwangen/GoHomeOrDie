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

    private const int _inventorySize = 10;

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

    public static int InventorySize
    {
        get { return _inventorySize; }
    }

    //ItemClass itemObject = new ItemClass();


	// Use this for initialization
	void Start () {
        _buttonBackground = new Texture[InventorySize];
        _buttonActive = new bool[InventorySize];

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
        GUI.skin = myCustomSkinThing;

		if (showInventory) 
		{
            for (int i = 0; i < InventorySize; i++)
            {
                var currentRect = new Rect(Screen.width / 2 - 160 + (i * 32), Screen.height - 34, 32, 32);
                if (currentRect.Contains(Event.current.mousePosition))
                {
                    _buttonBackground[i] = GuiItemBackgroundFocus;
                }
                else if (InventoryContent[i] != null)
                {
                    _buttonBackground[i] = GuiItemBackgroundDown;
                }
                else
                {
                    _buttonBackground[i] = GuiItemBackgroundUp;
                }

                GUI.DrawTexture(currentRect, _buttonBackground[i]);

                if (InventoryContent[i] != null)
                {
                    if (GUI.Button(currentRect, InventoryContent[i].icon))
                    {
                        UseItem(i);
                    }
                }
            }

            GUI.DrawTexture(new Rect(Screen.width / 2 - 180, Screen.height - 36, 360, 36), GuiBackground);
		}
	}

	void InventoryWindowMethod(int windowId)
	{
		for (int i = 0; i < InventorySize; i++)
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
