using System;
using System.Collections.Generic;
using System.Text;
using pr2.CarotEngine;

namespace Cabedge
{
	public partial class Cabedge
	{
		public class CabedgeMapScript : pr2.CarotEngine.Maps.MapScript
		{
			protected void TextBox(String text, bool textContinues)
			{
				//AddUserScript(new ScriptTextBox(text, textContinues, cabedge.gameController.uiController));
			}
		}
	}
}
