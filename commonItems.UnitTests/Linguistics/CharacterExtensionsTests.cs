using commonItems.Linguistics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace commonItems.UnitTests.Linguistics; 

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class CharacterExtensionsTests {
	[Theory]
	[InlineData("aeiou", true)]
	[InlineData("qwrtxcvb", false)]
	public void IsVowelReturnsCorrectValue(IEnumerable<char> testCharacter, bool expectedValue) {
		foreach (var c in testCharacter) {
			Assert.Equal(expectedValue, c.IsVowel());
		}
	}
}