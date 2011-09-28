using System;
using System.IO;

namespace pr2.Common {
	public class Log {

		static FileStream logfile;
		static StreamWriter streamWriter;

		/// <summary>
		/// initializes the logging subsystem
		/// </summary>
		public static void init() {
			if(streamWriter != null) throw new Exception("Attempting to initialize logging more than once");
			logfile = new FileStream("pr2.log", FileMode.Create, FileAccess.Write, FileShare.Read);
			streamWriter = new StreamWriter(logfile);
			log("Logfile opened: {0}", DateTime.Now);
		}

		/// <summary>
		/// logs the given text. if logging is not enabled, this will silently do nothing
		/// </summary>
		public static void log(string format, params object[] args) {
			if(streamWriter == null) return;
			streamWriter.WriteLine(format,args);
			streamWriter.Flush();
			logfile.Flush();
		}
	}
}