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

using pr2.Common;

namespace pr2.CarotEngine
{

    public class TouchManager
    {

        public Point Position;
        public bool Touched;

        public void DebugDraw(Blitter b)
        {
            List<ITouchComponent> allComponents = new List<ITouchComponent>(tempRecords);
            allComponents.AddRange(permanentRecords);
            foreach (ITouchComponent comp in allComponents)
                comp.Region.Draw(b);
        }

        public void Update(bool touched, int x, int y)
        {
            Console.WriteLine("{0}, {1}", x, y);
            Position = new Point(x, y);
            bool lastTouched = Touched;
            Touched = touched;
            bool newTouch = (Touched && !lastTouched);
            released.Clear();
            hovered.Clear();

            ITouchComponent highestCover = null;

            List<ITouchComponent> allComponents = new List<ITouchComponent>(tempRecords);
            allComponents.AddRange(permanentRecords);

            //find highest priority covered
            foreach (ITouchComponent r in allComponents)
            {
                bool contains = r.Region.Test(x, y);
                if (!contains) continue;
                if (highestCover == null || highestCover.Priority < r.Priority)
                    highestCover = r;
            }

            foreach (ITouchComponent r in allComponents)
            {
                object o = r.Tag;
                bool contains = r.Region.Test(x, y);

                //HACK - apply priority logic
                if (r != highestCover)
                    contains = false;

                if (contains)
                    hovered.Add(r);

                if (contains && newTouch)
                {
                    if (pressed.Contains(r))
                        triggered.Remove(r);
                    else
                        triggered.Add(r);
                    pressed.Add(r);
                }
                else
                {
                    pressed.Remove(r);
                    triggered.Remove(r);
                    if (pressed.Contains(r))
                        released.Add(r);
                }
            }

            tagReleased.Clear();
            tagHovered.Clear();
            tagPressed.Clear();
            tagTriggered.Clear();
            foreach (ITouchComponent itc in released) tagReleased.Add(itc.Tag);
            foreach (ITouchComponent itc in hovered)
                tagHovered.Add(itc.Tag);
            foreach (ITouchComponent itc in pressed) tagPressed.Add(itc.Tag);
            foreach (ITouchComponent itc in triggered) tagTriggered.Add(itc.Tag);

            foreach (ITouchComponent r in allComponents)
                r.Update(this);

            tempRecords.Clear();
        }


        List<object> tagTriggered = new List<object>();
        List<object> tagPressed = new List<object>();
        List<object> tagReleased = new List<object>();
        List<object> tagHovered = new List<object>();
        List<ITouchComponent> triggered = new List<ITouchComponent>();
        List<ITouchComponent> pressed = new List<ITouchComponent>();
        List<ITouchComponent> released = new List<ITouchComponent>();
        List<ITouchComponent> hovered = new List<ITouchComponent>();

        public bool IsTriggered(object o) { return tagTriggered.Contains(o); }
        public bool IsPressed(object o) { return tagPressed.Contains(o); }
        public bool IsReleased(object o) { return tagReleased.Contains(o); }
        public bool IsHovered(object o) { return tagHovered.Contains(o); }
        public bool IsTriggered(ITouchComponent o) { return triggered.Contains(o); }
        public bool IsPressed(ITouchComponent o) { return pressed.Contains(o); }
        public bool IsReleased(ITouchComponent o) { return released.Contains(o); }
        public bool IsHovered(ITouchComponent o) { return hovered.Contains(o); }


        public void Register(int priority, object o, float x, float y, float width, float height)
        {
            Register(priority, o, new Rectangle((int)x, (int)y, (int)width, (int)height));
        }

        public void Register(object o, float x, float y, float width, float height)
        {
            Register(0, o, x, y, width, height);
        }

        public void Register(int priority, object tag, ITouchRegion rgn)
        {
            Record r = new Record();
            r.tag = tag;
            r.region = rgn;
            r.priority = priority;
            tempRecords.Add(r);
        }

        public void Register(int priority, object tag, Rectangle rect)
        {
            Record r = new Record();
            r.tag = tag;
            r.region = new RectTouchRegion(rect);
            r.priority = priority;
            tempRecords.Add(r);
        }

        class Record : TouchComponentBase
        {
        }

        public abstract class TouchComponentBase : ITouchComponent
        {
            public TouchComponentBase() { tag = this; }
            public virtual ITouchRegion Region { get { return region; } }
            public ITouchRegion region;
            public virtual int Priority { get { return priority; } }
            public int priority;
            public virtual object Tag { get { return tag; } }
            public object tag;
            public virtual void Update(TouchManager touchManager) { }
        }

        public interface ITouchRegion
        {
            bool Test(Point point);
            bool Test(int x, int y);
            Rectangle Bounds { get; }
            void Draw(Blitter b);
        }

        public class RectTouchRegion : ITouchRegion
        {
            public RectTouchRegion(Rectangle rect) { this.rect = rect; }
            public bool Test(Point point) { return rect.Contains(point); }
            public bool Test(int x, int y) { return rect.Contains(x, y); }
            public Rectangle rect;
            public Rectangle Bounds { get { return rect; } }
            public void Draw(Blitter b)
            {
                b.Color = Color.Red;
                b.PushAlpha(0.25f);
                using (Image image = GameEngine.Game.LoadImage("/assembly/WhiteNoise.png"))
                    b.StretchBlit(image, rect);
                b.RectFill(rect);
                b.PopAlpha();
            }
        }

        public class ConvexPolyTouchRegion : ITouchRegion
        {
            public ConvexPolyTouchRegion(Vector2[] poly)
            {
                init(poly);
            }
            protected ConvexPolyTouchRegion() { }
            protected void init(Vector2[] poly)
            {
                if (poly.Length < 3) throw new ArgumentException("Must have 3 or more points!");
                this.poly = (Vector2[])poly.Clone();
                int left = int.MaxValue; foreach (Vector2 v in poly) left = Math.Min((int)v.X, left);
                int top = int.MaxValue; foreach (Vector2 v in poly) top = Math.Min((int)v.Y, top);
                int right = int.MinValue; foreach (Vector2 v in poly) right = Math.Max((int)v.X, right);
                int bottom = int.MinValue; foreach (Vector2 v in poly) bottom = Math.Max((int)v.Y, bottom);
                bounds = new Rectangle(left, top, right - left + 1, bottom - top + 1);
            }
            public bool Test(Point point) { return GameEngine.IsPointInsideConvexPolygon(GameEngine.ToVector2(point), poly); }
            public bool Test(int x, int y) { return GameEngine.IsPointInsideConvexPolygon(new Vector2(x, y), poly); }
            public Vector2[] poly;
            Rectangle bounds;
            public Rectangle Bounds { get { return bounds; } }
            public void Draw(Blitter b)
            {
                b.Color = Color.Red;
                b.PushAlpha(0.25f);
                using (Image image = GameEngine.Game.LoadImage("/assembly/WhiteNoise.png"))
                {
                    b.Source = image;
                    b.EngageTextureMode();
                    TesselateRender(b);
                    b.Source = null;
                }
                b.EngineColorMode();
                TesselateRender(b);
                b.PopAlpha();
            }

            //this is a really, really bad tesselator
            void TesselateRender(Blitter b)
            {
                Vector2[] tri = new Vector2[3];
                for (int i = 0; i <= poly.Length - 2; i++)
                {
                    tri[0] = poly[0];
                    tri[1] = poly[i % poly.Length];
                    tri[2] = poly[(i + 1) % poly.Length];
                    b.Triangle(tri);
                }
            }
        }

        public class QuadrilateralTouchRegion : ConvexPolyTouchRegion
        {
            public QuadrilateralTouchRegion(Vector2[] poly)
            {
                if (poly.Length != 4) throw new ArgumentException("Must be a quadrilateral (Vector2[4])");
                init(poly);
            }
        }

        public interface ITouchComponent
        {
            ITouchRegion Region { get; }
            int Priority { get; }
            object Tag { get; }
            void Update(TouchManager touchManager);
        }

        public void Register(ITouchComponent comp)
        {
            tempRecords.Add(comp);
        }

        public void RegisterPermanent(ITouchComponent comp)
        {
            permanentRecords.Add(comp);
        }

        List<ITouchComponent> permanentRecords = new List<ITouchComponent>();
        List<ITouchComponent> tempRecords = new List<ITouchComponent>();

    }

}
