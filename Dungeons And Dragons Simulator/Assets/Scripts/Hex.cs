using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{

	public enum HexDirections
	{
		UPLEFT,
		UPRIGHT,
		LEFT,
		RIGHT,
		DOWNLEFT,
		DOWNRIGHT
	}

    

	// X + Y + Z = 0
	// Z = -(X + Y)

	private int X;
    private int Y;
    private int Z;
    private bool positionSet;
    public float radius;
	public float height;
	public float wetness;
    public float heat;

    private bool growable;

    private float growthTimer;

    private GameState.HexTypes hexType;
    private GameState.HexTypes maxType;

	static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt (3) / 2;

	public bool allowWrapEastWest = false;
	public bool allowWrapNorthSouth = false;

	public GameObject unitInHex;
    public Material[] hexTypes;


    public void Step()
    {
        if (growable)
        {
            growthTimer += 1;
            switch (hexType)
            {
                case GameState.HexTypes.Desert:
                    growthTimer -= 0.5f;
                    if (growthTimer > 20)
                    {
                        hexType = GameState.HexTypes.Grassland;
                        Material[] materials = GetComponentInChildren<MeshRenderer>().materials;
                        materials[1] = hexTypes[2];
                        GetComponentInChildren<MeshRenderer>().materials = materials;
                    }
                    break;
                case GameState.HexTypes.Grassland:
                    if (growthTimer > 50 && maxType != GameState.HexTypes.Grassland)
                    {
                        hexType = GameState.HexTypes.Forest;
                        Material[] materials = GetComponentInChildren<MeshRenderer>().materials;
                        materials[1] = hexTypes[3];
                        GetComponentInChildren<MeshRenderer>().materials = materials;
                    }
                    break;
                case GameState.HexTypes.Forest:
                    if (growthTimer > 300 && maxType != GameState.HexTypes.Forest)
                    {
                        hexType = GameState.HexTypes.Jungle;
                        Material[] materials = GetComponentInChildren<MeshRenderer>().materials;
                        materials[1] = hexTypes[5];
                        GetComponentInChildren<MeshRenderer>().materials = materials;
                    }
                    break;
                default:
                    break;
            }
        }
    }


    public float Gather()
    {
        float reward = 0;
        Material[] materials;

        switch (hexType)
        {
            case GameState.HexTypes.Grassland:
                reward = growthTimer;
                growthTimer = 0;
                hexType = GameState.HexTypes.Desert;
                materials = GetComponentInChildren<MeshRenderer>().materials;
                materials[1] = hexTypes[4];
                GetComponentInChildren<MeshRenderer>().materials = materials;

                break;
            case GameState.HexTypes.Forest:
                reward = growthTimer * 2;
                growthTimer = 20;
                hexType = GameState.HexTypes.Grassland;
                materials = GetComponentInChildren<MeshRenderer>().materials;
                materials[1] = hexTypes[2];
                GetComponentInChildren<MeshRenderer>().materials = materials;

                break;
            case GameState.HexTypes.Jungle:
                reward = growthTimer * 3;
                growthTimer = 50;
                hexType = GameState.HexTypes.Forest;
                materials = GetComponentInChildren<MeshRenderer>().materials;
                materials[1] = hexTypes[3];
                GetComponentInChildren<MeshRenderer>().materials = materials;

                break;
            default:
                break;
        }

        return reward;
    }


    /// <summary>
    /// The initialization function.
    /// Run this before interacting with the object!
    /// </summary>
    /// <param name="x coordinate"></param>
    /// <param name="y coordiante"></param>
    public void SetLocation(int x, int y)
    {
        if (positionSet == false)
        {
            this.X = x;
            this.Y = y;
            this.Z = -x - y;
            positionSet = true;
        }
        else
        {
            Debug.Log("Position already set too " + X + ", " + Y + ", " + Z);
        }
        radius = 1f;
    }

	public Vector3 Position ()
	{
        return new Vector3 (HexHorizontalSpacing () * (this.X + this.Y / 2f), 0, HexVerticalSpacing () * this.Y);
	}

	public float HexHeight ()
	{
		return radius * 2;
	}

	public float HexWidth ()
	{
		return WIDTH_MULTIPLIER * HexHeight ();
	}

	public float HexVerticalSpacing ()
	{
		return HexHeight () * 0.75f;
	}

	public float HexHorizontalSpacing ()
	{
		return HexWidth ();
	}

	public Vector3 PositionFromCamera (Vector3 cameraPosition, float numRows, float numColumns)
	{
		float mapHeight = numRows * HexVerticalSpacing ();
		float mapWidth = numColumns * HexHorizontalSpacing ();


		return new Vector3 (0, 0, 0);
	}

	public Coordinates3D GetNeighbor (HexDirections direction)
	{
		return this.addCoordinates (NeighborDirections (direction));
	}

	public Coordinates3D GetCoordinates ()
	{
		return new Coordinates3D (X, Y);
	}

	private static Coordinates3D NeighborDirections (HexDirections direction)
	{
		switch (direction) {
		case HexDirections.DOWNLEFT:
			return new Coordinates3D (-1, 0, 1);
		case HexDirections.DOWNRIGHT:
			return new Coordinates3D (0, -1, 1);
		case HexDirections.LEFT:
			return new Coordinates3D (-1, 1, 0);
		case HexDirections.RIGHT:
			return new Coordinates3D (1, -1, 0);
		case HexDirections.UPLEFT:
			return new Coordinates3D (0, 1, -1);
		case HexDirections.UPRIGHT:
			return new Coordinates3D (1, 0, -1);
		}
		return new Coordinates3D (0, 0);
	}

	private static Coordinates3D NeighborDirections (int direction)
	{
		switch (direction) {
		case 0:
			return new Coordinates3D (1, -1, 0);
		case 1:
			return new Coordinates3D (1, 0, -1);
		case 2:
			return new Coordinates3D (0, 1, -1);
		case 3:
			return new Coordinates3D (-1, 1, 0);
		case 4:
			return new Coordinates3D (-1, 0, 1);
		case 5:
			return new Coordinates3D (0, -1, 1);
		}
		return new Coordinates3D (0, 0);
	}

	private Coordinates3D addCoordinates (Coordinates3D coordinates)
	{
		Coordinates3D newCoords = new Coordinates3D (coordinates.x + this.X, coordinates.y + this.Y, coordinates.z + this.Z);

		return newCoords;
	}

	public static Coordinates3D[] getRing (Coordinates3D center, int radius)
	{
		int size = radius * 6;
		Coordinates3D[] ring = new Coordinates3D[size];
		Coordinates3D hex = Coordinates3D.AddCoordinates (center, 
			                    Coordinates3D.MultiplyCoordinates (Hex.NeighborDirections (HexDirections.DOWNLEFT), radius));
		int num = 0;
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < radius; j++) {
				ring [num] = hex;
				num++;
				hex = Coordinates3D.AddCoordinates (hex, Hex.NeighborDirections (i));
			}
		}

		return ring;
	}

    public int GetX()
    {
        return X;
    }

    public int GetY()
    {
        return Y;
    }

    public int GetZ()
    {
        return Z;
    }

    public void setType(GameState.HexTypes type)
    {
        hexType = type;
        maxType = type;

        switch (hexType)
        {
            case GameState.HexTypes.Grassland:
                growable = true;
                growthTimer = 20;
                break;
            case GameState.HexTypes.Forest:
                growable = true;
                growthTimer = 50;
                break;
            case GameState.HexTypes.Jungle:
                growable = true;
                growthTimer = 300;
                break;
            default:
                growable = false;
                break;
        }
    }

    public GameState.HexTypes getType()
    {
        return hexType;
    }
}
