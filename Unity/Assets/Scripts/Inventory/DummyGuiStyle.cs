using UnityEngine;
using System.Collections;


public class DummyGuiStyle : MonoBehaviour
{

    public GUISkin myCustomSkinThing;
    public Texture GuiBackground;
    public Texture GuiItemBackgroundUp;
    public Texture GuiItemBackgroundDown;
    public Texture GuiItemBackgroundFocus;
    public Texture DummyContent;

    private Texture[] _buttonBackground;
    private Texture[] _inventoryContent;
    private bool[] _buttonActive;

    private GameObject _mainCamera;

	// Use this for initialization
	void Start () {
	    _buttonBackground = new Texture[10];
        _inventoryContent = new Texture[10];
        _buttonActive = new bool[10];
        GUI.skin = myCustomSkinThing;
	}
	
	// Update is called once per frame
    void OnGUI()
    {

        for (int i = 0; i < 10; i++)
        {
            var currentRect = new Rect(Screen.width / 2 - 160 + (i * 32), Screen.height - 34, 32, 32);
            if (currentRect.Contains(Event.current.mousePosition))
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

            _buttonActive[i] = GUI.Button(currentRect, _inventoryContent[i]);
            GUI.DrawTexture(currentRect, _buttonBackground[i]);
        }
        
        GUI.DrawTexture(new Rect(Screen.width/2 - 180,Screen.height - 36,360,36), GuiBackground);

    }
}
