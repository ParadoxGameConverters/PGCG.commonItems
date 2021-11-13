using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests {
	public class CommonRegexesTests {
		private class TestParser : Parser {
			public readonly Dictionary<string, string> variables = new();
			public TestParser() {
				RegisterRegex(CommonRegexes.Variable, (reader, varStr) =>
					variables.Add(varStr.Substring(1), ParserHelpers.GetString(reader))
				);
			}
		}
		[Fact]
		public void VariableRegexMatchesVariable() {
			var reader = new BufferedReader("@ai_aggresiveness = 70");
			var instance = new TestParser();
			instance.ParseStream(reader);
			Assert.Collection(instance.variables,
				pair => {
					Assert.Equal("ai_aggresiveness", pair.Key);
					Assert.Equal("70", pair.Value);
				}
			);
		}
		[Fact]
		public void VariableRegexDoesNotMatchInterpolatedExpressions() {
			var reader = new BufferedReader("@[100-ai_aggresiveness] = 70");
			var instance = new TestParser();
			instance.ParseStream(reader);
			Assert.Empty(instance.variables);
		}
	}
}
