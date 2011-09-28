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
	partial class Cabedge {

		public class Party {
			public Party clone() {
				Party ret = new Party();
				ret.chars = new List<Character>();
				foreach(Character c in chars)
					ret.chars.Add(c.clone());
				ret.gp = gp;
				ret.xp = xp;
				ret.xpt = xpt;
				ret.level = level;
				ret.availableEnhancements = new List<Enhancement>(availableEnhancements);
				ret.equippedEnhancements = (Enhancement[])equippedEnhancements.Clone();
				return ret;
			}
			public int numChars { get { return chars.Count; } }
			public List<Character> chars = new List<Character>();
			public int xp = 1531, xpt = 4128;
			public int gp = 4424;
			public int level = 27;

			public List<Enhancement> availableEnhancements = new List<Enhancement>();
			public Enhancement[] equippedEnhancements = new Enhancement[10];

			//enhanced characteristics
			public bool enhUnsurprisable;
		}
		
		/// <summary>
		/// the current party (with enhancements applied)
		/// </summary>
		protected Party party;

		/// <summary>
		/// the base party (without enhancements)
		/// </summary>
		protected Party baseParty = new Party();

		/// <summary>
		/// updates the current party from the base party
		/// </summary>
		void updateParty() {
			party = baseParty.clone();
			Enhancements.applyEnhancements(party);
		}

		public class Character {
			public Character clone() {
				Character ret = new Character(name, classname);
				ret.hp = hp;
				ret.chr = chr;
				ret.baseStats = baseStats;
				ret.bonusStats = bonusStats;
				return ret;
			}

			public Character(string name, string classname) { 
				this.name = name; 
				this.classname = classname;
			}
			public string name, classname;
			public int hp=80;
			V3Chr chr;

			public struct Stats {
				public int mhp;
				public int strength, intelligence, defense, agility;
				public void clear() {
					mhp = strength = intelligence = defense = agility = 0;
				}
			}

			public Stats baseStats;
			public Stats bonusStats;

			public void loadChar(string fname) {
				chr = new V3Chr(fname);
			}

			public Image getPortrait() {
				if(chr == null) return null;
				return chr.frames[chr.idleFrames[(int)Directions.s]];
			}
		}
	}
}