﻿using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;
using System.Collections;

public class CreditsRoll : MonoBehaviour
{

    public Sprite[] CreditSprite;
    private float _timer;
    private int _pos;


	// Use this for initialization
	void Start ()
	{
        CreditSprite = new Sprite[6];
	    _timer = 3;
	    _pos = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _timer += Time.deltaTime;
	    if (_timer > 6)
	    {
            if (_pos == CreditSprite.Length)
	        GetComponent<SpriteRenderer>().sprite = CreditSprite[_pos];
	        _timer = 0;
	        _pos++;
	    }
        Debug.Log(_timer);
	}
}
