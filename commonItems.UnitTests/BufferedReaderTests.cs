﻿using System.IO;
using System.Text;
using Xunit;

namespace commonItems.UnitTests {
	[Collection("Sequential")]
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class BufferedReaderTests {
		[Fact]
		public void BufferedReaderReadsCorrectly() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
			var reader = new BufferedReader(stream);
			var read = reader.Read(2);
			Assert.Equal("12", read);
			Assert.Equal("345", reader.ReadToEnd());
		}
		[Fact]
		public void BufferedReaderAllowsPushBackOfOneChar() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
			var reader = new BufferedReader(stream);
			reader.Read(); // in stream: 2345
			reader.Read(); // in stream: 345
			reader.PushBack('2'); // in stream: 2345
			Assert.Equal("2345", reader.ReadToEnd());
		}
		[Fact]
		public void BufferedReaderAllowsPushBackOfMultipleChars() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
			var reader = new BufferedReader(stream);
			reader.Read(2); // in stream: 345
			reader.PushBack('2'); // in stream: 2345
			reader.PushBack('1'); // in stream: 12345
			reader.PushBack('0'); // in stream: 012345
			Assert.Equal("012345", reader.ReadToEnd());
		}
		[Fact]
		public void BufferedReaderBreaksReadingOnStreamEnd() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
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

			var parser = new Parser();
			parser.ParseStream(a);
			parser.ParseStream(b);
			b.CopyVariables(a);

			Assert.Collection(b.Variables,
				variable => {
					(string? key, object? value) = variable;
					Assert.Equal("a", key);
					Assert.Equal(3, value);
				},
				variable => {
					(string? key, object? value) = variable;
					Assert.Equal("b", key);
					Assert.Equal(4, value);
				},
				variable => {
					(string? key, object? value) = variable;
					Assert.Equal("c", key);
					Assert.Equal(3, value);
				},
				variable => {
					(string? key, object? value) = variable;
					Assert.Equal("x", key);
					Assert.Equal(1, value);
				},
				variable => {
					(string? key, object? value) = variable;
					Assert.Equal("y", key);
					Assert.Equal(2, value);
				}
			);
		}
	}
}
