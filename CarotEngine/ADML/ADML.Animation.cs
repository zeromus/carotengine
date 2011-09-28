#if !XBOX

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {


	public interface IAnimScript {
		void Reset();
		int Tick();
		string Script { set; get; }
		int Frame { get; }
		int Length { get; }
	}

	/// <summary>
	/// Encapsulates a preview of an animscript.
	/// time values should be in ns (nanoseconds)
	/// </summary>
	public class AnimScriptPreview {
		public class FrameInfo {
			public int frame;
			public long duration;
		}
		public List<FrameInfo> frames = new List<FrameInfo>();
	}

	public class V3ChrAnimationScript : IAnimScript {
		int currWait;
		int index;
		int _frame;
		string _script;
		int _length;
		int[] ops;
		bool nodelay;
		static Regex splitter = new Regex("(W|F)\\d+", RegexOptions.Compiled);

		public int Length { get { return _length; } }

		public AnimScriptPreview preview() {
			AnimScriptPreview ret = new AnimScriptPreview();
			AnimScriptPreview.FrameInfo fi = null;
			foreach(int n in ops) {
				if((n & unchecked((int)0x80000000)) != 0) {
					if(fi != null)
						fi.duration = (n & 0x7FFFFFFF) * 1000000;
				} else {
					if(fi != null)
						ret.frames.Add(fi);
					fi = new AnimScriptPreview.FrameInfo();
					fi.frame = n;
					fi.duration = 1000000000; //a default duration of 1sec
				}
			}
			if(fi != null) ret.frames.Add(fi);
			return ret;
		}

		public V3ChrAnimationScript(string str) { setAnimString(str); }
		public void Reset() {
			index = 0;
			_frame = 0;
			run();
		}

		private void setAnimString(string str) {
			nodelay = false;
			if(str == null) { ops = null; _frame = -1; return; }
			_length = 0;
			//index = 0;
			//_frame = 0;
			_script = str;
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
				//adjustment for angelwing ticklength 
				if(bWait) {
					num *= 10;
					_length += num;
					num = num | unchecked((int)0x80000000);
				}

				ops[i] = num;
			}
			//run();
		}

		void run() {

			bool loop = false;
			for(; ; ) {
				if(index == ops.Length) {
					if(loop) {
						nodelay = true;
						_frame = ops[0];
						return;
					}
					loop = true;
					index = 0;
				}

				if((ops[index]&unchecked((int)0x80000000)) == 0) _frame = ops[index++];
				else { currWait = ops[index++] & 0x7FFFFFFF; return; }
			}
		}

		public int Frame { get { return _frame; } }
		public string Script { get { return _script; } set { setAnimString(value); } }

		public int Tick() {
			if(nodelay) return _frame;
			if(ops == null) return -1;
			currWait--;
			if(currWait==0) run();
			return _frame;
		}
	}

	

}

#endif