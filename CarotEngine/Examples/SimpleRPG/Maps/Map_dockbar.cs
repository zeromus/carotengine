using System;
using System.Collections.Generic;
using System.Text;

namespace Cabedge
{
	public partial class Cabedge
	{
		public class Map_dockbar : CabedgeMapScript
		{
			public void onLoad(String startLoc)
			{
                CreatePlayer("raw/man5.chr", 3, 11);
			}

			public void ExitDocks()
			{
				SwitchMap("raw/dock.map", "barExit");
			}
		}
	}
}
