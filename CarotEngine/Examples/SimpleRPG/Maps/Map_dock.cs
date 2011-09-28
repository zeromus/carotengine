using System;
using System.Collections.Generic;
using System.Text;
using pr2.CarotEngine;

namespace Cabedge
{
	public partial class Cabedge
	{
		public class Map_dock : CabedgeMapScript
		{
			public void onLoad(String startLoc)
			{
				if (startLoc == "barExit")
				{
					CreatePlayer("raw/man5.chr", 16, 28);
					EntitySetFacing(GetPlayer(), Directions.s);
				}
				else
				{
					CreatePlayer("raw/man5.chr", 7, 15);
					EntitySetFacing(GetPlayer(), Directions.n);
				}
			}

			public void EnterBar()
			{
				SwitchMap("raw/dockbar.map");
			}

			public void LockedDoor()
			{
				StartScript();
					PauseEntity(GetPlayer());
					TextBox("OPEN THE GOD DAMN DOOR IM NOT EVEN KIDDING", true);
					TextBox("OK JUST KIDDING.", false);
					ResumeEntity(GetPlayer());

					WaitSteps(GetPlayer(), 3);
					ScriptCallback("HeyWait");
					
				CommitScript();
			}

			public void HeyWait()
			{
				StartScript();
				
				PauseEntity(GetPlayer());
				TextBox("Hey. :( Come back!", false);
				ResumeEntity(GetPlayer());

				CommitScript();
			}
		}
	}
}
