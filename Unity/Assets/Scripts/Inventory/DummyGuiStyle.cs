using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DummyGuiStyle : MonoBehaviour
{

    public GUISkin myCustomSkinThing;
    public Texture GuiBackground;
    public Texture GuiItemBackgroundUp;
    public Texture GuiItemBackgroundDown;
    public Texture GuiItemBackgroundFocus;
    public Texture DummyContent;

    private bool[] _buttonActive;
    private Texture[] _buttonBackground;
    private Texture[] _inventoryContent;

	// Use this for initialization
	void Start () {
	    _buttonActive = new bool[10];
	    _buttonBackground = new Texture[10];
        _inventoryContent = new Texture[10];
	}
	
	// Update is called once per frame
    void OnGUI()
    {
        GUI.skin = myCustomSkinThing;

        _buttonActive[5] = true;
        _inventoryContent[5] = DummyContent;

        for (int i = 0; i < 10; i++)
        {
            if (_buttonActive[i])
            {
                _buttonBackground[i] = GuiItemBackgroundFocus;
            }
            else if (_inventoryContent[i] != null)
            {
                _buttonBackground[i] = GuiItemBackgroundDown;
            }
            else
            {
                _buttonBackground[i] = GuiItemBackgroundUp;
            }


            _buttonActive[i] = GUI.Button(new Rect(Screen.width / 2 - 160 + (i*32), Screen.height - 34, 32, 32), _inventoryContent[i]);
            GUI.DrawTexture(new Rect(Screen.width / 2 - 160 + (i * 32), Screen.height - 34, 32, 32), _buttonBackground[i]);
        }
        
        GUI.DrawTexture(new Rect(Screen.width/2 - 180,Screen.height - 36,360,36), GuiBackground);

    }
}
