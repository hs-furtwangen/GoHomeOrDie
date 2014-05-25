using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int mapWidth = 32;
	public int mapHeight = 64;
	public Transform targetFolder;
	public GameObject tilePrefab;
	public GameObject eventPrefab;
	public bool showEvents = false;
	public Transform itemFolder;
	public GameObject[] items;
	public Sprite[] grassTiles;
	public bool grassPassability = true;
	public Sprite[] pathCrossTiles;
	public Sprite[] pathTopLeftBottomRightTiles;
	public Sprite[] pathTopRightBottomLeftTiles;
	public Sprite[] pathTopLeftBottomLeftTiles;
	public Sprite[] pathTopLeftTopRightTiles;
	public Sprite[] pathTopRightBottomRightTiles;
	public Sprite[] pathBottomLeftBottomRightTiles;
	public Sprite[] pathTopLeftEndTiles;
	public Sprite[] pathTopRightEndTiles;
	public Sprite[] pathBottomLeftEndTiles;
	public Sprite[] pathBottomRightEndTiles;
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

	public enum TileType
	{
		None,
		Grass,
		PathCross,
		PathTopLeftBottomRight,
		PathTopRightBottomLeft,
		PathTopLeftBottomLeft,
		PathTopLeftTopRight,
		PathTopRightBottomRight,
		PathTopRightEnd,
		PathTopLeftEnd,
		PathBottomLeftBottomRight,
		PathBottomLeftEnd,
		PathBottomRightEnd,
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

		// And events!
		generateEvents ();
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

	public Vector2 getGridCoordinates(Vector2 screenPos)
	{
		return m_tileMap.screen2Map(screenPos.x, screenPos.y);
	}

	void generateEvents()
	{
		uint amountOfEvents = (uint)((m_tileMap.getWidth () * m_tileMap.getHeight ()) / 200.0f);

		for(uint i = 0; i < amountOfEvents; i++)
		{
			bool takeCoords = false;
			while(!takeCoords) {
				// Random coordinate
				uint x = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getWidth() - 1));
				uint y = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getHeight() - 1));

				// Bias towards grass tiles
				float probability = 0;
				if(checkTile(x, y, 0) == TileType.Grass)
					probability = 0.8f;
				if(checkTile(x, y, 0) == TileType.Water)
					probability = 0.2f;
				if(checkTile(x, y, 0) == TileType.PathCross)
					probability = 0.3f;

				if(Random.Range(0.0f, 1.0f) < probability)
				{
					takeCoords = true;
					Vector2 screenPos = m_tileMap.map2Screen(x, y);
					GameObject cur = Instantiate(eventPrefab, new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity) as GameObject;
					cur.transform.parent = targetFolder;
					cur.name = "Event-" + x + "x" + y;
					cur.SetActive(true);
					if(!showEvents)
						cur.GetComponent<SpriteRenderer>().sprite = null;
				}
			}
		}
	}

	/**
	 * Generates a path and returns the amount of generated tiles
	 */
	uint generatePath()
	{
		uint amount = 1;
		
		uint xStart = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getWidth() - 1));
		uint yStart = (uint)Mathf.RoundToInt(Random.Range(0, m_tileMap.getHeight() - 1));
		
		createTileAt (xStart, yStart, 0, TileType.PathCross);
		
		uint lastX = xStart;
		uint lastY = yStart;
		
		float breakProbability = 1.0f / (m_tileMap.getWidth() + m_tileMap.getHeight());
		
		while (true) {
			uint newX = lastX;
			uint newY = lastY;
			for(int i = 0; i < 4 && newX >= 0 && newY >= 0 && newX < m_tileMap.getWidth() && newY < m_tileMap.getHeight() && checkTile(newX, newY, 0) == TileType.PathCross; i++)
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
			createTileAt (newX, newY, 0, TileType.PathCross);
			amount++;
			
			// Break?
			if(Random.Range(0.0f, 1.0f) < breakProbability)
				break;
			
			lastX = newX;
			lastY = newY;
		}
		return amount;
	}

	TileType getPathFromTo(uint from, uint to)
	{
		if (from == 0 && to == 1)
			return TileType.PathTopLeftTopRight;
		if(from == 0 && to == 2)
			return TileType.PathTopLeftBottomLeft;
		if(from == 0 && to == 3)
			return TileType.PathTopLeftBottomRight;
		if(from == 1 && to == 2)
			return TileType.PathTopRightBottomLeft;
		if(from == 1 && to == 3)
			return TileType.PathTopRightBottomRight;
		if(from == 2 && to == 3)
			return TileType.PathBottomLeftBottomRight;
		return TileType.None;
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
		case TileType.PathCross:
			curSprite = pathCrossTiles[Random.Range(0, pathCrossTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopLeftEnd:
			curSprite = pathTopLeftEndTiles[Random.Range(0, pathTopLeftEndTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopRightEnd:
			curSprite = pathTopRightEndTiles[Random.Range(0, pathTopRightEndTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathBottomLeftEnd:
			curSprite = pathBottomLeftEndTiles[Random.Range(0, pathBottomLeftEndTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathBottomRightEnd:
			curSprite = pathBottomRightEndTiles[Random.Range(0, pathBottomRightEndTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopRightBottomLeft:
			curSprite = pathTopRightBottomLeftTiles[Random.Range(0, pathTopRightBottomLeftTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopRightBottomRight:
			curSprite = pathTopRightBottomRightTiles[Random.Range(0, pathTopRightBottomRightTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopLeftBottomLeft:
			curSprite = pathTopLeftBottomLeftTiles[Random.Range(0, pathTopLeftBottomLeftTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopLeftBottomRight:
			curSprite = pathTopLeftBottomRightTiles[Random.Range(0, pathTopLeftBottomRightTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathTopLeftTopRight:
			curSprite = pathTopLeftTopRightTiles[Random.Range(0, pathTopLeftTopRightTiles.Length-1)];
			passability = pathPassability;
			break;
		case TileType.PathBottomLeftBottomRight:
			curSprite = pathBottomLeftBottomRightTiles[Random.Range(0, pathBottomLeftBottomRightTiles.Length-1)];
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

	public TileType checkTile(uint x, uint y, uint z)
	{
		GameObject go = m_tileMap.getTile(x, y, z);
		if (go == null)
			return TileType.None;

		Sprite spr = go.GetComponent<SpriteRenderer>().sprite;
		Transform tr = go.GetComponent<Transform> ();
		if (System.Array.IndexOf (grassTiles, spr) > -1)
			return TileType.Grass;
		else if (System.Array.IndexOf (pathCrossTiles, spr) > -1)
			return TileType.PathCross;
		else if (System.Array.IndexOf (pathTopLeftBottomLeftTiles, spr) > -1)
			return TileType.PathTopLeftBottomLeft;
		else if (System.Array.IndexOf (pathTopLeftBottomRightTiles, spr) > -1)
			return TileType.PathTopLeftBottomRight;
		else if (System.Array.IndexOf (pathTopLeftTopRightTiles, spr) > -1)
			return TileType.PathTopLeftTopRight;
		else if (System.Array.IndexOf (pathTopRightBottomLeftTiles, spr) > -1)
			return TileType.PathTopRightBottomLeft;
		else if (System.Array.IndexOf (pathTopRightBottomRightTiles, spr) > -1)
			return TileType.PathTopRightBottomRight;
		else if (System.Array.IndexOf (pathBottomLeftBottomRightTiles, spr) > -1)
			return TileType.PathBottomLeftBottomRight;
		else if (System.Array.IndexOf (pathTopLeftEndTiles, spr) > -1)
			return TileType.PathTopLeftEnd;
		else if (System.Array.IndexOf (pathTopRightEndTiles, spr) > -1)
			return TileType.PathTopRightEnd;
		else if (System.Array.IndexOf (pathBottomLeftEndTiles, spr) > -1)
			return TileType.PathBottomLeftEnd;
		else if (System.Array.IndexOf (pathBottomRightEndTiles, spr) > -1)
			return TileType.PathBottomRightEnd;
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

	public TileType checkTile(Vector2 xy, uint z)
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
