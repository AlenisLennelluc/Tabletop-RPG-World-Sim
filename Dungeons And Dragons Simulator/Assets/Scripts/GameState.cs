using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

	private HexList hexes;

    private GameObject currentlySelectedObject;

    public int centerX;
    public int centerY;

    public enum HexTypes
    {
        Water, Ice, Tundra, Grassland, Forest, Jungle, Desert, Hill, Mountain
    };

    private HexTypes currentHexType;

	void Awake ()
	{
		hexes = new HexList (FindObjectOfType<MapInit>());
	}

	// Use this for initialization
	void Start ()
	{
        
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public GameObject GetHex (Coordinates2D coordinates)
	{
		return hexes.getHex (coordinates);
	}

    public void EmptyHexes ()
    {
        List<GameObject> hexlist = hexes.GetHexes();

        hexes.EmptyHexes();

        foreach (GameObject hex in hexlist)
        {
            Destroy(hex);
        }
    }

    public List<GameObject> GetHexList()
    {
        return hexes.GetHexes();
    }

	public bool HasHex (Coordinates2D coordinates)
	{
		return hexes.ContainsHex (coordinates);
	}

	public bool HasHex (int x, int y)
	{
		return hexes.ContainsHex (new Coordinates2D (x, y));
	}

	public bool InsertHex (Coordinates2D coordinates, Hex hex)
	{
		return hexes.TryAddHex (coordinates, hex.gameObject);
	}

	public bool InsertHex (int x, int y, Hex hex)
	{
		return hexes.TryAddHex (new Coordinates2D (x, y), hex.gameObject);
	}

    public List<GameObject> getNeighbors(int x, int y, int range = 1)
    {
        return hexes.getNeighbors(x, y, range);
    }

    public GameObject GetSelectedObject ()
    {
        return currentlySelectedObject;
    }

    public void SetSelectedObject (GameObject objectIn)
    {
        currentlySelectedObject = objectIn;
        currentHexType = objectIn.GetComponent<Hex>().getType();
    }

    public HexTypes GetSelectedHexType ()
    {
        return currentHexType;
    }
}
