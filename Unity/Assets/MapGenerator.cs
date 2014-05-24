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
	public Sprite[] bankTopLeftTiles;
	public Sprite[] bankBottomTiles;
	public Sprite[] bankRightTiles;
	public Sprite[] bankBottomLeftRightTiles;
	public Sprite[] bankTopLeftBottomRightTiles;
	public bool bankPassability = false;
	
	private TileMap m_tileMap;

	private enum TileType
	{
		None,
		Grass,
		Path,
		Tree,
		Water,
		WaterDeep,
		Bank_TL,
		Bank_TR,
		Bank_BL,
		Bank_BR,
		Bank_Left,
		Bank_Right,
		Bank_Top,
		Bank_Bottom,
		Bank_BottomLeftRight,
		Bank_TopLeftRight,
		Bank_TopLeftBottomRight,
		Bank_BottomLeftTopRight,
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
		generateShores();

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

	void generateShores()
	{
		for (uint x = 0; x < m_tileMap.getWidth(); x++) {
			for(uint y = 0; y < m_tileMap.getHeight(); y++) {
				// If this is a water tile check if it is at the shore
				if(checkTile(x, y, 0) == TileType.Water)
				{
					Vector2 tl = m_tileMap.getTopLeftOf(x, y);
					Vector2 tr = m_tileMap.getTopRightOf(x, y);
					Vector2 bl = m_tileMap.getBottomLeftOf(x, y);
					Vector2 br = m_tileMap.getBottomRightOf(x, y);
					if(!isWater(x, y+2, 0) && !isBank(x, y+2, 0))
						createTileAt(x, y+2, 0, TileType.Bank_Bottom);
					if(!isWater(x, y-2, 0) && !isBank(x, y-2, 0))
						createTileAt(x, y-2, 0, TileType.Bank_Top);
					if(!isWater(x-1, y, 0) && !isBank(x-1, y, 0))
						createTileAt(x-1, y, 0, TileType.Bank_Right);
					if(!isWater(x+1, y, 0) && !isBank(x+1, y, 0))
						createTileAt(x+1, y, 0, TileType.Bank_Left);
					if(!isWater(tl, 0))
						createTileAt(tl, 0, TileType.Bank_TL);
					if(!isWater(tr, 0))
						createTileAt(tr, 0, TileType.Bank_TR);
					if(!isWater(bl, 0))
						createTileAt(tl, 0, TileType.Bank_BL);
					if(!isWater(br, 0))
						createTileAt(br, 0, TileType.Bank_BR);
				}
			}
		}
	}

	bool isWater(uint x, uint y, uint z)
	{
		return checkTile (x, y, z) != TileType.Water && checkTile (x, y, z) != TileType.WaterDeep;
	}

	bool isWater(Vector2 xy, uint z)
	{
		return isWater((uint)xy.x, (uint) xy.y, z);
	}

	bool isBank(uint x, uint y, uint z)
	{
		return checkTile (x, y, z) != TileType.Bank_BL && checkTile (x, y, z) != TileType.Bank_Bottom && checkTile (x, y, z) != TileType.Bank_BR && checkTile (x, y, z) != TileType.Bank_Left && checkTile (x, y, z) != TileType.Bank_Right && checkTile (x, y, z) != TileType.Bank_TL && checkTile (x, y, z) != TileType.Bank_Top && checkTile (x, y, z) != TileType.Bank_TR;
	}

	bool isBank(Vector2 xy, uint z)
	{
		return isBank ((uint)xy.x, (uint)xy.y, z);
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
		case TileType.Bank_TL:
			curSprite = bankTopLeftTiles[Random.Range (0, bankTopLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_TR:
			curSprite = bankTopLeftTiles[Random.Range (0, bankTopLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_BL:
			curSprite = bankTopLeftTiles[Random.Range (0, bankTopLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_BR:
			curSprite = bankTopLeftTiles[Random.Range (0, bankTopLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Top:
			curSprite = bankBottomTiles[Random.Range (0, bankBottomTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Bottom:
			curSprite = bankBottomTiles[Random.Range (0, bankBottomTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Left:
			curSprite = bankRightTiles[Random.Range (0, bankRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Right:
			curSprite = bankRightTiles[Random.Range (0, bankRightTiles.Length - 1)];
			passability = bankPassability;
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

		// Need to flip?
		if (type == TileType.Bank_TR)
			cur.GetComponent<Transform>().localScale = new Vector3(-1, 1, 1);
		if (type == TileType.Bank_BL)
			cur.GetComponent<Transform>().localScale = new Vector3(1, -1, 1);
		if (type == TileType.Bank_BR)
			cur.GetComponent<Transform>().localScale = new Vector3(-1, -1, 1);
		if (type == TileType.Bank_Top)
			cur.GetComponent<Transform>().localScale = new Vector3(1, -1, 1);
		if (type == TileType.Bank_Left)
			cur.GetComponent<Transform>().localScale = new Vector3(-1, 1, 1);
		
		m_tileMap.setTile(x, y, z, cur);
		m_tileMap.setPassability (x, y, passability);
	}

	void createTileAt(Vector2 xy, uint z, TileType type)
	{
		createTileAt ((uint)xy.x, (uint)xy.y, z, type);
	}

	TileType checkTile(uint x, uint y, uint z)
	{
		GameObject go = m_tileMap.getTile (x, y, 0);
		Sprite spr = go.GetComponent<SpriteRenderer> ().sprite;
		Transform tr = go.GetComponent<Transform> ();
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
		else if (System.Array.IndexOf (bankTopLeftTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == 1)
			return TileType.Bank_TL;
		else if (System.Array.IndexOf (bankTopLeftTiles, spr) > -1 && tr.localScale.x == -1 && tr.localScale.y == 1)
			return TileType.Bank_TR;
		else if (System.Array.IndexOf (bankTopLeftTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == -1)
			return TileType.Bank_BL;
		else if (System.Array.IndexOf (bankTopLeftTiles, spr) > -1 && tr.localScale.x == -1 && tr.localScale.y == -1)
			return TileType.Bank_BR;
		else if (System.Array.IndexOf (bankBottomTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == 1)
			return TileType.Bank_Bottom;
		else if (System.Array.IndexOf (bankBottomTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == -1)
			return TileType.Bank_Top;
		else if (System.Array.IndexOf (bankRightTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == 1)
			return TileType.Bank_Right;
		else if (System.Array.IndexOf (bankRightTiles, spr) > -1 && tr.localScale.x == -1 && tr.localScale.y == 1)
			return TileType.Bank_Left;
		else
			return TileType.None;
	}

	TileType checkTile(Vector2 xy, uint z)
	{
		return checkTile ((uint)xy.x, (uint)xy.y, z);
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
