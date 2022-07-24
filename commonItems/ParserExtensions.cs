using System.Collections.Generic;

namespace commonItems; 

public static class ParserExtensions {
	public static void IgnoreUnregisteredItems(this Parser parser) {
		parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreItem);
	}
	public static void IgnoreAndLogUnregisteredItems(this Parser parser) {
		parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
	}
	/// <summary>
	/// Makes <paramref name="parser"/> ignore unregistered keywords and store them in <paramref name="ignoredTokenSet"/>
	/// </summary>
	/// <param name="parser">Parser to be modified</param>
	/// <param name="ignoredTokenSet">Set for storing ignored tokens</param>
	public static void IgnoreAndStoreUnregisteredItems(this Parser parser, ISet<string> ignoredTokenSet) {
		parser.RegisterRegex(CommonRegexes.Catchall, (reader, token) => {
			ignoredTokenSet.Add(token);
			ParserHelpers.IgnoreItem(reader);
		});
	}
}