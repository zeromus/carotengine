using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pr2.Common;

namespace pr2.CarotEngine {

	/// <summary>
	/// Something that can drive entities. Maybe AI, maybe player input.
	/// </summary>
	public interface IEntityDriver {
		bool Left { get; }
		bool Right { get; }
		bool Up { get; }
		bool Down { get; }
		bool Action { get; }
		void UnpressAction();
	}

	public class PlayerInputDriver : IEntityDriver {
		public PlayerInput PlayerInput;
		public PlayerInputDriver(PlayerInput playerInput) {
			this.PlayerInput = playerInput;
		}

		public bool Left { get { return PlayerInput.Left; } }
		public bool Right { get { return PlayerInput.Right; } }
		public bool Up { get { return PlayerInput.Up; } }
		public bool Down { get { return PlayerInput.Down; } }
		public bool Action { get { return PlayerInput.Confirm; } }
		public void UnpressAction() { PlayerInput.Confirm.Unpress(); }
	}

	class ObstructorUnit {
		public Dictionary<object, Rectangle> hash = new Dictionary<object, Rectangle>();

		public void add(int x, int y, int w, int h, object o) {
			hash[o] = new Rectangle(x, y, w, h);
		}

		public void remove(object o) {
			hash.Remove(o);
		}

		public object[] query(int x, int y, int w, int h) {
			List<object> al = new List<object>();
			int x1 = x + w;
			int y1 = y + h;
			foreach(KeyValuePair<object, Rectangle> de in hash) {
				Rectangle r = de.Value;

				//are we to the left of the obstruction?
				if(x1 <= r.X) continue;

				//are we above the obstruction?
				if(y1 <= r.Y) continue;

				//are we to the right of the obstruction?
				if(x >= r.Right) continue;

				//are we beneath the obstruction
				if(y >= r.Bottom) continue;

				al.Add(de.Key);
			}

			return al.ToArray();
		}

		public bool check(int x, int y, int w, int h, object self) {
			int x1 = x + w;
			int y1 = y + h;
			foreach(KeyValuePair<object, Rectangle> de in hash) {
				Rectangle r = de.Value;

				//ignore self
				//if(de.Key == self) continue;

				//are we to the left of the obstruction?
				if(x1 <= r.X) continue;

				//are we above the obstruction?
				if(y1 <= r.Y) continue;

				//are we to the right of the obstruction?
				if(x >= r.Right) continue;

				//are we beneath the obstruction
				if(y >= r.Bottom) continue;

				return true;
			}

			return false;
		}

	}


	/// <summary>
	/// A character that only has a hotspot
	/// </summary>
	public class DummyCharacter : ICharacter {
		public DummyCharacter(int hotspotSize) { _hotspot = new Rectangle(0,0,hotspotSize,hotspotSize); }
		public DummyCharacter(Rectangle hotspot) { _hotspot = hotspot; }
		void IDisposable.Dispose() { }
		Image ICharacter.Frame { get { return null; } }
		bool ICharacter.SupportsCommand(CharacterCommands cc) { return false; }
		void ICharacter.Command(CharacterCommands cc, params object[] args) { }
		void ICharacter.Tick() { }
		public Rectangle Hotspot { get { return _hotspot; } set { _hotspot = value; } }
		Rectangle _hotspot;
	}


	public interface ICharacter : IDisposable {
		Image Frame { get; }
		bool SupportsCommand(CharacterCommands cc);
		void Command(CharacterCommands cc, params object[] args);
		void Tick();
		Rectangle Hotspot { get; }
	}

	public enum CharacterCommands {
		Face, Walk, Stop, SetEntityQuery, Step, Animate
	}

	interface IEntityQuery {
		float ticksPerPixel { get; }
	}

	public class Entity : IEntityQuery {
		private int TS = GameConfiguration.TileSize;

		public object UserData;
		public EntityEngine ee;
		public String activationScript;
		public int Layer;
		public float _x, _y;
		public float speed;
		public Directions facing;
		public ICharacter character;
		public V3Chr defaultV3Chr;
		public Mode mode;
		public bool bHurry;
		public bool bCanHurry = true;
		public int priority = 0;
		public bool bTileBased = true;
		public Directions lastInchDirection = Directions.None;
		public int stepsTaken = 0;
		public bool bEntityAI = true;
		public bool bStepToWalkSimulator;

		public bool bObstructedByEntities = true;
		public bool bTrackObstruction = true;
		public bool bPlayer = false;
		public bool bInParty = false;
		public bool bPaused = false;

		public float thinkDelay = 1000.0f;
		public float delayCounter;

		public float dx, dy;
		public State state = State.Ready;
		public float x { get { return _x; } set { _x = value; dx = value; } }
		public float y { get { return _y; } set { _y = value; dy = value; } }
		public int tx { get { return (int)_x / TS; } }
		public int ty { get { return (int)_y / TS; } }
		public Point TilePos { get { return new Point(tx, ty); } }
		public int dtx { get { return (int)dx / TS; } }
		public int dty { get { return (int)dy / TS; } }
		public bool bRenderHotspot;
		public delegate void EventHandler(Entity e);
		public EventHandler OnMoveFinished;

		public IEntityDriver Driver;

		public enum State {
			Idle, Ready, Delaying, Moving, Animating
		}

		//----IEntityQuery
		public float ticksPerPixel { get { return 1.0f / speed * 1000.0f; } }
		//----------------

		//public delegate void CustomThink(Entity e);
		//public CustomThink customThink;

		public void WarpTile(int x, int y) {
			if(ee != null)
				ee.EntityWarpTile(this, x, y);
			else _Warp(x*TS,y*TS);
		}
		public void WarpTile(Point pt) { WarpTile(pt.X,pt.Y); }

		/// <summary>
		/// Internal-use-only warp to pixel
		/// </summary>
		internal void _Warp(int x, int y) {
			dx = _x = x;
			dy = _y = y;
		}

		public void move(Directions dir, int distance)
		{
			if (dir == Directions.e) dx = x+distance;
			if (dir == Directions.w) dx = x-distance;
			if (dir == Directions.n) dy = y-distance;
			if (dir == Directions.s) dy = y+distance;

			character.Command(CharacterCommands.Walk, dir);
			state = Entity.State.Moving;
		}

		public void setCharacter(ICharacter c) {
			character = c;
			c.Command(CharacterCommands.SetEntityQuery, this);
			c.Command(CharacterCommands.Face, facing);
		}

		public void delay() {
			delayCounter = thinkDelay;
			state = Entity.State.Delaying;
			character.Command(CharacterCommands.Stop);
		}

		internal void step(Directions d) {
			character.Command(CharacterCommands.Step, d);
			state = Entity.State.Moving;
		}

		public void walk(Directions d) {
			character.Command(CharacterCommands.Walk, d);
			state = Entity.State.Moving;
		}

		public void stop() {
			character.Command(CharacterCommands.Stop);
			state = Entity.State.Ready;
		}

		public void face(Directions d) {
			character.Command(CharacterCommands.Face, d);
			facing = d;
		}

		/// <summary>
		/// Tells the entity to execute an animation.
		/// Note that the meaning of the string depends on the ICharacter
		/// </summary>
		public void animate(string str) {
			character.Command(CharacterCommands.Animate, str);
			state = Entity.State.Animating;
		}

		public enum Mode {
			None,
			Wander
		}
	}




	public class EntityEngine {
		public delegate void EntityActivationHandler(Entity activator, Entity target);
		public delegate void ZoneActivationHandler(Entity activator, int ZoneIndex);
		public event EntityActivationHandler EntityActivated;
		public event ZoneActivationHandler ZoneActivated;

		/// <summary>
		/// callback to be used after each entity is rendered
		/// </summary>
		public Action<Entity,Blitter,int,int,int> CallbackAfterRender;

		private int TS = GameConfiguration.TileSize;

		public EntityEngine(IMapDataReader mdr) {
			this.mdr = mdr;
		}

		public interface IMapDataReader {
			int ReadZone(Entity ent, int layer, int x, int y);
			int ReadObs(Entity ent, int layer, int x, int y);
			TileEngine.Zone GetZone(int zone);
			Size GetLayerSize(int layer);
		}

		//public DynamicObstructionManager dom = new DynamicObstructionManager();
		public IMapDataReader mdr;
		private static System.Random _rand = new Random();
		private static int rand(int a, int b) { lock(_rand) return _rand.Next(a, b); }
		private static int rand(int a) { lock(_rand) return _rand.Next(a); }
		private static int rand() { lock(_rand) return _rand.Next(); }

		ObstructorUnit ou = new ObstructorUnit();

		List<object> party = new List<object>();

		//should this be public?
		public List<object> ents = new List<object>();

		public void Tick() {
			var entsSorted = new List<object>(ents);
			entsSorted.Sort(new EntityPrioritySorter());
			foreach(Entity e in entsSorted)
				entityTick(e);
		}


		public void renderObs(int layer, int rx, int ry, Blitter b) {
			b.SetColor(255, 0, 0);
			b.PushAlpha(0.5f);
			foreach(Rectangle r in ou.hash.Values)
				b.RectFill(r.X - rx, r.Y - ry, r.Width, r.Height);
			b.PopAlpha();
		}

		class EntityPrioritySorter : IComparer<object> {
			int IComparer<object>.Compare(object a, object b)
			{
				Entity ea = (Entity)a;
				Entity eb = (Entity)b;
				return ea.priority - eb.priority;
			}
		}

		class EntityYSorter : IComparer<object> {
			List<object> priorityList;
			public EntityYSorter(List<object> priorityList) { this.priorityList = priorityList; }
			int IComparer<object>.Compare(object a, object b)
			{
				Entity ea = (Entity)a;
				Entity eb = (Entity)b;
				float ydiff = ea._y - eb._y;
				if(ydiff == 0.0f) {
					int priordiff = ea.priority - eb.priority;
					if(priordiff == 0) return priorityList.IndexOf(a) - priorityList.IndexOf(b);
					else return priordiff;
				} else return System.Math.Sign(ydiff);
			}
		}

		public void Render(int layer, int rx, int ry, Blitter b) {
			var entsSorted = new List<object>(ents);
			entsSorted.Sort(new EntityYSorter(ents));

			foreach(Entity e in entsSorted) {

				if(e.character == null) continue;
				if(e.Layer != layer) continue;
				int x = (int)e._x;
				int y = (int)e._y;
				x -= e.character.Hotspot.X;
				y -= e.character.Hotspot.Y;
				x -= rx;
				y -= ry;
				//y -= 8; //TODO testing entity y-offsetting; this requires support in the gfx
				Image frame = e.character.Frame;
				if(frame != null) {
					b.Blit(e.character.Frame, x, y);
				}
				if(e.bRenderHotspot) {
					b.PushAlpha(0.5f);
					b.Color = Color.Black;
					b.RectFill(x+e.character.Hotspot.X,y+e.character.Hotspot.Y,e.character.Hotspot.Width,e.character.Hotspot.Height);
					b.PopAlpha();
				}

				if(CallbackAfterRender != null)
					CallbackAfterRender(e,b,layer,x,y);
			}
		}

		void triggerZones(Entity e)
		{
			int zone_id = mdr.ReadZone(e, e.Layer, e.tx, e.ty);
			if (zone_id != 0 && ZoneActivated != null)
			{
				ZoneActivated(e, zone_id);
			}
		}

		void entityTick(Entity e) {
			if (e.bPaused) return;

			int ticks = 1;
			if(e.bHurry) ticks = 4;

			Entity.State startState = e.state;

			float timeUnit = (float)ticks;
			for(; ; ) {
				if (e.state == Entity.State.Moving)
				{
					float moveAdd = e.speed / 1000.0f * timeUnit;
					if (e._x != e.dx)
					{
						moveAdd = Math.Min(Math.Abs(e._x - e.dx), moveAdd);
						timeUnit -= moveAdd / e.speed * 1000.0f;
						if (moveAdd != 0.0f)
						{
							if (e._x > e.dx) moveAdd = -moveAdd;
							e._x += moveAdd;
						}
						if (timeUnit < 0.0001f)
						{
							if(!e.bTileBased && e.bTrackObstruction)
								ou.add((int)e._x, (int)e._y, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
							break;
						}
					}
					else if (e._y != e.dy)
					{
						moveAdd = Math.Min(Math.Abs(e._y - e.dy), moveAdd);
						timeUnit -= moveAdd / e.speed * 1000.0f;
						if (moveAdd != 0.0f)
						{
							if (e._y > e.dy) moveAdd = -moveAdd;
							e._y += moveAdd;
						}
						if (timeUnit < 0.0001f)
						{
							if(!e.bTileBased && e.bTrackObstruction)
								ou.add((int)e._x, (int)e._y, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
							break;
						}
					}
					if (e._x == e.dx && e._y == e.dy) //we must be done moving
					{
						if(e.bTrackObstruction) {
							ou.remove(e);
							ou.add((int)e._x, (int)e._y, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
						}
						e.stepsTaken++;
						if(e.Driver != null) {
							triggerZones(e);
						}

						//e.stop();

						////
						//if (e.playerInputBound)
						//{
						//    triggerZones(e);
						//}
						//if (e.onMoveFinished != null)
						//    e.onMoveFinished(e);
					}
				}

				//if we dont have any movement to do, see if we need any delaying before thinking
				if(e.state == Entity.State.Delaying) {
					e.delayCounter -= timeUnit;

					//if that last delay elapsed our counter, 
					//adjust the timeUnit of work left to do, and un-delay ourselves
					if(e.delayCounter <= 0.0f) {
						timeUnit = 1.0f + e.delayCounter;
						e.delayCounter = 0.0f;
						//e.state = Entity.State.Ready;
						e.stop();
					} else //otherwise, we can bail
						break;
				}

				//if we made it here, then we certainly need to do some thinking
				EntityThink(e);

				//if the result of that was going idle, then bail out
				if(e.state == Entity.State.Idle) {
					e.stop();
				}

				if(e.state == Entity.State.Ready) {
					//nothing to do
					break;
				}
			}

			//tick the character controller
			//this means that hurrying will make the characters animate faster
			if(e.character != null) while(ticks-- != 0) e.character.Tick();
		}

		void stepToWalkSimulatorEvent(Entity e) {
			e.stop();
		}

		public void partyAdd(Entity e) {
			e.bInParty = true;
			party.Add(e);
			if(party.Count == 1)
				e.bPlayer = true;
		}
		public void partyClear() {
			foreach(Entity e in party) {
				e.bInParty = false;
				e.bPlayer = false;
			}
			party.Clear();
		}

		public void EntityWarpTile(Entity e, Point pt) { EntityWarpTile(e, pt.X, pt.Y); }
		public void EntityWarpTile(Entity e, int tx, int ty) {
			if(e.bTrackObstruction)
				ou.remove(e);
			
			e._Warp(tx * TS, ty * TS);

			if(e.bTrackObstruction)
				ou.add((int)e._x, (int)e._y, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
		}

		//starts a WALK command but will not finish it
		public void entityInch(Entity e, Directions d) {
			switch(d) {
				case Directions.n: e.dy -= 1.0f; break;
				case Directions.s: e.dy += 1.0f; break;
				case Directions.w: e.dx -= 1.0f; break;
				case Directions.e: e.dx += 1.0f; break;
			}

			e.state = Entity.State.Moving;

			if(d != e.lastInchDirection) {
				e.walk(d);
				e.lastInchDirection = d;
			}

			ou.remove(e);
			ou.add((int)e.dx, (int)e.dy, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
		}

		public void OnMoveFinished(Entity e) {
			if(e.bStepToWalkSimulator) {
				e.stop();
				e.bStepToWalkSimulator = false;
			}
			if(e.OnMoveFinished != null)
				e.OnMoveFinished(e);
		}

		public void entityStep(Entity e, Directions d) {
			if(e.character.SupportsCommand(CharacterCommands.Step))
				e.step(d);
			else {
				e.walk(d);
				e.bStepToWalkSimulator = true;
			}

			switch(d) {
				case Directions.n: e.dy -= TS; break;
				case Directions.s: e.dy += TS; break;
				case Directions.e: e.dx += TS; break;
				case Directions.w: e.dx -= TS; break;
			}

			//e.stepsTaken++;

			ou.remove(e);
			ou.add((int)e.dx, (int)e.dy, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
		}

		//warning: evil logic be here
		public void CheckObstructionLayer(ref bool lb, ref bool rb, ref bool ub, ref bool db, Entity e, int distance) {
			int px = (int)e._x;
			int py = (int)e._y;
			int tx = px / TS;
			int ty = py / TS;
			int pxe = px + e.character.Hotspot.Width - 1;
			int pye = py + e.character.Hotspot.Height - 1;

			for(int j = (px - distance) / TS; j < px / TS; j++)
				for(int i = py / TS; i <= pye / TS; i++)
					lb |= (mdr.ReadObs(e, e.Layer, j, i) != 0);

			for(int j = (pxe + distance) / TS; j > (pxe) / TS; j--)
				for(int i = py / TS; i <= pye / TS; i++)
					rb |= (mdr.ReadObs(e, e.Layer, j, i) != 0);

			for(int j = (py - distance) / TS; j < py / TS; j++)
				for(int i = px / TS; i <= pxe / TS; i++)
					ub |= (mdr.ReadObs(e, e.Layer, i, j) != 0);

			for(int j = (pye + distance) / TS; j > (pye) / TS; j--)
				for(int i = px / TS; i <= pxe / TS; i++)
					db |= (mdr.ReadObs(e, e.Layer, i, j) != 0);
		}

		public void scanObstructionEntities(out object[] le, out object[] re, out object[] ue, out object[] de, Entity e, int distance) {
			int px = (int)e._x;
			int py = (int)e._y;
			Rectangle hr = e.character.Hotspot;

			le = ou.query(px - distance, py, distance, hr.Height);
			re = ou.query(px + hr.Width, py, distance, hr.Height);
			ue = ou.query(px, py - distance, hr.Width, distance);
			de = ou.query(px, py + hr.Height, hr.Width, distance);
		}

		public void CheckObstructionEntities(ref bool lb, ref bool rb, ref bool ub, ref bool db, Entity e, int distance, bool bPartyMembersObstruct) {
			object[] le, re, ue, de;
			scanObstructionEntities(out le, out re, out ue, out de, e, distance);
			for(int i = 0; i < le.Length; i++) if(le[i] != e) if(bPartyMembersObstruct || !bPartyMembersObstruct && !party.Contains(le[i])) lb = true;
			for(int i = 0; i < re.Length; i++) if(re[i] != e) if(bPartyMembersObstruct || !bPartyMembersObstruct && !party.Contains(re[i])) rb = true;
			for(int i = 0; i < ue.Length; i++) if(ue[i] != e) if(bPartyMembersObstruct || !bPartyMembersObstruct && !party.Contains(ue[i])) ub = true;
			for(int i = 0; i < de.Length; i++) if(de[i] != e) if(bPartyMembersObstruct || !bPartyMembersObstruct && !party.Contains(de[i])) db = true;
		}

		public void doHurries(int layer, int tx, int ty) {
			//object[] oa = dom.read(layer,tx,ty);
			//if(oa == null) return;
			//foreach(Entity e in oa)
			//    if(e.bCanHurry) 
			//        e.bHurry = true;
		}

		public void doHurries2(Entity self, int layer, int x, int y, int w, int h) {
			object[] oa = ou.query(x, y, w, h);
			if(oa == null) return;
			foreach(Entity e in oa)
				if(e != self)
					if(e.bCanHurry)
						e.bHurry = true;
		}

		public void entityActivate(Entity self, int layer, int x, int y, int w, int h)
		{
			object[] oa = ou.query(x, y, w, h);
			if (oa == null) return;
			foreach (Entity e in oa)
				if (e != self)
					if (EntityActivated != null)
						EntityActivated(self, e);
		}

		void DriverThink(Entity e) {
			Entity.State oldState = e.state;
			e.state = Entity.State.Idle;
			Directions dir = Directions.None;

			bool l, r, u, d; l = r = u = d = false;
			if(e.Driver.Left && !e.Driver.Right) l = true;
			else if(!e.Driver.Left && e.Driver.Right) r = true;
			else if(e.Driver.Up && !e.Driver.Down) u = true;
			else if(!e.Driver.Up && e.Driver.Down) d = true;

			bool acc = false;

			//mbg 2/18/08 - dont understand this
			//if (e.playerInput.Confirm && !e.playerInput.Cancel)
			//{
			//    acc = true;
			//    e.playerInput.Confirm.Unpress();
			//}

			if(e.Driver.Action) {
				acc = true;
				e.Driver.UnpressAction();
			}
			

			bool lb, rb, ub, db; lb = rb = ub = db = false;
			bool elb, erb, eub, edb; elb = erb = eub = edb = false;
			if(e.bTileBased) {
				CheckObstructionLayer(ref lb, ref rb, ref ub, ref db, e, TS);
				if(e.bObstructedByEntities)
					CheckObstructionEntities(ref elb, ref erb, ref eub, ref edb, e, TS, false);
			} else {
				CheckObstructionLayer(ref lb, ref rb, ref ub, ref db, e, 1);
				if(e.bObstructedByEntities)
					CheckObstructionEntities(ref elb, ref erb, ref eub, ref edb, e, 1, false);
			}
			

			int hw = e.character.Hotspot.Width;
			int hh = e.character.Hotspot.Height;

			//we only want to face in tile-based mode
			//because for non-tilebased mode, we need to
			//keep the ChrController walking while the
			//player is pressing buttons, without interrupting
			//the walk with face commands
			if(l) {
				dir = Directions.w;
				if(e.bTileBased) {
					e.face(Directions.w);
					if(elb) doHurries2(e, e.Layer, (int)e._x - TS, (int)e._y, TS, hh);
					if(!lb && !elb)
						entityStep(e, Directions.w);
				} else {
					e.facing = Directions.w;
					if(elb) doHurries2(e, e.Layer, (int)e._x - 1, (int)e._y, 1, hh);
					if(!lb && !elb)
						entityInch(e, Directions.w);
				}
			} else if(r) {
				dir = Directions.e;
				if(e.bTileBased) {
					e.face(Directions.e);
					if(erb) doHurries2(e, e.Layer, (int)e._x + hh, (int)e._y, TS, hh);
					if(!rb && !erb)
						entityStep(e, Directions.e);
				} else {
					e.facing = Directions.e;
					if(erb) doHurries2(e, e.Layer, (int)e._x + hw, (int)e._y, 1, hh);
					if(!rb && !erb)
						entityInch(e, Directions.e);
				}
			} else if(u) {
				dir = Directions.n;
				if(e.bTileBased) {
					e.face(Directions.n);
					if(eub) doHurries2(e, e.Layer, (int)e._x, (int)e._y - TS, hw, TS);
					if(!ub && !eub)
						entityStep(e, Directions.n);
				} else {
					e.facing = Directions.n;
					if(eub) doHurries2(e, e.Layer, (int)e._x, (int)e._y - 1, hw, 1);
					if(!ub && !eub)
						entityInch(e, Directions.n);
				}
			} else if(d) {
				dir = Directions.s;
				if(e.bTileBased) {
					e.face(Directions.s);
					if(edb) doHurries2(e, e.Layer, (int)e._x, (int)e._y + hh, hw, TS);
					if(!db && !edb)
						entityStep(e, Directions.s);
				} else {
					e.facing = Directions.s;
					if(edb) doHurries2(e, e.Layer, (int)e._x, (int)e._y + hh, hw, 1);
					if(!db && !edb)
						entityInch(e, Directions.s);
				}
			}

			if (acc)
			{
				if (e.facing == Directions.e)
				{
					if (erb)
						entityActivate(e, e.Layer, (int)e._x + hh, (int)e._y, TS, hh);

					zoneActivateAdjacent(e);
				}
				else if (e.facing == Directions.w)
				{
					if (elb)
						entityActivate(e, e.Layer, (int)e._x - TS, (int)e._y, TS, hh);

					zoneActivateAdjacent(e);
				}
				else if (e.facing == Directions.n)
				{
					if (eub)
						entityActivate(e, e.Layer, (int)e._x, (int)e._y - TS, hw, TS);

					zoneActivateAdjacent(e);
				}
				else if (e.facing == Directions.s)
				{
					if (edb)
						entityActivate(e, e.Layer, (int)e._x, (int)e._y + hh, hw, TS);

					zoneActivateAdjacent(e);
				}
			}

			if(!e.bTileBased) {
				if(e.lastInchDirection != dir && dir != Directions.None)
					e.face(dir);

				if(e.lastInchDirection != Directions.None && dir == Directions.None) {
					e.stop();
					//e.face(dir);
					e.lastInchDirection = Directions.None;
				}
			}
		}

		void zoneActivateAdjacent(Entity e)
		{
			int tx = e.tx;
			int ty = e.ty;

			Directions d = e.facing;

			if (d == Directions.e) tx++;
			if (d == Directions.w) tx--;
			if (d == Directions.s) ty++;
			if (d == Directions.n) ty--;


			int zone = mdr.ReadZone(e, e.Layer, tx, ty);

			if (zone > 0)
			{
				TileEngine.Zone z = mdr.GetZone(zone);

				if (z != null)
				{
					if (z.bAdjacent && ZoneActivated != null)
					{
						e.Driver.UnpressAction();
						ZoneActivated(e, zone);
					}
				}
			}
		}

		void ClaimObstruction(Entity e) {
			ou.remove(e);
			ou.add((int)e.dx, (int)e.dy, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
		}

		void EntityThink(Entity e) {
			//if we have a driver, then use that
			if(e.Driver != null) {
				DriverThink(e);
				return;
			}
			
			//otherwise do no thinking if we are part of the party
			//if(e.bInParty) { return; }

			//otherwise, this is being driven by raw commands.
			//I really wish there were no raw commands.
			//npc AI should probably be turned into a driver one day!!! cool architecture!

			//Finish up the movement
			if(e.state == Entity.State.Moving) {	
				ClaimObstruction(e);

				OnMoveFinished(e);

				if(e.bEntityAI)
					e.delay();

				return;
			}

			if(!e.bEntityAI)
				return;

			//--------------
			//the rest of this is NPC thinking

			bool lb, rb, ub, db; lb = rb = ub = db = false;

			CheckObstructionLayer(ref lb, ref rb, ref ub, ref db, e, TS);
			CheckObstructionEntities(ref lb, ref rb, ref ub, ref db, e, TS, true);

			switch(e.mode) {
				case Entity.Mode.None: { e.state = Entity.State.Delaying; e.delay(); break; }
				case Entity.Mode.Wander: {
						//verge-style wandering (dont even try to move where you can't)
						if(e.bHurry) {
							int count = 0;
							int trick = 0;
							if(!lb) count++;
							if(!rb) count++;
							if(!ub) count++;
							if(!db) count++;
							if(count == 0) { e.delay(); return; }
							int move = rand(count);
							if(!lb) { if(move == trick++) { entityStep(e, Directions.w); break; } }
							if(!rb) { if(move == trick++) { entityStep(e, Directions.e); break; } }
							if(!ub) { if(move == trick++) { entityStep(e, Directions.n); break; } }
							entityStep(e, Directions.s);
						}
							//new-style wandering (try to move where you can't; face if youre obstructed)
						else {
							int move = rand(4);
							if(move == 0) {
								if(lb) { e.face(Directions.w); e.delay(); } else entityStep(e, Directions.w);
							}
							if(move == 1) {
								if(rb) { e.face(Directions.e); e.delay(); } else entityStep(e, Directions.e);
							}
							if(move == 2) {
								if(ub) { e.face(Directions.n); e.delay(); } else entityStep(e, Directions.n);
							}
							if(move == 3) {
								if(db) { e.face(Directions.s); e.delay(); } else entityStep(e, Directions.s);
							}
						}
						break;
					}
			}

			//no need to hurry anymore; we've hopefully made a decision that will have got us out of the way
			e.bHurry = false;
		}



		public Entity SpawnEntity(ICharacter character) {
			Entity e = new Entity();
			e.character = character;
			e.Layer= 0;
			e.y = e.x = 0.0f;
			e.facing = Directions.s;
			ents.Add(e);
			ou.add((int)e.dx, (int)e.dy, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
			return e;
		}

		public void AddEntity(Entity e)
		{
			e.ee = this;
			e.Layer = 0;
			e.facing = Directions.s;
			ents.Add(e);
			if(e.bTrackObstruction)
				ou.add((int)e.dx, (int)e.dy, e.character.Hotspot.Width, e.character.Hotspot.Height, e);
		}

		//public Entity SpawnMapEntity(Map.Entdef entdef) {
		//    Entity e = new Entity();
		//    e.layer = 0;
		//    e.facing = entdef.facing;
		//    e.speed = (float)(entdef.speed) / 2.0f;
		//    e.mode = Entity.Mode.Wander; //TODO HACK
		//    e.bRenderHotspot = true;
		//    e.activationScript = entdef.script;

		//    //HACK - needs to get path somehow
		//    e.defaultV3Chr = new V3Chr(Lib.GetAccompanyingFilePath("content/x",entdef.chrname));

		//    //automatically prefill the entities with a controller
		//    e.character = new V3ChrController(e.defaultV3Chr);
		//    //e.setCharacter(new FF6ChrController(e.defaultV3Chr,new int[]{7,9,12,0,2,5,14,16,19,21,23,26}));

		//    //(be sure to set chr before warping... since warping to a spot
		//    //requires claiming an obstruction area)
		//    entityWarpTile(e, entdef.x, entdef.y);


		//    ents.Add(e);
		//    return e;
		//}
	}
}