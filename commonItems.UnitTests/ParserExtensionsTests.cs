using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests; 

public class ParserExtensionsTests {
	[Fact]
	public void UnregisteredItemsCanBeStoredInASet() {
		var reader = new BufferedReader(@"
			a={}
			1 = {}
			ignore_me_1={}
			ignore_me_2 = {}");
		var ignoredTokens = new HashSet<string>();
		
		Assert.Empty(ignoredTokens);
		
		var parser = new Parser();
		parser.RegisterKeyword("a", ParserHelpers.IgnoreItem);
		parser.RegisterRegex(CommonRegexes.Integer, ParserHelpers.IgnoreItem);
		parser.IgnoreAndStoreUnregisteredItems(ignoredTokens);
		
		parser.ParseStream(reader);
		ignoredTokens.Should().BeEquivalentTo("ignore_me_1", "ignore_me_2");
	}
}