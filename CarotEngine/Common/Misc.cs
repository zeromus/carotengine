using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

//stuff other than static methods that are too tiny to gunk up lib

namespace pr2.Common {

	/// <summary>
	/// A simple no-bullshit callback
	/// </summary>
	public delegate void CallbackArg<T>(T arg);

	/// <summary>
	/// A simple no-bullshit callback
	/// </summary>
	public delegate void CallbackArgArg<T, U>(T arg, U arg2);

	/// <summary>
	/// A simple no-bullshit null callback
	/// </summary>
	public delegate void Callback();

	/// <summary>
	/// A simple no-bullshit callback that returns R
	/// </summary>
	public delegate R CallbackReturnsR<R>();

	/// <summary>
	/// A simple no-bullshit callback that takes T and returns R
	/// </summary>
	public delegate R CallbackArgReturnsR<T,R>(T arg);

	/// <summary>
	/// A class that makes a lightweight strongly-typed array[]-like thing using delegates for the getter and setter
	/// </summary>
	public class SmartArray<T, I> {
		CallbackArgReturnsR<I, T> getter;
		CallbackArgArg<I, T> setter;
		public SmartArray(CallbackArgReturnsR<I, T> getter, CallbackArgArg<I, T> setter) {
			this.getter = getter;
			this.setter = setter;
		}
		public T this[I index] { get { return getter(index); } set { setter(index, value); } }
	}

	/// <summary>
	/// compares two filenames without case sensitivity
	/// and assuming equality for / and \ path separators
	/// </summary>
	public class InsensitiveFilenameComparer : IComparer {

		public int Compare(object x, object y) {
			string xs = x as string;
			string ys = y as string;
			if(xs == null) throw new ArgumentException("InsensitiveFilenameComparer received a non-string or null");
			if(ys == null) throw new ArgumentException("InsensitiveFilenameComparer received a non-string or null");

			return string.Compare(xs.Replace('\\', '/'), ys.Replace('\\', '/'),StringComparison.OrdinalIgnoreCase);
		}
	}

	/// <summary>
	/// A hopefully memory-efficient reusable byte buffer
	/// </summary>
	public class StaticByteBuffer {
		MemoryStream ms;
		public StaticByteBuffer() {
			ms = new MemoryStream();
		}

		public byte[] arr { get { return ms.GetBuffer(); } }

		public void size(int size) {
			if(size > ms.Length) ms.SetLength(size);
		}

		public byte[] read(Stream s, int len) {
			size(len);
			s.Read(arr, 0, len);
			return arr;
		}

		public unsafe void read(void* dest, Stream s, int len) {
			size(len);
			s.Read(arr, 0, len);
			fixed(byte* b = arr)
				Lib.memcpy(dest, b, len);
		}
	}

	///// <summary>
	///// A simple, smart filesystem watcher. since the default one sucks
	///// </summary>
	//public class SmartFileSystemWatcher {

	//    public event Callback callback;

	//    public SmartFileSystemWatcher(int batch, string directory) {
	//        fsw.Path = directory;
	//        fsw.IncludeSubdirectories = true;
	//        fsw.NotifyFilter = NotifyFilters.LastWrite;
	//        fsw.Changed += new FileSystemEventHandler(fsw_Changed);
	//        fsw.EnableRaisingEvents = true;
	//        timer.Interval = batch;
	//        timer.AutoReset = false;
	//        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
	//    }

	//    FileSystemWatcher fsw = new FileSystemWatcher();
	//    System.Timers.Timer timer = new System.Timers.Timer();

	//    bool pending;

	//    void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
	//        timer.Stop();
	//        pending = true;
	//    }

	//    /// <summary>
	//    /// you have to keep ticking this so you can control the thread from which the event fires
	//    /// </summary>
	//    public void update() {
	//        if(pending) {
	//            if(callback != null) callback();
	//            pending = false;
	//        }
	//    }

	//    void fsw_Changed(object sender, FileSystemEventArgs e) {
	//        timer.Stop();
	//        timer.Start();
	//    }
	//}

	/// <summary>
	/// Manages a cache of disposable resources, keyed by a structure
	/// </summary>
	public class DisposableCacheManager<T, TINFO> : IDisposable where TINFO : struct, IDisposableCacheInfo<T> {
		Dictionary<TINFO, T> cache = new Dictionary<TINFO, T>();
		public T Get(TINFO info) {
			T ret = cache.TryGetValue(info, out ret) ? ret : (cache[info] = info.Create());
			return ret;
		}
		public void Dispose() {
			foreach(KeyValuePair<TINFO, T> kvp in cache)
				kvp.Key.Dispose(kvp.Value);
		}
		public T this[TINFO info] { get { return Get(info); } }
	}

	/// <summary>
	/// The key structure for a DisposableCacheManager
	/// </summary>
	public interface IDisposableCacheInfo<T> {
		T Create();
		void Dispose(T item);
	}
}