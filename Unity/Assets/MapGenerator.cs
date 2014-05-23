using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Transform targetFolder;
	public GameObject tilePrefab;
	public Sprite grassTile;

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
				GameObject cur = Instantiate(tilePrefab, new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity) as GameObject;
				cur.transform.parent = targetFolder;
				cur.name = "Tile-" + i + "x" + j;
				cur.SetActive(true);
				cur.GetComponent<SpriteRenderer>().sprite = grassTile;

				m_tileMap.setTile(i, j, 0, cur);
			}
		}
	}

	void generatePath()
	{
		//uint xStart = Random.Range(0, m_tileMap.getWidth() - 1);
		//uint yStart = Random.Range(0, m_tileMap.getHeight() - 1);

	}
}
