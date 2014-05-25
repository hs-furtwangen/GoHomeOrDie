using System.IO;
using UnityEngine;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{

    public Texture MessageBoxBackground;
    public GUISkin MessageBoxGuiSkin;
    private float _messageBoxAlpha;
    private int _fadeDirection;
    public float DisplayTimer;
    private float _displayTimerMultiplier;
    private float _currentFadeTimer;
    public float FadeSpeed;
    private bool _fadeing;
    private string _message;

	// Use this for initialization
	void Start ()
	{
	    _messageBoxAlpha = 0;
	    _fadeDirection = 1;
	    _fadeing = false;
	    _currentFadeTimer = DisplayTimer;
	}

    void Update()
    {
        if (_fadeing)
        {
            _messageBoxAlpha  = Mathf.Clamp(_messageBoxAlpha + FadeSpeed * _fadeDirection, 0, 1);
            if (_messageBoxAlpha == 1)
            {
                _currentFadeTimer = _currentFadeTimer - Time.deltaTime;
                if (_currentFadeTimer < 0)
                {
                    _fadeDirection *= -1;
                    _currentFadeTimer = DisplayTimer;
                }
                Debug.Log("FadeTimer: " + _currentFadeTimer);
            }
            else if (_messageBoxAlpha == 0)
            {
                _fadeDirection *= -1;
                _fadeing = false;
            }
            else
            {
                Debug.Log("Alpha: "+ _messageBoxAlpha);
            }
        }
    }

	// Update is called once per frame
	void OnGUI ()
	{
	    if (_messageBoxAlpha > 0)
	    {
	        GUI.color = new Color(1,1,1,_messageBoxAlpha);
	        GUI.skin = MessageBoxGuiSkin;
	        var messageBoxPosition = new Rect(Screen.width/2 - MessageBoxBackground.width/2, Screen.height - Screen.height/3, MessageBoxBackground.width, MessageBoxBackground.height);


	        GUI.DrawTexture(messageBoxPosition, MessageBoxBackground);
	        GUI.Label(messageBoxPosition, _message);
	    }
	}

    public void DisplayMessage(string msg, int durationMultiplier=1)
    {
        _currentFadeTimer = _currentFadeTimer*durationMultiplier;
        _fadeing = true;
        _message = msg;
    }

}
