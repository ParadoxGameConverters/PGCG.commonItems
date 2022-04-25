using Xunit;

namespace commonItems.UnitTests; 

public class CommonRegexesTests {
	private class TestParser : Parser { }
	[Fact]
	public void VariableRegexMatchesVariables() {
		var reader = new BufferedReader("@ai_aggressiveness = 70");
		var instance = new TestParser();
		instance.ParseStream(reader);
		Assert.Collection(reader.Variables,
			pair => {
				var (key, value) = pair;
				Assert.Equal("ai_aggressiveness", key);
				Assert.Equal(70, value);
			}
		);
	}
	[Fact]
	public void VariableRegexDoesNotMatchInterpolatedExpressions() {
		var reader = new BufferedReader("@[100-ai_aggressiveness] = 70");
		var instance = new TestParser();
		instance.ParseStream(reader);
		Assert.Empty(reader.Variables);
	}

	[Fact]
	public void InterpolatedExpressionRegexMatchesInterpolatedExpressions() {
		Assert.Matches(CommonRegexes.InterpolatedExpression, "@[100-ai_aggressiveness]");
	}
	[Fact]
	public void InterpolatedExpressionRegexMatchesVariables() {
		Assert.Matches(CommonRegexes.InterpolatedExpression, "@ai_aggressiveness");
	}
}