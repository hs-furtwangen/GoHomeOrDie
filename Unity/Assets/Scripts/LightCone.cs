using UnityEngine;
using System.Collections;

public class LightCone : MonoBehaviour {

	public GameObject player;
	public GameObject fogOfWar;

	// Use this for initialization
	void Start () {
		this.transform.position = player.transform.position;
		var radius = fogOfWar.GetComponent<FogOfWar>().RevInnerRadius;
		this.transform.localScale = new Vector3 (100.0f / radius, 100.0f / radius, 100.0f / radius);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = player.transform.position;
		var radius = fogOfWar.GetComponent<FogOfWar>().RevInnerRadius;
		this.transform.localScale = new Vector3 (100.0f / radius, 100.0f / radius, 100.0f / radius);
	}
}
