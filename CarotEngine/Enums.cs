using System;

namespace pr2.CarotEngine
{
	[Flags]
	public enum Directions
	{
		None=0,
		n=1,s=2,w=4,e=8,
		nw=5,ne=9,sw=6,se=10
	}

}