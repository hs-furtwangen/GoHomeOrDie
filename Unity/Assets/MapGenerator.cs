using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Transform targetFolder;
	public Transform usingFolder;
	public GameObject testTile;

	private TileMap m_tileMap;

	// Use this for initialization
	void Start () {
		m_tileMap = new TileMap (32, 32, 2);

		// For now just fill everything with the same tile
		for (uint i = 0; i < m_tileMap.getWidth(); i++)
		{
			for(uint j = 0; j < m_tileMap.getHeight(); j++)
			{
				Vector2 screenPos = m_tileMap.map2Screen(i, j);
				GameObject cur = Instantiate(testTile, new Vector3(screenPos.x, screenPos.y, 0), Quaternion.AngleAxis(-45, new Vector3(0, 0, 1))) as GameObject;
				cur.transform.parent = targetFolder;
				cur.name = "Tile-" + i + "x" + j;
				cur.SetActive(true);

				m_tileMap.setTile(i, j, 0, cur);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
