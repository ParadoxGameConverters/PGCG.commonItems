using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests;

public sealed class ParserBuilderTests {
	[Fact]
	public void ParserBuilderBuildsParserWithKeywordRegistration() {
		string? value = null;
		var parser = new ParserBuilder()
			.WithKeyword("key", reader => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader("key = value"));

		Assert.Equal("value", value);
	}

	[Theory]
	[InlineData("key ?= value")]
	[InlineData("key?= value")]
	public void ParserBuilderBuildsParserWithRegexRegistration(string input) {
		string? value = null;
		var parser = new ParserBuilder()
			.WithRegex("[key]+", (reader, _) => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(input));

		Assert.Equal("value", value);
	}

	[Fact]
	public void ParserBuilderCanIgnoreUnregisteredItems() {
		string? value = null;
		var parser = new ParserBuilder()
			.WithKeyword("key", reader => value = reader.GetString())
			.IgnoreUnregisteredItems()
			.Build();

		parser.ParseStream(new BufferedReader("key = value ignored = { nested = value }"));

		Assert.Equal("value", value);
	}

	[Fact]
	public void ParserBuilderCanIgnoreAndStoreUnregisteredItems() {
		var ignoredTokens = new SortedSet<string>();
		var parser = new ParserBuilder()
			.IgnoreAndStoreUnregisteredItems(ignoredTokens)
			.Build();

		parser.ParseStream(new BufferedReader("first = value second = { nested = value }"));

		Assert.Equal(["first", "second"], ignoredTokens);
	}

	[Fact]
	public void ParserBuilderSupportsImplicitVariableHandling() {
		string? name = null;
		var parser = new ParserBuilder(implicitVariableHandling: true)
			.WithKeyword("name", reader => name = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(
			"@best_country_on_earth_name = \"Roman Empire\"\n" +
			"name = @best_country_on_earth_name"
		));

		Assert.Equal("Roman Empire", name);
	}
}