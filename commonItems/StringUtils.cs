namespace commonItems;

public static class StringUtils {
	public static bool IsQuoted(this string str) {
		if (str.Length < 2) return false;
		return str[0] == '"' && str[^1] == '"';
	}
	
	public static string RemQuotes(this string str) {
		var length = str.Length;
		if (length < 2) {
			return str;
		}
		if (str[0] != '"' || str[^1] != '"') {
			return str;
		}
		return str[1..^1];
	}

	public static string AddQuotes(this string str) {
		if (str.Length > 2 && str[0] == '"' && str[^1] == '"') {
			return str;
		}

		if (str.Length > 0 && (str[0] == '"' || str[^1] == '"')) {
			return str;
		}

		return string.Concat("\"", str, "\"");
	}
}