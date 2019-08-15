using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInit : MonoBehaviour
{
    // Link to the hex prefab that we will manipulate
	public GameObject hexBase;

    // Link to the different textures for different land types
	public Material[] hexTypes;

    // Link to the gamestate
	private GameState gameState;

    // How zoomed in is the perlin map?
    // The more zoomed in the more gradual the changes
	private float heightResolution;

    // Spread the generated numbers over 0 and heightScale
    // instead of 0 and 1
	private float heightScale;

    // We will be grabbing 3 unrelated data values
    // so we want to grab them from different areas of the map
	private Vector2 perlinOffset;

    // Separate resolution value for temperature and humidity
	private float wetHeatResolution;

    // Separate scale value for temperature and humidity
	private float wetHeatScale;




	void Awake ()
	{
		//First: get a Perlin Map
		heightScale = 1.3f; // The scales are used to make percentages easier
		wetHeatScale = 1.2f;
		gameState = GameObject.FindGameObjectWithTag ("GameState").GetComponent<GameState> (); // Grab link to gameState
        NewMap();
    }

    public void NewMap()
    {
        heightResolution = Random.Range(.0075f, .015f); // Very small and tight, not a lot of change
        perlinOffset = new Vector2(Random.Range(30f, 60f), Random.Range(30f, 60f)); // Large numbers to spread out with
        wetHeatResolution = Random.Range(.02f, .04f); // Not as tight as height resolution, but change still wont be fast
    }

	// Use this for initialization
	void Start ()
	{
        // The initial map creation is started in gameState's start function,
        // so initializing here might be too late.
	}

	
	public Hex generateHex (int x, int y) // The x and y location of the hex on gameState's internal grid
	{
        // Double check there isn't a hex here already
		if (gameState.HasHex (x, y)) {
			throw new System.Exception ("Hex already exists.");
		}
        // Instantiate the hex
		GameObject hex = Instantiate (hexBase);
        // Grab a link to the hex's script component
		Hex newHex = hex.GetComponent<Hex> ();

        newHex.hexTypes = hexTypes;
        // Tell the hex where it is in the internal map
		newHex.SetLocation (x, y);
        // Use the y location to find where we are on the heat bands
		float heatBand = (float)(y * Mathf.PI) / (float)100;
        // Restrain that value between 1 and -1 reliably
        heatBand = Mathf.Sin(heatBand);
        // Grab the 3 x/y points we will be putting in the perlin map
        float heightX = (float)newHex.Position ().x * heightResolution + perlinOffset.x;
		float heightY = (float)newHex.Position ().z * heightResolution + perlinOffset.y;

		float wetX = (float)newHex.Position ().x * wetHeatResolution - perlinOffset.x;
		float wetY = (float)newHex.Position ().z * wetHeatResolution - perlinOffset.y;

		float heatX = (float)newHex.Position ().x * wetHeatResolution + perlinOffset.x * 2;
		float heatY = (float)newHex.Position ().z * wetHeatResolution + perlinOffset.y * 2;
        // Get the results from the perlin map
		float height = Mathf.PerlinNoise (heightX, heightY) * 2f - 1f;
		float wet = Mathf.PerlinNoise (wetX, wetY);
		float heat = Mathf.PerlinNoise (heatX, heatY);
        // Do some math to artificially create the heat bands, forcing hotter areas hotter ect.
        // (I dont remember quite how this works, would have to spend some time with it)
        float range = heatBand;
        if (range > .4)
        {
            heat = heat * .6f + .4f;
        }
        else if (range > .1)
        {
            float change = 1.1333f - 1.3333f * range;
            heat = heat * change + 1 - change;
        }
        heat = heat * range;
        range = (1 - range) / 2;
        heatBand = heatBand * range + range;
        heatBand = heatBand + heat;
        // Hand the hex it's height, humidity and temperature values
        newHex.height = height * heightScale;
		newHex.wetness = wet;
		newHex.heat = heatBand;
		//Debug.Log (newHex.height);
		//Debug.Log (newHex.wetness);
        // Grab a link to the hex's materials
        // Material[0] is a black border so we will be changing material[1]
		Material[] materials = hex.GetComponentInChildren<MeshRenderer> ().materials;
        // hex knows where it should be in the world based on it's x and y
		Vector3 vector = newHex.Position ();
		hex.transform.Translate (vector);
        // Set the hex as a child of this Map so Unity isn't cluttered
		hex.transform.SetParent (this.transform);

		// 0 = Tundra
		// 1 = Ice
		// 2 = Grassland
		// 3 = Forest
		// 4 = Desert
		// 5 = Jungle
		// 6 = Water
		// 7 = Hill
		// 8 = Mountain

		if (newHex.height < -.1) { // If new hex height is below water level it is water
			materials [1] = hexTypes [6];
            newHex.setType(GameState.HexTypes.Water);
		} else if (newHex.height < .9) { // else if it is below hill level check more
			if (newHex.heat < .1) { // If it is not hot..
				if (newHex.wetness < .4) { // or wet it is tundra
					materials [1] = hexTypes [0];
                    newHex.setType(GameState.HexTypes.Tundra);
                } else { // but if it is wet it is ice
					materials [1] = hexTypes [1];
                    newHex.setType(GameState.HexTypes.Ice);
                }
			} else if (newHex.heat < .7) { // Otherwise if the heat isn't too high...
				if (newHex.wetness < .5) { // and the wet isn't high it is grassland
					materials [1] = hexTypes [2];
                    newHex.setType(GameState.HexTypes.Grassland);
                } else { // but if the wet is high its a forest
					materials [1] = hexTypes [3];
                    newHex.setType(GameState.HexTypes.Forest);
                }
			} else { // Here the heat is over .7 == very hot
				if (newHex.wetness < .3) { // High heat but low wet: desert
					materials [1] = hexTypes [4];
                    newHex.setType(GameState.HexTypes.Desert);
                } else if (newHex.wetness < .5) { // High heat, moderate/low wet: more grassland
					materials [1] = hexTypes [2];
                    newHex.setType(GameState.HexTypes.Grassland);
                } else if (newHex.wetness < .7) { // High heat, moderate/high wet: more forest
                    materials [1] = hexTypes [3];
                    newHex.setType(GameState.HexTypes.Forest);
                } else { // High heat, high wet: jungle
					materials [1] = hexTypes [5];
                    newHex.setType(GameState.HexTypes.Jungle);
                }
			}
		} else if (newHex.height < 1) { // heights between .9 and 1 are hills
			materials [1] = hexTypes [7];
            newHex.setType(GameState.HexTypes.Hill);
        } else { // heights above 1 are mountains
			materials [1] = hexTypes [8];
            newHex.setType(GameState.HexTypes.Mountain);
        }
        // Hand the hex back it's new material array
		hex.GetComponentInChildren<MeshRenderer> ().materials = materials;
        // Name the hex with it's coords for debugging/clarification in unity
		hex.name = ("Hex " + x + ", " + y);

		if (!gameState.InsertHex (x, y, newHex)) { // This should not happen. Should be caught at the top
			GameObject.Destroy (hex);
			throw new System.Exception ("Hex already exists.");
		} else { // This should happen
			//Debug.Log ("Hex " + x + ", " + y + " was added to hexList");
		}
        // Return the newly minted hex
		return newHex;
	}
}
