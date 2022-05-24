using Xunit;

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
}