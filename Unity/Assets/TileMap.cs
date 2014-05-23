using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {

	public uint m_tileWidth;
	public uint m_tileHeight;

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
			Quaternion rot = Quaternion.AngleAxis(Mathf.PI / 4, new Vector3(0, 0, 1));
			Vector3 vec3 = rot * new Vector3(x * m_tileWidth, y * m_tileHeight, 0);
			Vector2 result = new Vector2(vec3.x, vec3.y);
			return result;
		} else
			return new Vector2();
	}

	public Vector2 screen2Map(float x, float y)
	{
		Quaternion rot = Quaternion.AngleAxis(- Mathf.PI / 4, new Vector3(0, 0, 1));
		Vector3 vec3 = rot * new Vector3(x / m_tileWidth, y / m_tileHeight, 0);
		Vector2 result = new Vector2(vec3.x, vec3.y);
		return result;
	}


	private uint m_width;
	private uint m_height;
	private uint m_layers;
	private GameObject[,,] m_tiles;
}
