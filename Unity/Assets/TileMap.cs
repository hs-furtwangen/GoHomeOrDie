using UnityEngine;
using System.Collections;

public class TileMap {

	public uint m_tileWidth = 32;
	public uint m_tileHeight = 16;

	public TileMap(uint width, uint height, uint layers)
	{
		m_width = width;
		m_height = height;
		m_layers = layers;
		m_tiles = new GameObject[m_width,m_height,layers];
	}

	public uint getWidth()
	{
		return m_width;
	}

	public uint getHeight()
	{
		return m_height;
	}

	public void setTile(uint x, uint y, uint z, GameObject tile)
	{
		if (x < m_width && y < m_height && z < m_layers) {
			m_tiles[x, y, z] = tile;
		}
	}

	public GameObject getTile(uint x, uint y, uint z)
	{
		if (x < m_width && y < m_height && z < m_layers) {
			return m_tiles [x, y, z];
		} else
			return null;
	}

	public Vector2 map2Screen(uint x, uint y)
	{
		if (x < m_width && y < m_height) {
			float sX = x;
			float sY = y / 2.0f * (m_tileHeight / (float)m_tileWidth);
			if(y % 2 == 1)
				sX += 1.0f / 2.0f;
			Vector2 result = new Vector2(sX, sY);
			return result;
		} else
			return new Vector2();
	}

	public Vector2 screen2Map(float x, float y)
	{
		float mX = x;
		float mY = y * 2.0f * (m_tileHeight / (float)m_tileWidth);
		if (mY % 2 == 1)
			mX -= 1.0f / 2.0f;

		Vector2 result = new Vector2(mX, mY);
		return result;
	}


	private uint m_width;
	private uint m_height;
	private uint m_layers;
	private GameObject[,,] m_tiles;
}
