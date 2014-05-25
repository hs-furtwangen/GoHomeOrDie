using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DawnTimer : MonoBehaviour {

	public double timeInSeconds = 10*60;
	public GameObject fogOfWar;
	public float lightBonus = 0;

	public GameObject Revealer;
	public Camera Camera;

	public GameObject BrightPlane;

	private double passedTime = 0;
	private float lightLevel = 1;
	private float startInnerRadius;
	private float startOuterRadius;

	private HashSet<int> pixToHide;
	private Color[] _colArr;

	private Vector3 _playerPos;

	private Texture2D blackTex;

	void Awake ()
	{
		blackTex = new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);

		var height = Camera.main.orthographicSize * 2.0f;
		var width = height * Screen.width / Screen.height;
		BrightPlane.transform.localScale = new Vector3(width, height, 0.1f);

		BrightPlane.renderer.material.mainTexture = blackTex;
	}
	
	// Use this for initialization
	void Start () {
		startInnerRadius = fogOfWar.GetComponent<FogOfWar>().RevInnerRadius;
		startOuterRadius = fogOfWar.GetComponent<FogOfWar>().RevRadius;
	}

	private void CutOutPlayer ()
	{
		// black out old pixels
		foreach (var pix in pixToHide) {
			_colArr[pix].a = 1;
		}

		pixToHide.Clear ();

		// draw new circle
		var playerPos = Revealer.transform.position;
		playerPos = Camera.WorldToScreenPoint (playerPos);

		var diffPos = playerPos - transform.position;
		
		var width = Screen.width;
		var height = Screen.height;
		
		var localPos = diffPos + new Vector3(width / 2.0f, -height / 2.0f, 0);
		
		var pixPosX = (int)((localPos.x / height) * height);
		var pixPosY = (int)-((localPos.y / width) * width);
		var pixPos = new Vector2(pixPosX, pixPosY);
				
		float inRad = fogOfWar.GetComponent<FogOfWar> ().RevInnerRadius;
		float outRad = inRad + 3;       

		for (var x = pixPosX - outRad; x <= pixPosX + outRad; x++)
			for (var y = pixPosY - outRad; y <= pixPosY + outRad; y++) {
				var pixInd = (int)(x * Screen.height + y);
				
				var posV = new Vector2(x, y);
				var dist = Vector2.Distance(posV, pixPos);
				
				if (dist <= outRad) {
					var alpha = 0f;
					
					if (dist > inRad)
						alpha = (dist - inRad) / (outRad - inRad);
					
					if (pixInd >= 0 && pixInd < _colArr.Length) {
						_colArr[pixInd].a = Mathf.Min(alpha, _colArr[pixInd].a);
						pixToHide.Add(pixInd);
					}
				}
			}

		blackTex.SetPixels(_colArr, 0);
		blackTex.Apply(false);
	}
	
	// Update is called once per frame
	void Update () {
		passedTime += Time.deltaTime;
		lightLevel = (float)(timeInSeconds - passedTime) / (float)timeInSeconds + lightBonus;
		fogOfWar.GetComponent<FogOfWar>().RevInnerRadius = (int) (startInnerRadius / lightLevel);
		fogOfWar.GetComponent<FogOfWar>().RevRadius = (int) (startOuterRadius / lightLevel);
		
		//guiTexture.color = new Color (0.1f, 0.1f, 0.1f, 0.75f - (lightLevel/2.0f));
		//guiTexture.color = new Color (1, 1, 1, 1);

		//CutOutPlayer ();


		if (lightLevel <= 0) {
			// TODO: GAME OVER!
		}
	}
}
