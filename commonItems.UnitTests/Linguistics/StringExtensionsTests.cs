using commonItems.Linguistics;
using Xunit;

namespace commonItems.UnitTests.Linguistics; 

public class StringExtensionsTests {
	[Theory]
	[InlineData("Cook Islands", "Cook")]
	[InlineData("Dominican Republic", "Dominican")]
	[InlineData("Soviet Union", "Soviet")]
	[InlineData("Scotland", "Scottish")]
	[InlineData("Lapland", "Lappish")]
	[InlineData("Finland", "Finnish")]
	[InlineData("England", "English")]
	[InlineData("Iceland", "Icelandic")]
	[InlineData("Switzerland", "Swiss")]
	[InlineData("Thailand", "Thai")]
	[InlineData("France", "French")]
	[InlineData("Lebanon", "Lebanese")]
	[InlineData("Nuremberg", "Nuremberger")]
	[InlineData("Hamburg", "Hamburger")]
	[InlineData("Indochina", "Indochinese")]
	[InlineData("Greece", "Greek")]
	[InlineData("Senegal", "Senegalese")]
	[InlineData("United Arab Emirates", "United Arab Emirati")]
	[InlineData("Denmark", "Danish")]
	[InlineData("Ghana", "Ghanaian")]
	// TODO: add the rest below Ghana
	public void GetAdjectiveGeneratesCorrectishAdjective(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}
}