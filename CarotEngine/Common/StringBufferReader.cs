namespace pr2.Common
{
	/// <summary>
	/// takes a char buffer from a stream, and return it as a string (stripping off all the trailing nulls)
	/// </summary>
	public class StringBufferReader
	{
		static byte[] strbuf = new byte[256];
		public static string read(System.IO.Stream s, int len)
		{
			if(len>strbuf.Length) strbuf = new byte[len];
			s.Read(strbuf,0,len);
			return System.Text.Encoding.UTF8.GetString(strbuf,0,len).TrimEnd('\0');
		}
	}
}