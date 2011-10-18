using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using pr2.CarotEngine;
using pr2.Common;

namespace Cabedge
{

    public partial class Cabedge : GameEngine
    {

        public override Type FindMeAFuckingType(string name)
        {
            if (name == "dock") return typeof(Cabedge.Map_dock);
            return null;
        }

        /// <summary>
        /// global cabedge game instance
        /// </summary>
        protected static Cabedge cabedge;

        static void Main(string[] args)
        {
            //dont want to deal with resourcemanager for now
            ResourceManager.OverrideEnableBegin(false);
            cabedge = new Cabedge();
            cabedge.Run();
        }

        protected override void GameInitialize()
        {
            SetResolution(640, 480);

            LoadResources();
        }

        void LoadResources()
        {
            redhead = LoadImage("raw/redhead.png");
            font_v3_1_black = new V3Sysfont(1, Color.Black);
            font_v3_2_black = new V3Sysfont(2, Color.Black);
            font_v3_3_black = new V3Sysfont(3, Color.Black);
            font_v3_4 = new V3Sysfont(4, Color.White);
            font_v3_6 = new V3Sysfont(4, Color.White);

            testFont = FontSW.v3import("raw/font2.png", true);
            img_ui = LoadImage("raw/ui.png");
            img_ui.Alphafy(Color.Cyan);
        }

        protected V3Sysfont font_v3_1_black;
        protected V3Sysfont font_v3_2_black;
        protected V3Sysfont font_v3_3_black;
        protected V3Sysfont font_v3_4;
        protected V3Sysfont font_v3_6;
        FontSW testFont;
        Image redhead;

        protected PlayerInput player1 = new PlayerInput(PlayerIndex.One);


        ///// <summary>
        ///// hacky input function. needs unpress.
        ///// </summary>
        //void input() {
        //    KeyboardState ks = Keyboard.GetState();
        //    GamePadState gps = GamePad.GetState(PlayerIndex.One);
        //    down = ks.IsKeyDown(Keys.Down) || (gps.DPad.Down == ButtonState.Pressed);
        //    up = ks.IsKeyDown(Keys.Up) || (gps.DPad.Up == ButtonState.Pressed);
        //    left = ks.IsKeyDown(Keys.Left) || (gps.DPad.Left == ButtonState.Pressed);
        //    right = ks.IsKeyDown(Keys.Right) || (gps.DPad.Right == ButtonState.Pressed);
        //    confirm = ks.IsKeyDown(Keys.LeftControl) || (gps.Buttons.A == ButtonState.Pressed);
        //    cancel = ks.IsKeyDown(Keys.Space) || (gps.Buttons.X == ButtonState.Pressed);
        //}

        //bool up, down, left, right, confirm, cancel;

        public override void Update(GameTime gameTime)
        {
            player1.Update();
            base.Update(gameTime);
            //gameTime.TotalGameTime difference from last is current game ticks
            //input();

            //Console.WriteLine("Up-{0}; Down-{1}; Left-{2}; Right-{3}; Confirm-{4}; Cancel-{5}", up, down, left, right, confirm, cancel);
            gameController.update(gameTime.ElapsedGameTime.Milliseconds);
        }

        void loading()
        {
            baseParty.chars.Add(new Character("Mr. Rogers", "neighbor"));
            baseParty.chars.Add(new Character("Dr. Tofu", "food"));
            baseParty.chars.Add(new Character("Smurf Master", "cartoon"));
            baseParty.chars.Add(new Character("Corncob", "food"));
            baseParty.chars.Add(new Character("Earmite", "creature"));
            baseParty.chars.Add(new Character("Spacebob", "enigma"));
            baseParty.chars.Add(new Character("Nugget", "food"));
            baseParty.chars.Add(new Character("Sloppy Jack", "neighbor"));
            baseParty.chars.Add(new Character("Rev. Tipper", "evangelist"));
            baseParty.chars.Add(new Character("Slipstick Jones", "human calculator"));
            baseParty.chars.Add(new Character("Hate Sandwich", "food"));
            baseParty.chars.Add(new Character("Fury Muffin", "food"));
            //baseParty.chars.Add(new Character("Danger Cracker"));
            //baseParty.chars.Add(new Character("Wrathful Buns"));
            //baseParty.chars.Add(new Character("Vengeance Pie"));
            //baseParty.chars.Add(new Character("Ire Cake"));
            //baseParty.chars.Add(new Character( "Nibbler"));
            //baseParty.chars.Add(new Character("Gruench"));
            //baseParty.chars.Add(new Character("Tubby Mugsy"));

            baseParty.chars[0].loadChar("raw/nathan.chr");

            baseParty.availableEnhancements.Add(Enhancements.mhp_plus_100);
            baseParty.availableEnhancements.Add(Enhancements.str_plus_10);
            baseParty.availableEnhancements.Add(Enhancements.premonition);

            baseParty.equippedEnhancements[0] = Enhancements.premonition;
            baseParty.equippedEnhancements[1] = Enhancements.mhp_plus_100;

            updateParty();
        }




        protected override void Draw(GameTime gameTime)
        {
            gameController.draw();
            //device.Clear(Color.Black);
            //te.renderFast(0, 0, screen);
            //Blitter b = new Blitter(screen);
            //b.alpha = 0.50f;
            //b.blit(redhead, 0, 0);
            //Microsoft.Xna.Framework.Input.Keyboard.GetState();
            //b.color = Color.Black;
            //b.alpha = 1;
            //b.rectFill(0, 0, 240, 40);
            //font_v3_4.render(b, 5, 5, "hello, world!");

            ////int xxx= font_v3_6.measureWidth("p i - r - s q u a r e d");

            //b.offsetWindow(0, 50);
            //b.rectFill(0, 0, 240, 40);
            //FontRenderer fr = new FontRenderer(testFont);
            //fr.render(b, 5, 5, "hello, world!");
        }
    }
}
