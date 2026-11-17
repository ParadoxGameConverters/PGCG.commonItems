using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class BufferedReaderTests {
	[Fact]
	public void BufferedReaderReadsCorrectly() {
		var stream = new MemoryStream("12345"u8.ToArray());
		var reader = new BufferedReader(stream);
		var read = reader.Read(2);
		Assert.Equal("12", read);
		Assert.Equal("345", reader.ReadToEnd());
	}
	[Fact]
	public void BufferedReaderAllowsPushBackOfOneChar() {
		var stream = new MemoryStream("12345"u8.ToArray());
		var reader = new BufferedReader(stream);
		reader.Read(); // in stream: 2345
		reader.Read(); // in stream: 345
		reader.PushBack('2'); // in stream: 2345
		Assert.Equal("2345", reader.ReadToEnd());
	}
	[Fact]
	public void BufferedReaderAllowsPushBackOfMultipleChars() {
		var stream = new MemoryStream("12345"u8.ToArray());
		var reader = new BufferedReader(stream);
		reader.Read(2); // in stream: 345
		reader.PushBack('2'); // in stream: 2345
		reader.PushBack('1'); // in stream: 12345
		reader.PushBack('0'); // in stream: 012345
		Assert.Equal("012345", reader.ReadToEnd());
	}
	[Fact]
	public void BufferedReaderBreaksReadingOnStreamEnd() {
		var stream = new MemoryStream("12345"u8.ToArray());
		var reader = new BufferedReader(stream);
		var read = reader.Read(7);
		Assert.Equal("12345", read);
	}

	[Fact]
	public void EndOfStreamIsFalseIfThereAreCharactersInStack() {
		var reader = new BufferedReader("token");
		Parser.GetNextTokenWithoutMatching(reader);
		Assert.True(reader.EndOfStream);
		reader.PushBack('e');
		reader.PushBack('v');
		reader.PushBack('a');
		Assert.False(reader.EndOfStream);
		Assert.Equal("ave", Parser.GetNextTokenWithoutMatching(reader));
		Assert.True(reader.EndOfStream);
	}

	[Fact]
	public void BufferedReaderCanBeConstructedWithoutParameters() {
		var reader = new BufferedReader();
		Assert.Equal(string.Empty, reader.ReadToEnd());
	}

	[Fact]
	public void VariablesCanBeCopiedFromOtherReader() {
		var a = new BufferedReader("@x=1 @y=2 @c=3");
		var b = new BufferedReader("@a=3 @b=4 @c=5");

		var parser = new Parser(implicitVariableHandling: true);
		parser.ParseStream(a);
		parser.ParseStream(b);
		b.CopyVariables(a);

		Assert.Collection(b.Variables,
			variable => {
				(string key, object value) = variable;
				Assert.Equal("a", key);
				Assert.Equal(3, value);
			},
			variable => {
				(string key, object value) = variable;
				Assert.Equal("b", key);
				Assert.Equal(4, value);
			},
			variable => {
				(string key, object value) = variable;
				Assert.Equal("c", key);
				Assert.Equal(3, value);
			},
			variable => {
				(string key, object value) = variable;
				Assert.Equal("x", key);
				Assert.Equal(1, value);
			},
			variable => {
				(string key, object value) = variable;
				Assert.Equal("y", key);
				Assert.Equal(2, value);
			}
		);
	}

	[Fact]
	public void EvaluateExpressionLogsErrorWhenInterpolatedExpressionCannotBeEvaluated() {
		var output = new StringWriter();
		Console.SetOut(output);

		var reader = new BufferedReader();
		reader.Variables.Add("a", 3);
		const string expressionStr = "@[@a-2]"; // should be @[a-2]
		var value = new BufferedReader().EvaluateExpression("@[@a-2]"); // should be @[a-2]

		Assert.Contains("[WARN] Failed to evaluate expression \"@[@a-2]\"", output.ToString());
		Assert.Equal(expressionStr, value);
	}
	
	[Fact]
	public void VariableIsResolvedWhenItExists() {
		var reader = new BufferedReader();
		reader.Variables.Add("a", 3);
		var value = reader.ResolveVariable("@a");

		Assert.Equal(3, value);
	}
	
	[Fact]
	public void WarningIsLoggedAndNullReturnedWhenVariableCannotBeResolved() {
		var output = new StringWriter();
		Console.SetOut(output);

		var reader = new BufferedReader();
		var value = reader.ResolveVariable("@a");

		Assert.Contains("[WARN] Variable not found: @a", output.ToString());
		Assert.Null(value);
	}

	[Fact]
	public void EndOfStreamIsFalseWhileBufferHoldsCharacters() {
		var reader = new BufferedReader("abc");

		Assert.Equal('a', reader.Peek()); // pulls the characters into the internal buffer
		Assert.False(reader.EndOfStream);
		Assert.Equal("abc", reader.ReadToEnd());
		Assert.True(reader.EndOfStream);
	}

	[Fact]
	public void ReadToEndCombinesBufferedAndPushedBackCharacters() {
		var reader = new BufferedReader("12345");
		Assert.Equal('1', (char)reader.Read());
		reader.PushBack('1');

		Assert.Equal("12345", reader.ReadToEnd());
	}

	[Fact]
	public void PeekAndSkipWorkAcrossBufferBoundary() {
		var reader = new BufferedReader(new string('a', 1500) + "XYZ");

		reader.Skip(1500);

		Assert.Equal('X', reader.Peek());
		Assert.Equal('X', (char)reader.Read());
		Assert.Equal("YZ", reader.ReadToEnd());
	}

	[Fact]
	public void ReadSpansBufferBoundary() {
		const int textLength = 2000;
		var reader = new BufferedReader(new string('x', textLength));
		Assert.Equal('x', (char)reader.Read()); // pulls a chunk into the internal buffer

		var rest = reader.Read(textLength - 1);

		Assert.Equal(new string('x', textLength - 1), rest);
		Assert.True(reader.EndOfStream);
	}

	[Fact]
	public void ReadLineIsRebuiltFromBufferedCharacters() {
		var longLine = new string('a', 1500);
		var reader = new BufferedReader($"{longLine}\nsecond line\r\nthird line\rfourth line");
		Assert.Equal('a', (char)reader.Read()); // pulls a chunk into the internal buffer

		Assert.Equal(longLine[1..], reader.ReadLine());
		Assert.Equal("second line", reader.ReadLine());
		Assert.Equal("third line", reader.ReadLine());
		Assert.Equal("fourth line", reader.ReadLine());
		Assert.Null(reader.ReadLine());
	}

	[Fact]
	public void SkipRestOfLineWorksAcrossBufferBoundary() {
		var longToken = new string('t', 1500);
		var reader = new BufferedReader($"{longToken}# comment long enough to cross the buffer boundary\nnext_token");

		Assert.Equal(longToken, Parser.GetNextLexeme(reader));
		Assert.Equal("next_token", Parser.GetNextLexeme(reader));
	}
}