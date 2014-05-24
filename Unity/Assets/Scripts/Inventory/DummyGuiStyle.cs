using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DummyGuiStyle : MonoBehaviour
{

    public GUISkin myCustomSkinThing;
    public Texture GuiBackground;
    public Texture GuiItemBackgroundUp;
    public Texture GuiItemBackgroundDown;
    public Texture DummyContent;

    private bool[] _buttonActive;
    private Texture[] _buttonBackground;

	// Use this for initialization
	void Start () {
	    _buttonActive = new bool[10];
	    _buttonBackground = new Texture[10];

	    foreach (var i in _buttonActive)
	    {
	        //i = false;
	    }
	}
	
	// Update is called once per frame
    void OnGUI()
    {
        GUI.skin = myCustomSkinThing;

        GUI.DrawTexture(new Rect(Screen.width / 2 - 160, Screen.height - 34, 32, 32), GuiItemBackgroundUp);
        GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 - 128, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 - 96, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 - 64, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 - 32, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 - 0, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 + 32, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 + 64, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 + 96, Screen.height - 34, 32, 32), "Bla");
        GUI.Button(new Rect(Screen.width / 2 + 128, Screen.height - 34, 32, 32), "Bla");

        
        GUI.DrawTexture(new Rect(Screen.width/2 - 180,Screen.height - 36,360,36), GuiBackground);

    }
}
