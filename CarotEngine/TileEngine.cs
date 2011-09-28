using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {
	public class TileEngine :  GameEngineComponent {
		public Map m;
		public EntityEngine ee;
		public List<Zone> zones = new List<Zone>();
		public MapEntityHashtable mapEnts = new MapEntityHashtable();

		public bool bRenderObsLayer = false;
		public bool bRenderEntityObs;

		//public int fillTile = -1;
		public Map.Layer fillLayer = null;

		public class MapEntityHashtable {
			Dictionary<object, object> byindex = new Dictionary<object, object>();
			Dictionary<object, object> byname = new Dictionary<object, object>();
			public void add(Entity e, string name, int index) {
				byindex[index] = e;
				byname[name] = e;
			}
			public Entity this[object key] {
				get {
					if(key is string) return (Entity)byname[key];
					else return (Entity)byindex[key];
				}
			}
		}

		class EntityEngineInterface : EntityEngine.IMapDataReader {
			TileEngine te;
			public EntityEngineInterface(TileEngine te) { this.te = te; }
			public int ReadZone(Entity ent, int layer, int x, int y) { return te.m.logicalLayer.readZone(x, y); }
			public int ReadObs(Entity ent, int layer, int x, int y) { return te.m.logicalLayer.readObs(x, y); }
			public TileEngine.Zone GetZone(int zone) { if (zone < te.zones.Count) return te.zones[zone]; return null;  }
			public Point getScroll(int layer) { return new Point((int)te.camera.X, (int)te.camera.Y); } 
			public Size GetLayerSize(int layer) { return new Size(te.m.logicalLayer.width, te.m.logicalLayer.height); }
		}

		public class Zone
		{
			public float chance;
			public String name;
			public String script;
			public bool bAdjacent;			
		}

		public void tick() {
			m.vsp.Update();
			ee.Tick();
			camera.Tick();
		}

		public TileEngine(Map m) {
			
			this.m = m;
			
			ee = new EntityEngine(new EntityEngineInterface(this));
			camera = new TileEngineCamera(m.width,m.height);
			
			foreach(Map.Entdef entdef in m.entdefs)
			{
				//todo
				//Entity e = ee.SpawnMapEntity(entdef);
				//mapEnts.add(e, entdef.descr, entdef.index);
			}

			foreach (Map.Zonedef zd in m.zonedefs)
			{
				Zone z = new Zone();
				z.bAdjacent = (zd.method == 1);
				z.chance = zd.percent / 255.0f;
				z.name = zd.name;
				z.script = zd.script;

				zones.Add(z);
			}

			ee.EntityActivated += new EntityEngine.EntityActivationHandler(ee_EntityActivated);
			ee.ZoneActivated += new EntityEngine.ZoneActivationHandler(ee_ZoneActivated);
		}

		private void ee_ZoneActivated(Entity activator, int ZoneIndex)
		{
			Console.WriteLine("Encountered zone activation {0}", ZoneIndex);
			double roll = zonernd.NextDouble();
			Zone z = zones[ZoneIndex];

			if (roll < z.chance)
			{
				//m.scriptHandler.ZoneInvoke(activator, z);
			}
		}

		Random zonernd = new Random();
		private void ee_EntityActivated(Entity activator, Entity target)
		{
			Console.WriteLine("Entity activation occurred, script: {0}", target.activationScript);
			//m.scriptHandler.EntityInvoke(activator, target);
		}

		public TileEngineCamera camera;

		private void renderObsLayer(int xo, int yo, Blitter b) {
			int xofs = -(xo & 15);
			int yofs = -(yo & 15);
			int xtc = xo >> 4;
			int ytc = yo >> 4;

			int tw = (b.Dest.Width >> 4) + 2;
			int th = (b.Dest.Height >> 4) + 2;

			b.SetColor(255, 255, 255);
			b.PushAlpha(0.5f);
			for(int y = 0; y < th; y++)
				for(int x = 0; x < tw; x++) {
					if(y + ytc >= m.height || x + xtc >= m.width || x + xtc < 0 || y + ytc < 0) continue;
					int t = m.logicalLayer.readObs(x + xtc, y + ytc);
					if(t == 0) continue;
					b.RectFill(xofs + x * 16, yofs + y * 16, 16, 16);
				}
			b.PopAlpha();
		}

		private void renderLayer(bool bBottom, int xo, int yo, Map.TileLayer l, Blitter b) {

			int oxw = (int)((float)xo * l.parallax.X);
			int oyw = (int)((float)yo * l.parallax.Y);
			int xofs = -(oxw & 15);
			int yofs = -(oyw & 15);
			int xtc = oxw >> 4;
			int ytc = oyw >> 4;

			int tw = (b.width >> 4) + 2;
			int th = (b.height >> 4) + 2;

			b.BeginBatch();
			b.PushAlpha((float)((100 - (float)l.lucent) / 100.0));

			if(bBottom)
				for(int y = 0; y < th; y++)
					for(int x = 0; x < tw; x++) {
						int t = l.getTile(x + xtc, y + ytc);
						if(t == 0) continue;
						if(t == -1) {
							if(fillLayer != null) {
								t = fillLayer[(fillLayer.height + ((y + ytc) % fillLayer.height)) % fillLayer.height,
											(fillLayer.width + ((x + xtc) % fillLayer.width)) % fillLayer.width];
							} else continue;
						}

						// Overkill: Takes animations into account.
						t = m.vsp.tileIndex[t];

						b.BlitSubrectBatched(m.vsp.tiles, (t & 127) << 4, ((t >> 7) << 4), 16, 16, xofs + x * 16, yofs + y * 16);
					} else
				for(int y = 0; y < th; y++)
					for(int x = 0; x < tw; x++) {
						int t = l.getTile(x + xtc, y + ytc);
						if(t < 1) continue;

						// Overkill: Takes animations into account.
						t = m.vsp.tileIndex[t];

						b.BlitSubrectBatched(m.vsp.tiles, (t & 127) << 4, ((t >> 7) << 4), 16, 16, xofs + x * 16, yofs + y * 16);
					}

			b.ExecuteSubrectBatch(m.vsp.tiles);
			b.PopAlpha();
		}

		public void render(int rx, int ry, Blitter b) {
			bool bBottom = true;
			string[] ops = m.rstring.Split(',');
			foreach(string str in ops) {
				if(str == "E") {
					ee.Render(0, rx, ry, b);
					if(bRenderEntityObs) ee.renderObs(0, rx, ry, b);
				}
				try {
					//wtf xna has no tryparse
					//if(int.TryParse(str, out l)) {
					if(str[0] >= '0' && str[0] <= '9') {
						int l = int.Parse(str);
						renderLayer(bBottom, rx, ry, m.layers[l - 1], b);
						bBottom = false;
					}
				} catch { }
			}
			if(bRenderObsLayer) {
				renderObsLayer(rx, ry, b);
			}
		}

		public void renderLayerFast(bool isBottom, int xo, int yo, Map.TileLayer l, Image dest) {

			//SimpleVertex[] v = new SimpleVertex[4];
			//v[0].x = v[0].y = 0;
			//v[1].x = dest.Width; v[0].y = 0;
			//v[2].x = dest.Width; v[2].y = dest.Height;
			//v[3].x = 0; v[3].y = dest.Height;

			//v[0].tu = xo / 16.0f;
			//v[0].tv = yo / 16.0f;
			//v[1].tu = v[0].tu + dest.Width / 16.0f;
			//v[1].tv = v[0].tv;
			//v[2].tu = v[1].tu;
			//v[2].tv = v[0].tv + dest.Height / 16.0f;
			//v[3].tu = v[0].tu;
			//v[3].tv = v[2].tv;

			//game.CustomRender();
			//game.Techniques.map.setTextures(l.tex, m.vsp.tiles.getTex());
			//game.Techniques.map.setParams(m.vsp.tilesW, m.vsp.tilesH, l.width, l.height);
			//game.Techniques.map.Activate();

			//game.SetPointFiltering(0); //tilemap HAS to be point filtering. make sure here
			////game.setLinearFiltering(1); //in case we want linear filtering
			
			//if(isBottom)
			//    game.DisableAlphaBlend();

			//game.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, v, 0, 2);

			//if (isBottom)
			//    game.EnableAlphaBlend();
		}

		public void renderFast(int rx, int ry, Image dest) {
			Blitter b = new Blitter(dest);
			game.SetDest(dest);

			bool bBottom = true;
			string[] ops = m.rstring.Split(',');

			foreach(string str in ops) {
				if(str == "E") {
					ee.Render(0, rx, ry, b);
					if(bRenderEntityObs) ee.renderObs(0, rx, ry, b);
				}
				//wtf xna has no tryparse
				//if(int.TryParse(str, out l)) {
				if(str[0] >= '0' && str[0] <= '9') {
					int l = int.Parse(str);
					renderLayerFast(bBottom, rx, ry, m.layers[l - 1], dest);
					bBottom = false;
				}
			}
			if(bRenderObsLayer) {
				renderObsLayer(rx, ry, b);
			}
		}

		public void render(Image dest) {
			renderFast((int)camera.X, (int)camera.Y, dest);
		}

	}
}