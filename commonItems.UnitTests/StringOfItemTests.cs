using Xunit;

namespace commonItems.UnitTests;

public sealed class StringOfItemTests {
	[Fact]
	public void StringOfItemConvertsBracedObjectsToStrings() {
		const string input =
			"""
			= {
			                \t{
			                \t\tid = 180
			                \t\ttype = 46
			                \t}
			                }
			""";
		var reader = new BufferedReader(input);

		var theItem = reader.GetStringOfItem();
		Assert.Equal(input[2..], theItem.ToString());
	}

	[Fact]
	public void StringOfItemConvertsBracedObjectsToStringsOnExistsEquals() {
		const string input =
			"""
			?= {
			                \t{
			                \t\tid = 180
			                \t\ttype = 46
			                \t}
			                }
			""";
		var reader = new BufferedReader(input);

		var theItem = reader.GetStringOfItem();
		Assert.Equal(input[3..], theItem.ToString()); // without the "?= " at the start
	}

	[Fact]
	public void StringOfItemHandlesQuotedCurlyBracesInString() {
		string input = "= \"blah { blah \"";
		var reader = new BufferedReader(input);
		var stringOfItem = reader.GetStringOfItem();
		Assert.Equal(input[2..], stringOfItem.ToString());

		input = "= \"blah } blah \"";
		reader = new BufferedReader(input);
		stringOfItem = reader.GetStringOfItem();
		Assert.Equal(input[2..], stringOfItem.ToString());
	}

	[Fact]
	public void StringOfItemHandlesNestedQuotedCurlyBraces() {
		string input =
			"""
			= {
				{
					id = "{"
					type = 46
				} bla
			}
			""";
		StringOfItem stringOfItem = new(new BufferedReader(input));
		Assert.Equal(input[2..], stringOfItem.ToString());

		input =
			"""
			= {
				{
					id = "}"
					type = 46
				} bla
			}
			""";
		stringOfItem = new(new BufferedReader(input));
		Assert.Equal(input[2..], stringOfItem.ToString());
	}

	[Fact]
	public void StringOfItemGetsStringAfterEquals() {
		var reader = new BufferedReader(" = foo");
		var theItem = reader.GetStringOfItem();
		Assert.Equal("foo", theItem.ToString());
	}

	[Fact]
	public void StringOfItemHandlesItemContainingStringWithEscapedQuote() {
		const string input =
			"""
			= {
				foo = "some junk\"}"
				bar = baz
			}
			""";
		var reader = new BufferedReader(input);
		var theItem = reader.GetStringOfItem();
		Assert.Equal(input[2..], theItem.ToString());
	}

	[Fact]
	public void StringOfItemCanBeConstructedFromString() {
		const string str = "{ key=value }";
		Assert.Equal(str, new StringOfItem(str).ToString());
	}

	[Theory]
	[InlineData("string")]
	[InlineData("2")]
	[InlineData("4.56")]
	[InlineData("\"{ quoted object }\"")]
	public void IsArrayOrObjectReturnsFalseForSimpleValues(string str) {
		var stringItem = new StringOfItem(str);
		Assert.False(stringItem.IsArrayOrObject());
	}

	[Theory]
	[InlineData("{ field1=1 field2=2 }")]
	[InlineData("{ 1 2 3 }")]
	[InlineData("rgb { 40 70 50 }")]
	public void IsArrayOrObjectReturnsTrueForArraysAndObjects(string str) {
		var stringItem = new StringOfItem(str);
		Assert.True(stringItem.IsArrayOrObject());
	}
}