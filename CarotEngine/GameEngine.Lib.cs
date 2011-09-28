using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace pr2.CarotEngine {

partial class GameEngine {
	public static Vector2 ToVector2(Point pt) { return new Vector2(pt.X, pt.Y); }
	public static Vector3 ToVector3(Point pt) { return new Vector3(pt.X, pt.Y, 0); }
	public static Vector3 ToVector3(Vector2 pt) { return new Vector3(pt.X, pt.Y, 0); }
    public static Vector2[] ToVector2(Vector3[] pts)
    {
        Vector2[] ret = new Vector2[pts.Length];
        for (int i = 0; i < pts.Length; i++)
            ret[i] = new Vector2(pts[i].X, pts[i].Y);
        return ret;
    }

    public static Vector2[] ToVector2(Vector4[] pts)
    {
        Vector2[] ret = new Vector2[pts.Length];
        for (int i = 0; i < pts.Length; i++)
            ret[i] = new Vector2(pts[i].X, pts[i].Y);
        return ret;
    }

    public const float TwoPi = (float)Math.PI * 2;
	public const float Pi = (float)Math.PI;

	/// <summary>
	/// returns the angle from east to the provided point
	/// </summary>
	public static float AngleTo(Vector2 delta) {
		return (float)Math.Atan2(delta.Y, delta.X) - (float)Math.Atan2(1, 0);
	}
	
	public static Vector2 Floor(Vector2 v) {
		return new Vector2((int)v.X, (int)v.Y);
	}

	public static Point TopLeft(Rectangle rect) {
		return new Point(rect.Left, rect.Top);
	}

	public static Point Subtract(Point a, Point b) {
		return new Point(a.X-b.X, a.Y-b.Y);
	}

	public static float ManhattanDistance(Vector2 a, Vector2 b) {
		return Math.Abs(a.X-b.X) + Math.Abs(a.Y-b.Y);
	}

	public static int ManhattanDistance(Point a, Point b) {
		return Math.Abs(a.X-b.X) + Math.Abs(a.Y-b.Y);
	}

	public static float AngleTo(Vector2 a, Vector2 b) {
		b -= a;
		float dot = Microsoft.Xna.Framework.Vector2.Dot(Microsoft.Xna.Framework.Vector2.UnitX, b);
		return (float)Math.Acos(dot / b.Length());
	}

	public static bool IsPointInsideConvexPolygon(Vector2 pt, params Vector2[] polygon)
	{
		for(int i=0; i<polygon.Length; i++) {
			int a = i, b = i+1;
			if(b==polygon.Length) b=0;
			Vector2 x = (pt-polygon[a]);
			Vector2 y = (pt-polygon[b]);
			if(Vector3.Cross(ToVector3(x), ToVector3(y)).Z<0)
				return false;
		}
		return true;
	}

	/// <summary>
	/// Transforms a rectangle with the provided matrix
	/// </summary>
	public static Vector2[] Transform(Matrix mat, Rectangle rect) {
		Vector2[] ret = new Vector2[4] {
			new Vector2(rect.Left, rect.Top),
			new Vector2(rect.Right, rect.Top),
			new Vector2(rect.Right, rect.Bottom),
			new Vector2(rect.Left, rect.Bottom),
		};
		Vector2.Transform(ret, ref mat, ret);
		return ret;
	}

	/// <summary>
	/// Transforms a poly with the provided matrix
	/// </summary>
	public static Vector2[] Transform(Matrix mat, Vector2[] poly) {
		Vector2[] ret = (Vector2[])poly.Clone();
		Vector2.Transform(ret, ref mat, ret);
		return ret;
	}


    /// <summary>
    /// Transforms a poly with the provided matrix
    /// </summary>
    public static Vector3[] Transform(Matrix mat, Vector3[] poly)
    {
        Vector3[] ret = (Vector3[])poly.Clone();
        Vector3.Transform(ret, ref mat, ret);

        return ret;
    }

    /// <summary>
    /// Transforms a poly with the current matrices and viewport. Useful for perspective projection matrices
    /// </summary>
    public Vector3[] TransformCurrent(Vector3[] poly)
    {
        Vector3[] ret = (Vector3[])poly.Clone();
        for(int i=0;i<ret.Length;i++) {
            ret[i] = Device.Viewport.Project(ret[i], Matrices.projection, Matrices.view, Matrices.world);
        }
        return ret;
    }

    /// <summary>
    /// Transforms a poly with the provided matrix
    /// </summary>
    public static Vector4[] Transform(Matrix mat, Vector4[] poly)
    {
        Vector4[] ret = (Vector4[])poly.Clone();
        Vector4.Transform(ret, ref mat, ret);

        return ret;
    }

    /// <summary>
    /// extracts the position from a set of SimpleVertex items
    /// </summary>
    public static Vector3[] ExtractVector3(IEnumerable<SimpleVertex> verts)
    {
        List<Vector3> ret = new List<Vector3>();
        foreach (SimpleVertex sv in verts)
            ret.Add(sv.pos);
        return ret.ToArray();
    }

    /// <summary>
    /// extracts the position from a set of SimpleVertex items
    /// </summary>
    public static Vector4[] ExtractVector4(IEnumerable<SimpleVertex> verts)
    {
        List<Vector4> ret = new List<Vector4>();
        foreach (SimpleVertex sv in verts)
        {
            Vector4 v = new Vector4(sv.x, sv.y, sv.z, 1);
            ret.Add(v);
        }
        return ret.ToArray();
    }

	/// <summary>
	/// Transforms a point with the provided matrix
	/// </summary>
	public static Vector2 Transform(Matrix mat, Vector2 point) {
		return Vector2.Transform(point, mat);
	}

	public static Color RandomTestColor(object o) { return TestColors[(o.GetHashCode()/11)%TestColors.Length]; }
	public static Color RandomTestColor(object o, int salt) { return TestColors[((o.GetHashCode() / 11)^salt) % TestColors.Length]; }
	public static Color RandomTestColor(int salt) { return TestColors[salt % TestColors.Length]; }

	/// <summary>
	/// Colors suitable for testing, shuffled. All colors described as white, black, and gray have been removed
	/// </summary>
	public static Color[] TestColors = new Color[] {
		Color.Red,
		Color.BurlyWood,
		Color.CornflowerBlue,
		Color.Aquamarine,
		Color.LimeGreen,
		Color.DarkViolet,
		Color.SaddleBrown,
		Color.BlanchedAlmond,
		Color.Lavender,
		Color.RosyBrown,
		Color.DarkCyan,
		Color.DarkRed,
		Color.PowderBlue,
		Color.CadetBlue,
		Color.DarkOrchid,
		Color.Ivory,
		Color.PaleVioletRed,
		Color.Maroon,
		Color.LightCoral,
		Color.PaleGreen,
		Color.LightGreen,
		Color.MintCream,
		Color.LightPink,
		Color.Bisque,
		Color.Pink,
		Color.Turquoise,
		Color.DarkSeaGreen,
		Color.SeaShell,
		Color.MediumTurquoise,
		Color.Beige,
		Color.LightSeaGreen,
		Color.SpringGreen,
		Color.Plum,
		Color.Honeydew,
		Color.SandyBrown,
		Color.DarkMagenta,
		Color.MidnightBlue,
		Color.LightCyan,
		Color.DarkKhaki,
		Color.LightGoldenrodYellow,
		Color.YellowGreen,
		Color.Cornsilk,
		Color.PaleTurquoise,
		Color.Thistle,
		Color.Violet,
		Color.DodgerBlue,
		Color.LightYellow,
		Color.Fuchsia,
		Color.BlueViolet,
		Color.Magenta,
		Color.Green,
		Color.Silver,
		Color.OrangeRed,
		Color.DarkOrange,
		Color.MediumSpringGreen,
		Color.Moccasin,
		Color.Brown,
		Color.AliceBlue,
		Color.Purple,
		Color.MistyRose,
		Color.MediumPurple,
		Color.Tan,
		Color.DarkTurquoise,
		Color.Gainsboro,
		Color.Azure,
		Color.RoyalBlue,
		Color.Peru,
		Color.Tomato,
		Color.Blue,
		Color.LightBlue,
		Color.Linen,
		Color.SkyBlue,
		Color.Khaki,
		Color.PapayaWhip,
		Color.Aqua,
		Color.Snow,
		Color.Orange,
		Color.Teal,
		Color.DarkGoldenrod,
		Color.LightSalmon,
		Color.SteelBlue,
		Color.GreenYellow,
		Color.Chartreuse,
		Color.MediumSeaGreen,
		Color.LawnGreen,
		Color.Cyan,
		Color.Gold,
		Color.DarkOliveGreen,
		Color.Salmon,
		Color.DarkSlateBlue,
		Color.Lime,
		Color.PaleGoldenrod,
		Color.SeaGreen,
		Color.SlateBlue,
		Color.Wheat,
		Color.MediumSlateBlue,
		Color.Sienna,
		Color.Olive,
		Color.Yellow,
		Color.MediumVioletRed,
		Color.LemonChiffon,
		Color.Indigo,
		Color.MediumBlue,
		Color.MediumOrchid,
		Color.OldLace,
		Color.OliveDrab,
		Color.Navy,
		Color.DarkGreen,
		Color.Goldenrod,
		Color.PeachPuff,
		Color.LavenderBlush,
		Color.HotPink,
		Color.IndianRed,
		Color.DeepSkyBlue,
		Color.LightSteelBlue,
		Color.LightSkyBlue,
		Color.Orchid,
		Color.Firebrick,
		Color.Coral,
		Color.ForestGreen,
		Color.DarkSalmon,
		Color.MediumAquamarine,
		Color.Chocolate,
		Color.Crimson,
		Color.DeepPink,
		Color.DarkBlue,

		};


}

}