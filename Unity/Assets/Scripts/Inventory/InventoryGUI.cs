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

	public GameObject RaftPrefab;
	public GameObject TorchPrefab;

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

		// Auto-craft
		string[] raftArr = {"Log", "Rope"};
		string[] torchArr = {"Rag", "Fuel", "Stick"};
		string[] flashlightArr = {"Flashlight", "Battery"};

		if (checkPreconditions (raftArr)) {
			for(uint i = 0; i < raftArr.Length; i++)
				UseItem(getPosOf(raftArr[i]));
			// Add raft
			var obj = Instantiate(RaftPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			var item = obj.GetComponent<LootItem>();
			ItemCreatorClass itc = new ItemCreatorClass(item.name, item.iconSprite.texture, item.description);
			InventoryContent[getFirstEmpty()] = itc;
			Destroy(obj);
		}
		if (checkPreconditions (torchArr)) {
			for(uint i = 0; i < torchArr.Length; i++)
				UseItem(getPosOf(torchArr[i]));
			// Add raft
			var obj = Instantiate(TorchPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			var item = obj.GetComponent<LootItem>();
			ItemCreatorClass itc = new ItemCreatorClass(item.name, item.iconSprite.texture, item.description);
			InventoryContent[getFirstEmpty()] = itc;
			Destroy(obj);
		}

	}

	bool checkPreconditions(string[] pre)
	{
		for(uint i = 0; i < pre.Length; i++)
		{
			if(!hasPickedUp(pre[i]))
			   return false;
		}
		return true;
	}

	bool hasPickedUp(string name)
	{
		for (int i = 0; i < InventorySize; i++) {
			if(InventoryContent[i] != null && InventoryContent[i].name == name)
				return true;
		}
		return false;
	}

	int getPosOf(string name)
	{
		for (int i = 0; i < InventorySize; i++) {
			if(InventoryContent[i] != null && InventoryContent[i].name == name)
				return i;
		}
		return -1;
	}

	int getFirstEmpty()
	{
		for (int i = 0; i < InventorySize; i++) {
			if(InventoryContent[i] == null)
				return i;
		}
		return -1;
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
