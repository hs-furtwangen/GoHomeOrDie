using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int mapWidth = 32;
	public int mapHeight = 64;
	public Transform targetFolder;
	public GameObject tilePrefab;
	public Sprite[] grassTiles;
	public bool grassPassability = true;
	public Sprite[] pathTiles;
	public bool pathPassability = true;
	public Sprite[] treeTiles;
	public bool treePassability = false;
	public Sprite[] waterTiles;
	public bool waterPassability = false;
	public Sprite[] deepWaterTiles;
	public bool deepWaterPassability = false;

	private TileMap m_tileMap;

	private enum TileType
	{
		None,
		Grass,
		Path,
		Tree,
		Water,
		WaterDeep,
	}

	// Use this for initialization
	void Start () {
		m_tileMap = new TileMap ((uint)mapWidth, (uint)mapHeight, 2);

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

		// Generate a lake
		generateLake ();
		generateDeepWater ();

		// And trees!
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
			for(uint y = 0; y < m_tileMap.getHeight(); y++) {
				if(checkTile(x, y, 0) == TileType.Grass)
					treeProbability = 0.11f;
				if(Random.Range(0.0f, 1.0f) < treeProbability)
					createTileAt(x, y, 1, TileType.Tree);
				treeProbability = 0;
			}
		}
	}

	void generateLake()
	{
		uint xStart;
		uint yStart;
		do {
			xStart = (uint)Mathf.RoundToInt (Random.Range (0, m_tileMap.getWidth () - 1));
			yStart = (uint)Mathf.RoundToInt (Random.Range (0, m_tileMap.getHeight () - 1));
		} while(checkTile(xStart, yStart, 0) != TileType.Grass);
		generateLake (xStart, yStart, 1.0f);
	}

	void generateLake(uint x, uint y, float probability)
	{
		createTileAt(x, y, 0, TileType.Water);
		// Every adjacent tile has a certain probability to be a water tile as well
		// Top left
		if (Random.Range (0.0f, 1.0f) < probability) {
			Vector2 coord = m_tileMap.getTopLeftOf (x, y);
			if(isCoordValid(coord) && checkTile((uint)coord.x, (uint)coord.y, 0) == TileType.Grass)
			{
				createTileAt((uint)coord.x, (uint)coord.y, 0, TileType.Water);
				generateLake((uint)coord.x, (uint)coord.y, probability - 0.1f);
			}
		}
		// Top right
		if (Random.Range (0.0f, 1.0f) < probability) {
			Vector2 coord = m_tileMap.getTopRightOf (x, y);
			if(isCoordValid(coord) && checkTile((uint)coord.x, (uint)coord.y, 0) == TileType.Grass)
			{
				createTileAt((uint)coord.x, (uint)coord.y, 0, TileType.Water);
				generateLake((uint)coord.x, (uint)coord.y, probability - 0.1f);
			}
		}
		// Bottom left
		if (Random.Range (0.0f, 1.0f) < probability) {
			Vector2 coord = m_tileMap.getBottomLeftOf (x, y);
			if(isCoordValid(coord) && checkTile((uint)coord.x, (uint)coord.y, 0) == TileType.Grass)
			{
				createTileAt((uint)coord.x, (uint)coord.y, 0, TileType.Water);
				generateLake((uint)coord.x, (uint)coord.y, probability - 0.1f);
			}
		}
		// Bottom right
		if (Random.Range (0.0f, 1.0f) < probability) {
			Vector2 coord = m_tileMap.getBottomRightOf (x, y);
			if(isCoordValid(coord) && checkTile((uint)coord.x, (uint)coord.y, 0) == TileType.Grass)
			{
				createTileAt((uint)coord.x, (uint)coord.y, 0, TileType.Water);
				generateLake((uint)coord.x, (uint)coord.y, probability - 0.1f);
			}
		}
	}

	void generateDeepWater()
	{
		for (uint x = 0; x < m_tileMap.getWidth(); x++) {
			for(uint y = 0; y < m_tileMap.getHeight(); y++) {
				if (checkTile(x, y, 0) == TileType.Water)
				{
					Vector2 tl = m_tileMap.getTopLeftOf(x, y);
					Vector2 tr = m_tileMap.getTopRightOf(x, y);
					Vector2 bl = m_tileMap.getBottomLeftOf(x, y);
					Vector2 br = m_tileMap.getBottomRightOf(x, y);
					if((checkTile((uint)tl.x, (uint)tl.y, 0) == TileType.Water || checkTile((uint)tl.x, (uint)tl.y, 0) == TileType.WaterDeep)
					   && (checkTile((uint)tr.x, (uint)tr.y, 0) == TileType.Water || checkTile((uint)tr.x, (uint)tr.y, 0) == TileType.WaterDeep)
					   && (checkTile((uint)bl.x, (uint)bl.y, 0) == TileType.Water || checkTile((uint)bl.x, (uint)bl.y, 0) == TileType.WaterDeep)
					   && (checkTile((uint)br.x, (uint)br.y, 0) == TileType.Water || checkTile((uint)br.x, (uint)br.y, 0) == TileType.WaterDeep))
						createTileAt(x, y, 0, TileType.WaterDeep);
				}
			}
		}
	}

	void createTileAt(uint x, uint y, uint z, TileType type)
	{
		Sprite curSprite = grassTiles[0];
		bool passability = true;
		switch (type) {
		case TileType.Grass:
			curSprite = grassTiles[Random.Range(0, grassTiles.Length-1)];
			passability = grassPassability;
			break;
		case TileType.Path:
			curSprite = pathTiles[Random.Range(0, pathTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.Tree:
			curSprite = treeTiles[Random.Range(0, treeTiles.Length-1)];
			passability = treePassability;
			break;
		case TileType.Water:
			curSprite = waterTiles[Random.Range (0, waterTiles.Length - 1)];
			passability = waterPassability;
			break;
		case TileType.WaterDeep:
			curSprite = deepWaterTiles[Random.Range (0, deepWaterTiles.Length - 1)];
			passability = deepWaterPassability;
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
		m_tileMap.setPassability (x, y, passability);
	}

	TileType checkTile(uint x, uint y, uint z)
	{
		Sprite spr = m_tileMap.getTile (x, y, 0).GetComponent<SpriteRenderer> ().sprite;
		if (System.Array.IndexOf (grassTiles, spr) > -1)
						return TileType.Grass;
				else if (System.Array.IndexOf (pathTiles, spr) > -1)
						return TileType.Path;
				else if (System.Array.IndexOf (treeTiles, spr) > -1)
						return TileType.Tree;
				else if (System.Array.IndexOf (waterTiles, spr) > -1)
						return TileType.Water;
				else if (System.Array.IndexOf (deepWaterTiles, spr) > -1)
						return TileType.WaterDeep;
		else
			return TileType.None;
	}

	bool isCoordValid(Vector2 coord)
	{
		return coord.x >= 0 && coord.x < m_tileMap.getWidth() && coord.y >= 0 && coord.y < m_tileMap.getHeight();
	}

	bool isCoordValid(int x, int y, int z)
	{
		return x >= 0 && x < m_tileMap.getWidth() && y >= 0 && y < m_tileMap.getHeight() && z >= 0 && z < m_tileMap.getLayers();
	}
}
