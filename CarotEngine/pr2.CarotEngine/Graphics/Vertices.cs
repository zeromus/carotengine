using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	/// <summary>
	/// position 2d only
	/// </summary>
	public struct VertexVector2 {
		public float x { get { return pos.X; } set { pos.X = value; } }
		public float y { get { return pos.Y; } set { pos.Y = value; } }
		public Vector2 pos;

		public static VertexDeclaration Decl;
		public static readonly int SizeInBytes = 8;
		public static readonly VertexElement[] VertexElements = new VertexElement[]{
			new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
		};

		public VertexVector2(Vector2 pos) {
			this.pos = pos;
		}

		public VertexVector2(float x, float y) {
			pos = new Vector2(x, y);
		}
	}

	/// <summary>
	/// position, color, tex2d
	/// no w-coord
	/// </summary>
	public struct SimpleVertex : IVertexType
	{
		public Vector3 pos;
		public Color col;
		public Vector2 tex;

		public float x { get { return pos.X; } set { pos.X = value; } }
		public float y { get { return pos.Y; } set { pos.Y = value; } }
		public float z { get { return pos.Z; } set { pos.Z = value; } }
		public float tu { get { return tex.X; } set { tex.X = value; } }
		public float tv { get { return tex.Y; } set { tex.Y = value; } }

		public static VertexDeclaration Decl;
		public static readonly int SizeInBytes = 24;
		public static readonly VertexElement[] VertexElements = new VertexElement[]{
                new VertexElement(0,VertexElementFormat.Vector3,VertexElementUsage.Position,0),
                new VertexElement(12,VertexElementFormat.Color,VertexElementUsage.Color,0),
                new VertexElement(16,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,0)};

		VertexDeclaration IVertexType.VertexDeclaration { get { return Decl; } }

		public SimpleVertex(float x, float y, float z) {
			pos = new Vector3(x, y, z);
			col = Color.Transparent;
			tex = new Vector2(0, 0);
		}

		public SimpleVertex(float x, float y, float tu, float tv) {
			pos = new Vector3(x, y, 0);
			col = Color.Transparent;
			tex = new Vector2(tu, tv);
		}
	}


}