using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace pr2.CarotEngine
{
    public partial class RpgController
    {
        public static RpgController rpg;
        protected TileEngine CurrentTileEngine;
        protected Map CurrentMap;
        protected Entity playerEntity;
        protected V3Chr playerChr;
        protected V3ChrController playerChrController;
        protected PlayerInput playerInput;
        protected List<ActionScript> scripts = new List<ActionScript>();
        protected ActionScript workingScript = null;

        public RpgController(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
            if (RpgController.rpg == null)
                RpgController.rpg = this;
        }

        public Entity GetPlayer()
        {
            return playerEntity;
        }

        public void EntitySetFacing(Entity ent, Directions direction)
        {
            ent.face(direction);
        }

        public Map GetCurrentMap()
        {
            return CurrentMap;
        }

        public void SwitchMap(String mapname)
        {
            SwitchMap(mapname, null);
        }

        public void SwitchMap(String mapname, String startLoc)
        {
            UnloadMap();

            CurrentMap = new Map(mapname,this);
            CurrentMap.initTextures();
            CurrentTileEngine = new TileEngine(CurrentMap);
            CurrentMap.scriptHandler.InvokeOnload(startLoc);
            playerInput.UnpressAll();
        }

        public void CameraLookAt(int x, int y)
        {
            if (CurrentTileEngine != null)
            {
                CurrentTileEngine.camera.Attach(null);
                CurrentTileEngine.camera.X = x;
                CurrentTileEngine.camera.Y = y;
            }
        }

        public void CameraDetach()
        {
            if (CurrentTileEngine != null)
            {
                CurrentTileEngine.camera.Attach(null);
            }
        }

        public void CameraLookAt(Entity ent)
        {
            if (CurrentTileEngine != null)
            {
                CurrentTileEngine.camera.Attach(ent);
            }
        }

        public Entity CreateEntity(String chr, int tx, int ty)
        {
            Entity e = null;

            if (CurrentTileEngine != null)
            {
                e = new Entity();
                V3Chr v3c = new V3Chr(chr);
                V3ChrController v3cc = new V3ChrController(v3c);

                e.setCharacter(v3cc);
                e.WarpTile(tx * 16, ty * 16);
                e.speed = 100f;
                CurrentTileEngine.ee.AddEntity(e);

            }

            return e;
        }

        public Entity CreatePlayer(String chr, int tx, int ty)
        {
            if (CurrentTileEngine != null)
            {
                playerEntity = new Entity();
                playerChr = new V3Chr(chr);
                playerChrController = new V3ChrController(playerChr);

                //playerEnt.bPlayer = true;
                playerEntity.setCharacter(playerChrController);
                CurrentTileEngine.ee.EntityWarpTile(playerEntity, tx, ty);
                playerEntity.speed = 100f;
                //playerEntity.bTileBased = false;

                //MUST TODO
                playerEntity.Driver = new PlayerInputDriver(playerInput);

                CurrentTileEngine.ee.AddEntity(playerEntity);
                CurrentTileEngine.camera.Attach(playerEntity);
            }
            else
            {
                playerEntity = null;
            }

            return playerEntity;
        }

        public void UnloadMap()
        {
            CurrentMap = null;
            CurrentTileEngine = null;
        }

        public void tick()
        {
            if (CurrentTileEngine != null)
            {
                CurrentTileEngine.tick();

                foreach (ActionScript script in scripts.ToArray())
                    script.tick();
            }
        }

        public void draw(Image dest)
        {
            if (CurrentTileEngine != null)
            {
                CurrentTileEngine.render(dest);
            }
        }


    }
}
