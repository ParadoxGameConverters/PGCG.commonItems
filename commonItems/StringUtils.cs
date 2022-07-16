namespace commonItems;

public static class StringUtils {
	public static string RemQuotes(this string str) {
		var length = str.Length;
		if (length < 2) {
			return str;
		}
		if (!str.StartsWith('"') || !str.EndsWith('"')) {
			return str;
		}
		return str.Substring(1, length - 2);
	}

	public static string AddQuotes(this string str) {
		if (str.Length > 2 && str.StartsWith('"') && str.EndsWith('"')) {
			return str;
		}

		if (!str.StartsWith('"') && !str.EndsWith('"')) {
			return "\"" + str + "\"";
		}
		return str;
	}
}