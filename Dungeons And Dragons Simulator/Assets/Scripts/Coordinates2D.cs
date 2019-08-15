using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates2D
{
	public readonly int x;
	public readonly int y;

	public Coordinates2D (int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	static public Coordinates2D convertToXY (int x, int y, int z)
	{
		return new Coordinates2D (x, y);
	}

	static public Coordinates2D convertToXY (Coordinates3D coordinates)
	{
		return new Coordinates2D (coordinates.x, coordinates.y);
	}

    static public Coordinates2D add (Coordinates2D a, Coordinates2D b)
    {
        return new Coordinates2D(a.x + b.x, a.y + b.y);
    }

	public class EqualityComparer : IEqualityComparer<Coordinates2D>
	{
		public bool Equals (Coordinates2D x, Coordinates2D y)
		{
			return (x.x == y.x && x.y == y.y);
		}

		public int GetHashCode (Coordinates2D x)
		{
			return ElegantSigned (x.x, x.y);
		}

		private int ElegantSigned (int x, int y)
		{
			if (x < 0) {
				if (y < 0) {
					return 3 + 4 * Elegant (-x - 1, -y - 1);
				}
				return 2 + 4 * Elegant (-x - 1, y);
			}
			if (y < 0) {
				return 1 + 4 * Elegant (x, -y - 1);
			}
			return 4 * Elegant (x, y);
		}

		private int Elegant (int x, int y)
		{
			if (x < y) {
				return y * y + x;
			} else {
				return x * x + x + y;
			}
		}
	}
}
