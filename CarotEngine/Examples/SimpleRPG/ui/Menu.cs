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


namespace Cabedge {
	public partial class Cabedge {

		/// <summary>
		/// abstract representation of menu. it may be controlled or rendered in any way
		/// </summary>
		class Menu {
			public Menu() { }

			//sub-items. leaf menus wont have any items
			public List<Menu> items = new List<Menu>();

			public bool isEnterable = false;

			/// <summary>
			/// initiates the population procedure
			/// </summary>
			internal void populate() {
				onPopulate();
			}

			/// <summary>
			/// this is called when one needs to populate the sub item list
			/// </summary>
			virtual internal void onPopulate() {
			}

			/// <summary>
			/// It may help you to put a label here
			/// </summary>
			public string label;

			/// <summary>
			/// this is called when the item is activated
			/// </summary>
			public virtual void activate() { }

		}

		/// <summary>
		/// A simple menu that is naught but a label
		/// </summary>
		class LabelMenu : Menu {
			public LabelMenu(string label) { this.label = label; }
		}

		/// <summary>
		/// abstract controller for a menu which has a linear item list
		/// </summary>
		class MenuNavigator {
			public Menu root;

			public List<Menu> menus = new List<Menu>();
			public List<int> selections = new List<int>();
			public int nestLevel;

			/// <summary>
			/// set this to true if you want the selection behavior to wrap around
			/// </summary>
			public bool wrap = false;

			public Menu activeMenu { get { return menus[nestLevel]; } }
			public int activeSelection { get { return selections[nestLevel]; } }

			/// <summary>
			/// returns the menu at the specified nest level.
			/// conveniently returns null if that nest level is not active
			/// </summary>
			public Menu menuAt(int index) {
				if(index > nestLevel) return null;
				else return menus[index];
			}

			/// <summary>
			/// returns the selection at the specified nest level
			/// conveniently returns -1 if that nest level is not active
			/// </summary>
			public int selectionAt(int index) {
				if(index > nestLevel) return -1;
				else return selections[index];
			}

			/// <summary>
			/// returns the currently selected submenu at the specified nestlevel
			/// conveniently returns null if that nest level is not active
			/// </summary>
			public Menu submenuAt(int index) {
				if(index > nestLevel) return null;
				else return menus[index].items[selections[index]];
			}

			public void start() {
				menus.Clear();				
				selections.Clear();
				nestLevel = -1;
			}

			public void next() {
				if(wrap)
					selections[nestLevel] = Lib.listWrap(selections[nestLevel], 1, 0, menus[nestLevel].items.Count);
				else
					selections[nestLevel] = Lib.listClip(selections[nestLevel], 1, 0, menus[nestLevel].items.Count);
				forcePopulate();
			}

			public void prev() {
				if(wrap)
					selections[nestLevel] = Lib.listWrap(selections[nestLevel], -1, 0, menus[nestLevel].items.Count);
				else
					selections[nestLevel] = Lib.listClip(selections[nestLevel], -1, 0, menus[nestLevel].items.Count);
				forcePopulate();
			}

			/// <summary>
			/// forces the current selection to populate
			/// </summary>
			void forcePopulate() {
				if(submenuAt(nestLevel) != null)
					submenuAt(nestLevel).populate();
			}

			/// <summary>
			/// returns whether or not the user is in the menu. when all levels have been exited, the menu is exited
			/// </summary>
			public bool IsInMenu { get { return nestLevel != -1; } }

			/// <summary>
			/// enters the current menu item
			/// </summary>
			void enter() {
				if(nestLevel == -1) {
					selections.Add(0);
					menus.Add(root);
				} else {
					selections.Add(0);
					//an exception will be thrown if this item is not enterable
					menus.Add(menus[nestLevel].items[selections[nestLevel]]);
				}
				nestLevel++;
				forcePopulate();
			}

			public void activate() {
				if(nestLevel == -1)
					enter();
				else if(submenuAt(nestLevel).isEnterable)
					enter();
				else
					submenuAt(nestLevel).activate();
			}

			/// <summary>
			/// exit a level of the menu
			/// </summary>
			public void exit() {
				if(nestLevel == -1) throw new ApplicationException("Attempted to exit an empty menu");
				selections.RemoveAt(nestLevel);
				menus.RemoveAt(nestLevel);
				nestLevel--;
			}
		}


	}
}