using AnyAscii;

namespace commonItems;

/// <summary>
/// This class converts alphabetic, numeric, and symbolic Unicode characters
/// which are not in the first 127 ASCII characters (the "Basic Latin" Unicode
/// block) into their ASCII equivalents, if one exists.
/// <para/>
/// For example, '&amp;agrave;' will be replaced by 'a'.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Converts characters above ASCII to their ASCII equivalents. For example,
	/// accents are removed from accented characters. 
	/// </summary>
	/// <param name="input">     The string of characters to fold </param>
	/// <returns> ASCII string </returns>
	public static string FoldToASCII(this string input) {
		return input.Transliterate();
	}
}