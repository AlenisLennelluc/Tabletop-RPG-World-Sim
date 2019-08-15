using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	public GameObject mouseManager;
	public GameObject mapManager;
	public GameObject cameraCenter;
	public GameObject unitPrefab;
	private MapInit initMap;
	private GameState gameState;
	private Dictionary <Coordinates2D, Hex> XYtoHex;
	private int range = 25;
	private int numberOfUnits;


	void Awake ()
	{
		XYtoHex = new Dictionary<Coordinates2D, Hex> (new Coordinates2D.EqualityComparer ());
		numberOfUnits = 0;

        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        initMap = mapManager.GetComponent(typeof(MapInit)) as MapInit;
    }

	// Use this for initialization
	void Start ()
	{
		//gameState = GameObject.FindGameObjectWithTag ("GameState").GetComponent<GameState> ();
		//initMap = mapManager.GetComponent (typeof(MapInit)) as MapInit;
		//InitMap ();
        

		//GameState (1);
	}
	
	// Maintains place in turn
	//		void GameState (int state)
	//		{
	//			switch (state) {
	//			case 1:
	//				{
	//					InitMap ();
	//
	//					GameState (2);
	//					break;
	//				}
	//			//case 2:
	//			}
	//		}

	public void InitMap ()
	{
        initMap.NewMap();
        XYtoHex.Clear();
		int searchRadius = 0;
		bool foundLand = false;
		Hex hex;
		Coordinates3D landFound = new Coordinates3D (0, 0);
		hex = initMap.generateHex (0, 0);
		XYtoHex.Add (new Coordinates2D (0, 0), hex);
		if (hex.height > 0 && hex.height < .8f && hex.wetness < .75f) {
			foundLand = true;
		}
		while (!foundLand) {
			if (searchRadius < 25) {
				searchRadius++;
			} else {
				break;
			}
			Coordinates3D[] ring = Hex.getRing (new Coordinates3D (0, 0), searchRadius);
			for (int i = 0; i < ring.Length; i++) {
				hex = initMap.generateHex (ring [i].x, ring [i].y);
				XYtoHex.Add (Coordinates2D.convertToXY (ring [i]), hex);
				if (hex.height > 0 && hex.height < .8f && hex.wetness < .75f) {
					foundLand = true;
					landFound = ring [i];
					break;
				}
			}
		}

		//cameraCenter.transform.position = hex.Position ();
		Hex startingHex = hex;
        gameState.centerX = startingHex.GetX();
        gameState.centerY = startingHex.GetY();

        FindObjectOfType<CameraControl>().UpdatePos(hex.transform.position.x, hex.transform.position.z);

		for (int dx = -range; dx < range - 1; dx++) {
			for (int dy = Mathf.Max (-range + 1, -dx - range); dy < Mathf.Min (range, -dx + range - 1); dy++) {
				if (!XYtoHex.ContainsKey (new Coordinates2D (dx + hex.GetX (), dy + hex.GetY ()))) {
					XYtoHex.Add (new Coordinates2D (dx + hex.GetX (), dy + hex.GetY ()), initMap.generateHex (dx + hex.GetX (), dy + hex.GetY ()));
				}
			}
		}

        Debug.Log("Map Initialized");

		//InitUnit (startingHex);
	}

	private void InitUnit (Hex land)
	{
		GameObject unit = Instantiate (unitPrefab);
		Unit unitScript = unit.GetComponent<Unit> ();
		unitScript.setUnitID (numberOfUnits);
		numberOfUnits++;
		unit.transform.SetParent (gameState.GetHex (Coordinates2D.convertToXY (land.GetCoordinates ())).transform);
		unit.transform.position = land.Position ();
	}

    public void LeftMouseUp (GameObject newObject)
    {
        GameObject selectedObject = gameState.GetSelectedObject();
        if (selectedObject != null && selectedObject.CompareTag("Hex"))
        {
            selectedObject.GetComponent<MeshRenderer>().materials[0].color = Color.black;
        }


        gameState.SetSelectedObject(newObject);

        MeshRenderer mesh = newObject.GetComponent<MeshRenderer>();

        mesh.materials[0].color = Color.cyan;
    }

    public void RightMouseDown (GameObject newObject)
    {

    }

    public void RightMouseUp (GameObject newObject)
    {
        GameObject selectedObject = gameState.GetSelectedObject();

        if (selectedObject != null && newObject.GetComponent<Hex>().getType() != GameState.HexTypes.Mountain && selectedObject.transform.childCount > 0)
        {
            selectedObject.GetComponent<MeshRenderer>().materials[0].color = Color.black;
            selectedObject = selectedObject.transform.GetChild(0).gameObject;
            selectedObject.transform.SetParent(newObject.transform);
            selectedObject.GetComponent<Unit>().ChangeHex(newObject.GetComponent<Hex>());
            selectedObject = null;
        }
    }
}
