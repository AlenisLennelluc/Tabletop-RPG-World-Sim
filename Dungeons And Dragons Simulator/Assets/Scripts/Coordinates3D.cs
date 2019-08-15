using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates3D
{

	public readonly int x;
	public readonly int y;
	public readonly int z;

	//Rule: x + y + z = 0

	public Coordinates3D (int x, int y)
	{
		z = (x + y) * -1;

		this.x = x;
		this.y = y;
	}

	public Coordinates3D (int x, int y, int z)
	{
		if (x + y + z == 0) {
			this.x = x;
			this.y = y;
			this.z = z;
		} else {
			throw new System.Exception ("Given Coordinates are not valid.");
		}
	}

	static public Coordinates3D convertToXYZ (int x, int y)
	{
		return new Coordinates3D (x, y);
	}

	static public Coordinates3D convertToXYZ (Coordinates2D coordinates)
	{
		return new Coordinates3D (coordinates.x, coordinates.y);
	}

	public static Coordinates3D AddCoordinates (Coordinates3D left, Coordinates3D right)
	{
		Coordinates3D result = new Coordinates3D (left.x + right.x, left.y + right.y, left.z + right.z);
		return result;
	}

	public static Coordinates3D MultiplyCoordinates (Coordinates3D coordinates, int length)
	{
		Coordinates3D result = new Coordinates3D (coordinates.x * length, coordinates.y * length, coordinates.z * length);
		return result;
	}
}
