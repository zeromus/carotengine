using System;
using System.Collections.Generic;
using System.Text;

#if XBOX360

namespace pr2.Common
{
	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

}


#endif