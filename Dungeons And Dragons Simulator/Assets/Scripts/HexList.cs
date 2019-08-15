using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexList
{
	private Dictionary<Coordinates2D, GameObject> hexList;
    private MapInit map;

	public HexList (MapInit map)
	{
        this.map = map;
        hexList = new Dictionary<Coordinates2D, GameObject> (new Coordinates2D.EqualityComparer ());
	}

    public List<GameObject> GetHexes()
    {
        List<GameObject> hexes = new List<GameObject>();

        foreach (KeyValuePair<Coordinates2D, GameObject> hex in hexList)
        {
            hexes.Add(hex.Value);
        }

        return hexes;
    }

	public List<GameObject> getNeighbors (Coordinates2D coordinates, int range = 1)
	{
		List<GameObject> results = new List<GameObject> ();

		for (int dx = -range; dx < range - 1; dx++) {
			for (int dy = Mathf.Max (-range + 1, -dx - range); dy < Mathf.Min (range, -dx + range - 1); dy++) {
				GameObject hex;

				if (hexList.TryGetValue (new Coordinates2D (coordinates.x + dx, coordinates.y + dy), out hex)) {
					results.Add (hex);
				}
                else
                {
                    hex = map.generateHex(coordinates.x + dx, coordinates.y + dy).gameObject;
                    results.Add(hex);
                }
			}
		}

		return results;
	}

	public List<GameObject> getNeighbors (int x, int y, int range = 1)
	{
		return getNeighbors (new Coordinates2D (x, y), range);
	}

	public GameObject getHex (Coordinates2D coordinates)
	{
		GameObject hex;
		if (hexList.TryGetValue (coordinates, out hex)) {
			return hex;
		} else {
            return map.generateHex(coordinates.x, coordinates.y).gameObject;
		}
	}

	public bool TryAddHex (Coordinates2D coordinates, GameObject hex)
	{
		if (!hexList.ContainsKey (coordinates)) {
			hexList.Add (coordinates, hex);
			return true;
		} else {
			return false;
		}
	}

	public bool TryAddHex (int x, int y, GameObject hex)
	{
		return TryAddHex (new Coordinates2D (x, y), hex);
	}

	public bool TryAddHex (Coordinates3D coordinates, GameObject hex)
	{
		return TryAddHex (Coordinates2D.convertToXY (coordinates), hex);
	}

	public bool ContainsHex (Coordinates2D coordinates)
	{
		return hexList.ContainsKey (coordinates);
	}

	public bool ContainsHex (int x, int y)
	{
		return ContainsHex (new Coordinates2D (x, y));
	}

    public void EmptyHexes()
    {
        hexList.Clear();
    }
}
