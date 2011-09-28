using System;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	public class StandardChrAnimationController {
		int currWait;
		int index;
		int _frame;
		string _animString;
		int[] ops;
		static Regex splitter = new Regex("(W|F)\\d+", RegexOptions.Compiled);

		public StandardChrAnimationController(string str) { setAnimString(str); }
		public void reset() {
			index = 0;
			_frame = 0;
			run();
		}

		private void setAnimString(string str) {
			if(str == null) { ops = null; _frame = 0; return; }
			//index = 0;
			//_frame = 0;
			_animString = str;
			//for now, we will just parse it into sequences of frames and waits
			MatchCollection elems = splitter.Matches(str);
			ops = new int[elems.Count];
			for(int i=0; i<elems.Count; i++) {
				int num=0;
				bool bWait = false;
				foreach(char c in elems[i].Value) {
					if(c == 'W') { bWait = true; continue; } else if(c == 'F') continue;
					num *= 10;
					num += (c-'0');
				}
				//adjustment from 100hz for 1000hz for carotengine ticklength 
				if(bWait) num *= 10;

				if(bWait) num = num|unchecked((int)0x80000000);

				ops[i] = num;
			}
			//run();
		}

		void run() {
			for(; ; ) {
				if(index==ops.Length) index=0;
				if((ops[index]&unchecked((int)0x80000000)) == 0) _frame = ops[index++];
				else { currWait = ops[index++]&0x7FFFFFFF; return; }
			}
		}

		public int frame { get { return _frame; } }
		public string animString { get { return _animString; } set { setAnimString(value); } }

		public void tick() {
			if(ops == null) return;
			currWait--;
			if(currWait==0) run();
		}
	}

}