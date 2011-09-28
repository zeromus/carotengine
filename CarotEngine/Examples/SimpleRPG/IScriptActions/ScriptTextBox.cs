using System;
using System.Collections.Generic;
using System.Text;
using pr2.CarotEngine;


namespace Cabedge
{
	public partial class Cabedge
	{
		class ScriptTextBox : ActionScript.IScriptAction
		{
			Cabedge.UiController uic;
			String text;
			bool continues;
			public ScriptTextBox(String text, bool continues, Cabedge.UiController uic)
			{
				this.text = text;
				this.continues = continues;
				this.uic = uic;
			}

			public void invoke()
			{
				uic.textBoxOpen();
				uic.textBoxText(text, continues);
				//throw new Exception("The method or operation is not implemented.");
			}

			public void tick()
			{
				//throw new Exception("The method or operation is not implemented.");
			}

			public bool Finished
			{
				get
				{
					//throw new Exception("The method or operation is not implemented."); 
					//return true;
					return !uic.isTextBoxWaiting();
				}
			}
		}
	}
}
