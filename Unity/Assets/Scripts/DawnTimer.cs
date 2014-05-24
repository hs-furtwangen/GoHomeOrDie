using UnityEngine;
using System.Collections;

public class DawnTimer : MonoBehaviour {

	public double timeInSeconds = 10*60;
	public GameObject fogOfWar;
	public float lightBonus = 0;

	private double passedTime = 0;
	private float lightLevel = 1;
	private float startInnerRadius;
	private float startOuterRadius;

	// Use this for initialization
	void Start () {
		startInnerRadius = fogOfWar.GetComponent<FogOfWar>().RevInnerRadius;
		startOuterRadius = fogOfWar.GetComponent<FogOfWar>().RevRadius;
	}
	
	// Update is called once per frame
	void Update () {
		passedTime += Time.deltaTime;
		lightLevel = (float)(timeInSeconds - passedTime) / (float)timeInSeconds + lightBonus;
		fogOfWar.GetComponent<FogOfWar>().RevInnerRadius = Mathf.RoundToInt(startInnerRadius / lightLevel);
		fogOfWar.GetComponent<FogOfWar>().RevRadius = Mathf.RoundToInt(startOuterRadius / lightLevel);

		if (lightLevel <= 0) {
			// TODO: GAME OVER!
		}
	}
}
