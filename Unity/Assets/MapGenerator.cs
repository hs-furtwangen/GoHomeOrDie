using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Transform targetFolder;
	public GameObject tilePrefab;
	public Sprite grassTile;
	public Sprite pathTile;
	public Sprite treeTile;

	private TileMap m_tileMap;

	private enum TileType
	{
		None,
		Grass,
		Path,
		Tree,
	}

	// Use this for initialization
	void Start () {
		m_tileMap = new TileMap (16, 32, 2);

		// For now just fill everything with the same tile
		for (uint i = 0; i < m_tileMap.getWidth(); i++)
		{
			for(uint j = 0; j < m_tileMap.getHeight(); j++)
			{
				createTileAt(i, j, 0, TileType.Grass);
			}
		}

		// Generate a path
		for(uint i = 0; i < m_tileMap.getWidth() + m_tileMap.getHeight();)
			i += generatePath();

		generateTrees ();
	}

	/**
	 * Generates a path and returns the amount of generated tiles
	 */
	uint generatePath()
	{
		uint amount = 1;

		uint xStart = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getWidth() - 1));
		uint yStart = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getHeight() - 1));

		createTileAt (xStart, yStart, 0, TileType.Path);

		uint lastX = xStart;
		uint lastY = yStart;

		float breakProbability = 1.0f / (m_tileMap.getWidth() + m_tileMap.getHeight());

		while (true) {
			uint newX = lastX;
			uint newY = lastY;
			for(int i = 0; i < 4 && newX >= 0 && newY >= 0 && newX < m_tileMap.getWidth() && newY < m_tileMap.getHeight() && checkTile(newX, newY, 0) == TileType.Path; i++)
			{
				// Generate new tile in one of the directions of the previous tile
				int dir = Mathf.RoundToInt(Random.Range(0, 3));
				Vector2 coord;
				switch(dir)
				{
				case 0:
					coord = m_tileMap.getTopLeftOf(lastX, lastY);
					newX = (uint)coord.x;
					newY = (uint)coord.y;
					break;
				case 1:
					coord = m_tileMap.getTopRightOf(lastX, lastY);
					newX = (uint)coord.x;
					newY = (uint)coord.y;
					break;
				case 2:
					coord = m_tileMap.getBottomLeftOf(lastX, lastY);
					newX = (uint)coord.x;
					newY = (uint)coord.y;
					break;
				case 3:
					coord = m_tileMap.getBottomRightOf(lastX, lastY);
					newX = (uint)coord.x;
					newY = (uint)coord.y;
					break;
				}
			}
			createTileAt (newX, newY, 0, TileType.Path);
			amount++;

			// Break?
			if(Random.Range(0.0f, 1.0f) < breakProbability)
				break;

			lastX = newX;
			lastY = newY;
		}
		return amount;
	}

	void generateTrees()
	{
		float treeProbability = 0;
		for (uint x = 0; x < m_tileMap.getWidth(); x++) {
			for(uint y = 0; y < m_tileMap.getHeight(); y ++) {
				if(checkTile(x, y, 0) == TileType.Grass)
					treeProbability = 0.1f;
				else if(checkTile(x, y, 0) == TileType.Path)
					treeProbability = 0;
				if(Random.Range(0.0f, 1.0f) < treeProbability)
					createTileAt(x, y, 1, TileType.Tree);
			}
		}
	}

	void createTileAt(uint x, uint y, uint z, TileType type)
	{
		Sprite curSprite = grassTile;
		switch (type) {
		case TileType.Grass:
			curSprite = grassTile;
			break;
		case TileType.Path:
			curSprite = pathTile;
			break;
		case TileType.Tree:
			curSprite = treeTile;
			break;
		}

		Vector2 screenPos = m_tileMap.map2Screen(x, y);
		screenPos.y += (curSprite.rect.height - m_tileMap.m_tileHeight) / m_tileMap.m_tileHeight * 0.25f; 

		GameObject cur = Instantiate(tilePrefab, new Vector3(screenPos.x, screenPos.y, -z), Quaternion.identity) as GameObject;
		cur.transform.parent = targetFolder;
		cur.name = "Tile-" + x + "x" + y + "x" + z + "_" + type.ToString();
		cur.SetActive(true);
		cur.GetComponent<SpriteRenderer>().sprite = curSprite;
		cur.GetComponent<SpriteRenderer>().sortingOrder = (int)(m_tileMap.getHeight() * z - y);
		
		m_tileMap.setTile(x, y, z, cur);
	}

	TileType checkTile(uint x, uint y, uint z)
	{
		Sprite spr = m_tileMap.getTile (x, y, 0).GetComponent<SpriteRenderer> ().sprite;
		if (spr == grassTile)
			return TileType.Grass;
		else if (spr == pathTile)
			return TileType.Path;
		else if (spr == treeTile)
			return TileType.Tree;
		else
			return TileType.None;
	}
}
