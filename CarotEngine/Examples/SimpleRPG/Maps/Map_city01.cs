using System;
using System.Collections.Generic;
using System.Text;

namespace Cabedge
{
	public partial class Cabedge
	{
		public class Map_city01 : CabedgeMapScript
		{
			public void OnLoad()
			{
				CreatePlayer("raw/man5.chr", 50, 24);

			}
			public void Test1()
			{
				StartScript();
				MoveEntity(args.activatedEntity, "R5");
				CommitScript();
			}
		}
	}
}
