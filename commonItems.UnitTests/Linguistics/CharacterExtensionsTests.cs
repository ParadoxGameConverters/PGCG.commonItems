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
	[InlineData(".,/?-(+=@$%^&*!`1\t\"", false)]
	public void IsVowelReturnsCorrectValue(IEnumerable<char> testCharacter, bool expectedValue) {
		foreach (var c in testCharacter) {
			Assert.Equal(expectedValue, c.IsVowel());
		}
	}
	
	[Theory]
	[InlineData("qwrtpśćfghjklzxbnmłńżź", true)]
	[InlineData("eyoa", false)]
	[InlineData(".,/?-(+=@$%^&*!`1\t\"", false)]
	public void IsConsonantReturnsCorrectValue(IEnumerable<char> testCharacter, bool expectedValue) {
		foreach (var c in testCharacter) {
			Assert.Equal(expectedValue, c.IsConsonant());
		}
	}
}