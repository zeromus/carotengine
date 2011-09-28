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

		public class Enhancement {
			public Enhancement(string name, string tooltip) {
				this.name = name; this.tooltip = tooltip;
			}
			public string name, tooltip;
			public int salePrice;
		}

		static class Enhancements {
			public static Enhancement mhp_plus_100 = new Enhancement("MHP +100", "Increases all characters' max HP by 100");
			public static Enhancement str_plus_10 = new Enhancement("STR +10", "Increases all characters' Strength by 10");
			public static Enhancement premonition = new Enhancement("Premonition", "Party cannot be surprised by opponents");


			/// <summary>
			/// Applies the party's set of enhancements to itsself.
			/// </summary>
			public static void applyEnhancements(Party party) {
				foreach(Enhancement enh in party.equippedEnhancements) {
					if(enh == null) continue;

					Character.Stats allStatDelta = new Character.Stats();
					if(enh == Enhancements.mhp_plus_100) allStatDelta.mhp = 100;
					if(enh == Enhancements.str_plus_10) allStatDelta.strength = 10;

					if(enh == Enhancements.premonition) party.enhUnsurprisable = true;
					
					//in case we need to apply something to each character
					applyToEachCharacter(party, allStatDelta);
				}
			}

			static void applyToEachCharacter(Party party, Character.Stats statDelta) {
				foreach(Character character in party.chars)
					applyToCharacter(character, statDelta);
			}

			static void applyToCharacter(Character character, Character.Stats statDelta) {
				character.bonusStats.agility += statDelta.agility;
				character.bonusStats.defense += statDelta.defense;
				character.bonusStats.intelligence += statDelta.intelligence;
				character.bonusStats.mhp += statDelta.mhp;
				character.bonusStats.strength += statDelta.strength;
			}

		}
	}
}