using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pr2.Common {
	/// <summary>
	/// Library of static methods for processing strings
	/// </summary>
	public static class Str {
		/// <summary>
		/// This is a convenience method to minimize casting in Enum.Parse...
		/// </summary>
		public static K ToEnum<K>(string value) {
			if(!typeof(K).IsEnum)
				throw new Exception("Str.ToEnum must be called with enumeration types only!");
			return (K)Enum.Parse(typeof(K), value,true);
		}


		/// <summary>
		/// Converts an array of objects to comma-separated list of .ToString() values for each object
		/// </summary>
		public static string CommaSeparatedList(bool includeSpace, params string[] values) { return CommaSeparatedList(values, "", includeSpace); }
		public static string CommaSeparatedList(params object[] values) { return CommaSeparatedList(values, ""); }
		public static string CommaSeparatedList(string quoteSymbol, params object[] values) { return CommaSeparatedList(values, quoteSymbol); }
		public static string CommaSeparatedList(IEnumerable array) { return CommaSeparatedList(array, ""); }
		public static string CommaSeparatedList(IEnumerable array, string quoteSymbol) { return CommaSeparatedList(array, quoteSymbol, false); }
		public static string CommaSeparatedList(IEnumerable array, string quoteSymbol, bool includeSpace) {
			IEnumerator enumerator = array.GetEnumerator();
			StringBuilder list = new StringBuilder();
			while(enumerator.MoveNext())
				if(enumerator.Current != null) {
					list.Append(quoteSymbol + enumerator.Current.ToString() + quoteSymbol);
					break;
				}

			while(enumerator.MoveNext()) {
				list.Append(includeSpace ? ", " : ",");
				list.Append(quoteSymbol + enumerator.Current.ToString() + quoteSymbol);
			}
			return list.ToString();
		}

		/// <summary>
		/// Convert an array to a string with separated items (uses .ToString() on every array item)
		/// </summary>
		public static string SeparatedList(IEnumerable array, string separator) {
			IEnumerator enumerator = array.GetEnumerator();
			StringBuilder list = new StringBuilder();
			if(enumerator.MoveNext())
				list.Append(enumerator.Current.ToString());

			while(enumerator.MoveNext()) {
				list.Append(separator);
				list.Append(enumerator.Current.ToString());
			}
			return list.ToString();
		}

		/// <summary>
		/// conerts bytes to an uppercase string of hex numbers in upper case without any spacing or anything
		/// </summary>
		public static string BytesToHexString(byte[] bytes) {
			StringBuilder sb = new StringBuilder();
			foreach(byte b in bytes)
				sb.AppendFormat("{0:X2}", b);
			return sb.ToString();
		}

		/// <summary>
		/// converts a hexadecimal string to an array of bytes
		/// </summary>
		public static byte[] HexStringToBytes(string hex) {
			hex = hex.ToLower();
			if((hex.Length & 1) != 0) throw new ArgumentException("Input hex string not an even number of chars", "hex");
			int byteCount = hex.Length / 2;
			byte[] data = new byte[byteCount];
			for(int j = 0; j < byteCount; j++) {
				char h = hex[j * 2];
				char l = hex[j * 2 + 1];
				byte b = 0;
				if(h >= '0' && h <= '9') b = (byte)((int)(h - '0') << 4);
				else b = (byte)(((int)(h - 'a') + 10) << 4);
				if(l >= '0' && l <= '9') b += (byte)((int)(l - '0'));
				else b += (byte)(((int)(l - 'a') + 10));
				data[j] = b;
			}
			return data;
		}

		/// <summary>
		/// VB-Style Left$ function (reflected from Microsoft.VisualBasic.dll / Microsoft.VisualBasic.Strings.Left)
		/// </summary>
		public static string Left(string str, int Length) {
			if(Length < 0) {
				throw new ArgumentException("Argument 'Length' must be greater than or equal to zero");
			}
			if((Length == 0) || (str == null)) {
				return "";
			}
			if(Length >= str.Length) {
				return str;
			}
			return str.Substring(0, Length);
		}

		/// <summary>
		/// VB-Style Right$ function (reflected from Microsoft.VisualBasic.dll / Microsoft.VisualBasic.Strings.Right)
		/// </summary>
		public static string Right(string str, int Length) {
			if(Length < 0) {
				throw new ArgumentException("Argument 'Length' must be greater than or equal to zero");
			}
			if((Length == 0) || (str == null)) {
				return "";
			}
			int num1 = str.Length;
			if(Length >= num1) {
				return str;
			}
			return str.Substring(num1 - Length, Length);
		}


		/// <summary>
		/// VB-Style Mid$ function (reflected from Microsoft.VisualBasic.dll / Microsoft.VisualBasic.Strings.Mid)
		/// </summary>
		public static string Mid(string str, int Start) {
			string text1;
			try {
				if(str == null) {
					return null;
				}
				text1 = Mid(str, Start, str.Length);
			} catch(Exception exception1) {
				throw exception1;
			}
			return text1;
		}

		/// <summary>
		/// VB-Style Mid$ function (reflected from Microsoft.VisualBasic.dll / Microsoft.VisualBasic.Strings.Mid)
		///  and fixed to be zero-based
		/// </summary>
		public static string Mid(string str, int Start, int Length) {
			if(Start < 0) {
				throw new ArgumentException("Argument 'Start' must be greater than or equal to zero");
			}
			if(Length < 0) {
				throw new ArgumentException("Argument 'Length' must be greater than or equal to zero");
			}
			if((Length == 0) || (str == null)) {
				return "";
			}
			int num1 = str.Length;
			if(Start > num1) {
				return "";
			}
			if((Start + Length) > num1) {
				return str.Substring(Start);
			}
			return str.Substring(Start, Length);
		}

		/// <summary>
		/// Splits a string using a string as the divider. This has been cursorily tested
		/// </summary>
		public static IList<string> Split(string str, string separator) {
			if(string.IsNullOrEmpty(separator)) throw new ArgumentException("Argument 'separator' must be non-null and non-empty", "separator");

			int ofs = 0;
			List<string> ret = new List<string>();
			if(str.Length == 0) return ret;

			List<int> splits = new List<int>();
			splits.Add(0);

			int separatorLen = separator.Length;
			for(; ; ) {
				int idx = str.IndexOf(separator, ofs);
				if(idx == -1) break;
				splits.Add(idx);
				ofs = idx + separatorLen;
			}

			int chunks = splits.Count;
			splits.Add(str.Length);
			for(int i = 0; i < chunks; i++) {
				int start = splits[i] + (i == 0 ? 0 : separatorLen);
				int len = splits[i + 1] - start;
				ret.Add(str.Substring(start, len));
			}

			return ret;
		}

		/// <summary>
		/// Reads an ascii string from a file whose length is stored first as a short.
		/// Of course, we can never be sure whether this will be a ushort or a short. alas. Lets hope its a ushort
		/// </summary>
		public static string ReadPrefix16(System.IO.Stream s) {
			BinaryReader br = new BinaryReader(s);
			ushort len = br.ReadUInt16();
			byte[] buf = new byte[len];
			s.Read(buf, 0, len);
			return System.Text.Encoding.UTF8.GetString(buf, 0, len);
		}

		public static void WritePrefix16(System.IO.Stream s, string str) {
			BinaryWriter bw = new BinaryWriter(s);
			bw.Write((ushort)str.Length);
			byte[] buf = System.Text.Encoding.UTF8.GetBytes(str);
			s.Write(buf, 0, str.Length);
		}



	}
}