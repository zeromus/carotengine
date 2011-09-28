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

	public class CircleMesh {
		public enum CircleType {
			Filled, Outline
		}
		CircleType type;
		GameEngine game;
		int segments;
		public CircleMesh(GameEngine game, CircleType type, int segments) {
			this.segments = segments;
			this.type = type;
			this.game = game;

			int vtxstart = 0, vtxcount;
			if(type == CircleType.Filled)
				vtxcount = segments+2;
			//vertices[0] = new SimpleVertex(0, 0, 0.5f, 0.5f);
			else
				vtxcount=segments+1;

			SimpleVertex[] vertices = new SimpleVertex[vtxcount];

			for(int i=0; i<segments; i++) {
				double angle = 2.0 * Math.PI * i / segments;
				double y = Math.Sin(angle);
				double x = Math.Cos(angle);
				double u = (x+1)/2;
				double v = (y+1)/2;
				vertices[vtxstart+i] = new SimpleVertex((float)x, (float)y, (float)u, (float)v);
			}

			if(type == CircleType.Filled)
				vertices[segments+1] = vertices[1];
			else
				vertices[segments] = vertices[0];


			vertexBuffer = new VertexBuffer(game.Device, typeof(SimpleVertex), vtxcount, BufferUsage.WriteOnly);
			vertexBuffer.SetData<SimpleVertex>(vertices);

		}

		VertexBuffer vertexBuffer;

		public void Draw() {
			game.Device.SetVertexBuffer(vertexBuffer);
			if (type == CircleType.Filled)
			{
				throw new NotSupportedException("triangle fan disappeared..");
				//game.Device.DrawPrimitives(PrimitiveType.TriangleFan, 0, segments);
			}
			else game.Device.DrawPrimitives(PrimitiveType.LineStrip, 0, segments);
		}
	}
}