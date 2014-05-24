using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int mapWidth = 32;
	public int mapHeight = 64;
	public Transform targetFolder;
	public GameObject tilePrefab;
	public Transform itemFolder;
	public GameObject[] items;
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
	public Sprite[] bankTopRightTiles;
	public Sprite[] bankBottomLeftTiles;
	public Sprite[] bankBottomRightTiles;
	public Sprite[] bankTopTiles;
	public Sprite[] bankBottomTiles;
	public Sprite[] bankRightTiles;
	public Sprite[] bankLeftTiles;
	public Sprite[] bankBottomLeftRightTiles;
	public Sprite[] bankTopRightBottomRightTiles;
	public bool bankPassability = false;
	public Sprite[] stoneTiles;
	public bool stonePassability = true;
	
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
		Bank_TopRightBottomRight,
		Bank_TopLeftBottomLeft,
		Stone,
	}
	
	private enum DirectionFlags
	{
		Top = 1 << 0,
		Left = 1 << 1,
		Down = 1 << 2,
		Right = 1 << 3,
		TopLeft = 1 << 4,
		TopRight = 1 << 5,
		BottomLeft = 1 << 6,
		BottomRight = 1 << 7,
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

		// Generate lakes
		uint amountOfLakes = m_tileMap.getWidth () * m_tileMap.getHeight() / 1000;
		for(uint i = 0; i < amountOfLakes; i++)
			generateLake();
		generateDeepWater ();
		generateShores();

		// And trees!
		generateTrees ();

		// And stones!
		generateStones ();

		// And items!
		generateItems ();
	}

    public int GetTileZIndex(float posY)
    {
		return (int)(m_tileMap.getHeight() - m_tileMap.screen2Map(0, posY).y);
	}

	public bool isPassable(Vector2 screenPos)
	{
		var mapPos = m_tileMap.screen2Map(screenPos.x, screenPos.y);
		return m_tileMap.getPassability ((uint)mapPos.x, (uint)mapPos.y);
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
					treeProbability = 0.2f;
				if(Random.Range(0.0f, 1.0f) < treeProbability)
					createTileAt(x, y, 1, TileType.Tree);
				treeProbability = 0;
			}
		}
	}

	void generateStones()
	{
		for (uint x = 0; x < m_tileMap.getWidth(); x++) {
			for(uint y = 0; y < m_tileMap.getHeight(); y++) {
				if(checkTile(x, y, 1) == TileType.None)
				{
					float probability = 0.001f;
					if(checkTile(x, y, 0) == TileType.Grass)
						probability += 0.01f;
					if(Random.Range(0.0f, 1.0f) < probability)
						createTileAt(x, y, 1, TileType.Stone);
				}
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
				// If this is not a water tile check if it is located at the shore
				if(!isWater (x, y, 0) && !isBank(x, y, 0))
				{
					Vector2 tl = m_tileMap.getTopLeftOf(x, y);
					Vector2 tr = m_tileMap.getTopRightOf(x, y);
					Vector2 bl = m_tileMap.getBottomLeftOf(x, y);
					Vector2 br = m_tileMap.getBottomRightOf(x, y);
					Vector2 top = new Vector2(x, y+2);
					Vector2 left = new Vector2(x-1, y);
					Vector2 bottom = new Vector2(x, y-2);
					Vector2 right = new Vector2(x+1, y);
					int flags = 0;
					if(isWater(top, 0))
						flags |= (int)DirectionFlags.Down;
					if(isWater(tl, 0))
						flags |= (int)DirectionFlags.BottomRight;
					if(isWater(left, 0))
						flags |= (int)DirectionFlags.Right;
					if(isWater(bl, 0))
						flags |= (int)DirectionFlags.TopRight;
					if(isWater(bottom, 0))
						flags |= (int)DirectionFlags.Top;
					if(isWater(br, 0))
						flags |= (int)DirectionFlags.TopLeft;
					if(isWater(right, 0))
						flags |= (int)DirectionFlags.Left;
					if(isWater(tr, 0))
						flags |= (int)DirectionFlags.BottomLeft;
					TileType bankType = getBankTile(flags);
					if(bankType != TileType.None)
						createTileAt(x, y, 0, bankType);
				}
			}
		}
	}

	void generateItems()
	{
		for (uint i = 0; i < items.Length; i++) {
			// Generate random location
			bool created = false;
			do {
				uint x = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getWidth()));
				uint y = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getHeight()));
				if(m_tileMap.getPassability(x, y)) {
					Vector2 screenPos = m_tileMap.map2Screen(x, y);
					screenPos.y += (items[i].GetComponent<SpriteRenderer>().sprite.rect.height - m_tileMap.m_tileHeight) / m_tileMap.m_tileHeight * 0.25f; 

					GameObject cur = Instantiate(items[i], new Vector3(screenPos.x, screenPos.y, -1), Quaternion.identity) as GameObject;
					cur.transform.parent = itemFolder;
					cur.name = "Item-" + x + "x" + y;
					cur.SetActive(true);
					cur.GetComponent<SpriteRenderer>().sortingOrder = (int)(m_tileMap.getHeight() * 1 - y);
					created = true;
				}
			} while(!created);
		}
	}

	bool isWater(uint x, uint y, uint z)
	{
		return checkTile (x, y, z) == TileType.Water || checkTile (x, y, z) == TileType.WaterDeep;
	}

	bool isWater(Vector2 xy, uint z)
	{
		return isWater((uint)xy.x, (uint) xy.y, z);
	}

	bool isBank(uint x, uint y, uint z)
	{
		switch (checkTile (x, y, z)) {
		case TileType.Bank_BL:
		case TileType.Bank_Bottom:
		case TileType.Bank_BottomLeftRight:
		case TileType.Bank_BR:
		case TileType.Bank_Left:
		case TileType.Bank_Right:
		case TileType.Bank_TL:
		case TileType.Bank_Top:
		case TileType.Bank_TopLeftBottomLeft:
		case TileType.Bank_TopLeftRight:
		case TileType.Bank_TopRightBottomRight:
		case TileType.Bank_TR:
			return true;
		default:
			return false;
		}
	}

	bool isBank(Vector2 xy, uint z)
	{
		return isBank ((uint)xy.x, (uint)xy.y, z);
	}

	// flags needs to be ORed values of DirectionFlags
	TileType getBankTile(int flags)
	{
		if ((flags & (int)DirectionFlags.TopLeft) != 0 && (flags & (int)DirectionFlags.TopRight) != 0)
			return TileType.Bank_TopLeftRight;
		if ((flags & (int)DirectionFlags.TopLeft) != 0 && (flags & (int)DirectionFlags.BottomLeft) != 0)
			return TileType.Bank_TopLeftBottomLeft;
		if ((flags & (int)DirectionFlags.BottomLeft) != 0 && (flags & (int)DirectionFlags.BottomRight) != 0)
			return TileType.Bank_BottomLeftRight;
		if ((flags & (int)DirectionFlags.BottomRight) != 0 && (flags & (int)DirectionFlags.TopRight) != 0)
			return TileType.Bank_TopRightBottomRight;
		if ((flags & (int)DirectionFlags.TopLeft) != 0)
			return TileType.Bank_TL;
		if ((flags & (int)DirectionFlags.BottomLeft) != 0)
			return TileType.Bank_BL;
		if ((flags & (int)DirectionFlags.BottomRight) != 0)
			return TileType.Bank_BR;
		if ((flags & (int)DirectionFlags.TopRight) != 0)
			return TileType.Bank_TR;
		if ((flags & (int)(DirectionFlags.Top)) != 0)
			return TileType.Bank_Top;
		if ((flags & (int)DirectionFlags.Left) != 0)
			return TileType.Bank_Left;
		if ((flags & (int)DirectionFlags.Down) != 0)
			return TileType.Bank_Bottom;
		if ((flags & (int)DirectionFlags.Right) != 0)
			return TileType.Bank_Right;
		return TileType.None;
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
			curSprite = bankTopRightTiles[Random.Range (0, bankTopRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_BL:
			curSprite = bankBottomLeftTiles[Random.Range (0, bankBottomLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_BR:
			curSprite = bankBottomRightTiles[Random.Range (0, bankBottomRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Top:
			curSprite = bankTopTiles[Random.Range (0, bankTopTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Bottom:
			curSprite = bankBottomTiles[Random.Range (0, bankBottomTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Left:
			curSprite = bankLeftTiles[Random.Range (0, bankLeftTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_Right:
			curSprite = bankRightTiles[Random.Range (0, bankRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_BottomLeftRight:
			curSprite = bankBottomLeftRightTiles[Random.Range (0, bankBottomLeftRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_TopLeftRight:
			curSprite = bankBottomLeftRightTiles[Random.Range (0, bankRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_TopLeftBottomLeft:
			curSprite = bankTopRightBottomRightTiles[Random.Range (0, bankTopRightBottomRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Bank_TopRightBottomRight:
			curSprite = bankTopRightBottomRightTiles[Random.Range (0, bankTopRightBottomRightTiles.Length - 1)];
			passability = bankPassability;
			break;
		case TileType.Stone:
			curSprite = stoneTiles[Random.Range(0, stoneTiles.Length-1)];
			passability = stonePassability;
			break;
		default:
			curSprite = grassTiles[Random.Range(0, grassTiles.Length-1)];
			passability = grassPassability;
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
		if (type == TileType.Bank_TopLeftRight)
			cur.GetComponent<Transform>().localScale = new Vector3(1, -1, 1);
		if (type == TileType.Bank_TopLeftBottomLeft)
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
		GameObject go = m_tileMap.getTile(x, y, z);
		if (go == null)
			return TileType.None;

		Sprite spr = go.GetComponent<SpriteRenderer>().sprite;
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
		else if (System.Array.IndexOf (bankTopLeftTiles, spr) > -1)
			return TileType.Bank_TL;
		else if (System.Array.IndexOf (bankTopRightTiles, spr) > -1 )
			return TileType.Bank_TR;
		else if (System.Array.IndexOf (bankBottomLeftTiles, spr) > -1 )
			return TileType.Bank_BL;
		else if (System.Array.IndexOf (bankBottomRightTiles, spr) > -1)
			return TileType.Bank_BR;
		else if (System.Array.IndexOf (bankBottomTiles, spr) > -1)
			return TileType.Bank_Bottom;
		else if (System.Array.IndexOf (bankTopTiles, spr) > -1)
			return TileType.Bank_Top;
		else if (System.Array.IndexOf (bankRightTiles, spr) > -1)
			return TileType.Bank_Right;
		else if (System.Array.IndexOf (bankLeftTiles, spr) > -1)
			return TileType.Bank_Left;
		else if (System.Array.IndexOf (bankBottomLeftRightTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == 1)
			return TileType.Bank_BottomLeftRight;
		else if (System.Array.IndexOf (bankBottomLeftRightTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == -1)
			return TileType.Bank_TopLeftRight;
		else if (System.Array.IndexOf (bankTopRightBottomRightTiles, spr) > -1 && tr.localScale.x == 1 && tr.localScale.y == 1)
			return TileType.Bank_TopRightBottomRight;
		else if (System.Array.IndexOf (bankTopRightBottomRightTiles, spr) > -1 && tr.localScale.x == -1 && tr.localScale.y == 1)
			return TileType.Bank_TopLeftBottomLeft;
		else if(System.Array.IndexOf (stoneTiles, spr) > -1)
			return TileType.Stone;
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
