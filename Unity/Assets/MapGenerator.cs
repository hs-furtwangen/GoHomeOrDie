using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public GameObject targetFolder;
	public GameObject usingFolder;

	private TileMap m_tileMap;

	// Use this for initialization
	void Start () {
		m_tileMap = new TileMap (32, 32, 2);


//		_overlay[i, j] = Instantiate(TileOverlay, new Vector3(posXY.x, posXY.y, 0), Quaternion.identity) as GameObject;
//		_overlay[i, j].transform.parent = OverlayInstanciateTarget;
//		_overlay[i, j].name = "Tile-" + j + "x" + i;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
