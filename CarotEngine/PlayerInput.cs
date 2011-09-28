using System;
using System.Threading;
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

namespace pr2.CarotEngine
{
	/// <summary>
	/// at some point we need to make this polled at high frequency to support keyboard input or something
	/// also to capture very quick button presses? but if we have that, we need a concept of freezing the state
	/// so that it doesnt change for an entire update cycle. 
	/// we will want to catch quick button presses even if they begin and end during a render()
	/// </summary>
	public class PlayerInput {

		public VirtualButton Left, Right, Down, Up, Confirm, Cancel, A, B, MenuY, MenuX, LShoulder, RShoulder, BX, AY;
		public VirtualButton F5, F10;
		public VirtualButton NumPad9, NumPad8, NumPad7, NumPad4, NumPad1;
		public VirtualButton LeftCtrl;
		public VirtualButton DebugPrint;
		public VirtualButton LeftMouse, RightMouse;
        public VirtualButton LTrigger, RTrigger;

		PlayerIndex playerIndex;

		public PlayerInput(PlayerIndex playerIdx)
		{
			playerIndex = playerIdx;
			Left = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.Left) || (istate.padstate.DPad.Left == ButtonState.Pressed); });
			Right = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Right) || (istate.padstate.DPad.Right == ButtonState.Pressed); });
			Down = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Down) || (istate.padstate.DPad.Down == ButtonState.Pressed); });
			Up = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Up) || (istate.padstate.DPad.Up == ButtonState.Pressed); });
			Confirm = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Z) || (istate.padstate.Buttons.B == ButtonState.Pressed); });
			Cancel = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.X) || (istate.padstate.Buttons.A == ButtonState.Pressed); });
			A = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Z) || (istate.padstate.Buttons.B == ButtonState.Pressed); });
			B = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.X) || (istate.padstate.Buttons.A == ButtonState.Pressed); });
			BX = new VirtualButton(this,delegate() { 
				return istate.keystate.IsKeyDown(Keys.Z) || (istate.padstate.Buttons.Y == ButtonState.Pressed)
					|| istate.keystate.IsKeyDown(Keys.S) || (istate.padstate.Buttons.A== ButtonState.Pressed);
			});
			AY = new VirtualButton(this, delegate()
			{
				return istate.keystate.IsKeyDown(Keys.X) || (istate.padstate.Buttons.B == ButtonState.Pressed)
					|| istate.keystate.IsKeyDown(Keys.A) || (istate.padstate.Buttons.X == ButtonState.Pressed);
			});
			MenuX = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.A) || (istate.padstate.Buttons.Y == ButtonState.Pressed); });
			MenuY = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.S) || (istate.padstate.Buttons.X == ButtonState.Pressed); });
			DebugPrint = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.Scroll); });
			F5 = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.F5); });
			F10 = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.F10); });
			LShoulder = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.OemTilde) || (istate.padstate.Buttons.LeftShoulder == ButtonState.Pressed); });
			RShoulder = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.D1) || (istate.padstate.Buttons.RightShoulder == ButtonState.Pressed); });
			LeftMouse = new VirtualButton(this,delegate() { return istate.mousestate.LeftButton == ButtonState.Pressed;  });
			RightMouse = new VirtualButton(this,delegate() { return istate.mousestate.RightButton == ButtonState.Pressed; });
            LTrigger = new VirtualButton(this,delegate() { return istate.padstate.Triggers.Left > 0.5f; });
            RTrigger = new VirtualButton(this,delegate() { return istate.padstate.Triggers.Right > 0.5f; });
			NumPad9 = new VirtualButton(this,delegate() { return istate.keystate.IsKeyDown(Keys.NumPad9); });
			NumPad8 = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.NumPad8); });
			NumPad7 = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.NumPad7); });
			NumPad1 = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.NumPad1); });
			NumPad4 = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.NumPad4); });
			LeftCtrl = new VirtualButton(this, delegate() { return istate.keystate.IsKeyDown(Keys.LeftControl); });
		}
		public InputState istate = new InputState();

		public class InputState {
			public KeyboardState keystate;
			public GamePadState padstate;
			public MouseState mousestate;
		}
		public void UnpressAll()
		{
			Left.Unpress();
			Right.Unpress();
			Down.Unpress();
			Up.Unpress();
			Confirm.Unpress();
			Cancel.Unpress();
			MenuX.Unpress();
			MenuY.Unpress();
			DebugPrint.Unpress();
			LShoulder.Unpress();
			RShoulder.Unpress();
			LeftMouse.Unpress();
			RightMouse.Unpress();
			F5.Unpress();
			F10.Unpress();
            LTrigger.Unpress();
            RTrigger.Unpress();
			A.Unpress();
			B.Unpress();
			BX.Unpress();
			AY.Unpress();
			NumPad9.Unpress();
			NumPad8.Unpress();
			NumPad7.Unpress();
			NumPad1.Unpress();
			NumPad4.Unpress();
			LeftCtrl.Unpress();
		}
		public void Update()
		{
			istate.keystate = Keyboard.GetState();
			istate.padstate = GamePad.GetState(playerIndex);
			istate.mousestate = Mouse.GetState();
			Left.Poll();
			Right.Poll();
			Down.Poll();
			Up.Poll();
			Confirm.Poll();
			Cancel.Poll();
			MenuX.Poll();
			MenuY.Poll();
			DebugPrint.Poll();
			RShoulder.Poll();
			LShoulder.Poll();
			LeftMouse.Poll();
			RightMouse.Poll();
			F5.Poll();
			F10.Poll();
            LTrigger.Poll();
            RTrigger.Poll();
			A.Poll();
			B.Poll();
			BX.Poll();
			AY.Poll();
			NumPad9.Poll();
			NumPad8.Poll();
			NumPad7.Poll();
			NumPad1.Poll();
			NumPad4.Poll();
			LeftCtrl.Poll();
		}

		public bool Dead;

		/// <summary>
		/// represents an unpressable button
		/// </summary>
		public class VirtualButton {

			public delegate bool FetchState();
			FetchState fetcher;
			bool currState = false;
			bool unpressed = false;
			PlayerInput input;

			public VirtualButton(PlayerInput input, FetchState fetcher)
			{
				this.input = input;
				this.fetcher = fetcher;
			}

			public void Poll() {
				currState = fetcher();
				if(unpressed && !currState)
					unpressed = false;
			}

			public void Unpress() {
				if(currState)
					unpressed = true;
			}

			/// <summary>
			/// Returns true if the button is pressed. Then calls unpress.
			/// There is probably no need for this. Look into UnpressAll() instead
			/// </summary>
			public bool SinglePress {
				get {
					bool ret = (bool)this;
					Unpress();
					return ret;
				}
			}

			/// <summary>
			/// casts the button to a bool so you can easily see whether its pressed
			/// </summary>
			public static implicit operator bool(VirtualButton vb) {
				if(vb.unpressed) return false;
				if (vb.input.Dead) return false;
				return vb.currState;
			}
		}
	}
}
