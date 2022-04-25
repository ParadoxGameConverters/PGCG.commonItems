using System.Text;

namespace commonItems; 

public static class EncodingConversions {
	public static string ConvertUTF8ToASCII(string utf8String) {
		return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(utf8String)));
	}
}