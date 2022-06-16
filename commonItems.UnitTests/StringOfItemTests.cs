﻿using Xunit;

namespace commonItems.UnitTests;

public class StringOfItemTests {
	[Fact]
	public void StringOfItemConvertsBracedObjectsToStrings() {
		const string input =
			@"= {\n
                \t{\n
                \t\tid = 180\n
                \t\ttype = 46\n
                \t}\n
                }";
		var reader = new BufferedReader(input);

		var theItem = reader.GetStringOfItem();
		Assert.Equal(input[2..], theItem.ToString());
	}

	[Fact]
	public void StringOfItemGetsStringAfterEquals() {
		var reader = new BufferedReader(" = foo");
		var theItem = reader.GetStringOfItem();
		Assert.Equal("foo", theItem.ToString());
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