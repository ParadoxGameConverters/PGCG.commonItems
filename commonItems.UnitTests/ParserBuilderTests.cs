using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class ParserBuilderTests {
	private sealed class CollectingAppender : AppenderSkeleton {
		public List<string> Messages { get; } = [];

		protected override void Append(LoggingEvent loggingEvent) {
			lock (Messages) {
				Messages.Add(loggingEvent.RenderedMessage ?? string.Empty);
			}
		}
	}

	[Fact]
	public void ParserBuilderBuildsParserWithKeywordRegistration() {
		string? value = null;
		var parser = new ParserBuilder()
			.WithKeyword("key", reader => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader("key = value"));

		Assert.Equal("value", value);
	}

	[Fact]
	public void ParserBuilderBuildsParserWithTwoArgumentKeywordRegistration() {
		string? matchedKeyword = null;
		string? value = null;
		var parser = new ParserBuilder()
			.WithKeyword("key", (reader, keyword) => {
				matchedKeyword = keyword;
				value = reader.GetString();
			})
			.Build();

		parser.ParseStream(new BufferedReader("key = value"));

		Assert.Equal("key", matchedKeyword);
		Assert.Equal("value", value);
	}

	[Theory]
	[InlineData("key ?= value")]
	[InlineData("key?= value")]
	public void ParserBuilderBuildsParserWithRegexRegistration(string input) {
		string? value = null;
		var parser = new ParserBuilder()
			.WithRegex("[key]+", (reader, _) => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(input));

		Assert.Equal("value", value);
	}

	[Theory]
	[InlineData("key ?= value")]
	[InlineData("key?= value")]
	public void ParserBuilderBuildsParserWithStringRegexSimpleRegistration(string input) {
		string? value = null;
		var parser = new ParserBuilder()
			.WithRegex("[key]+", reader => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(input));

		Assert.Equal("value", value);
	}

	[Theory]
	[InlineData("key ?= value")]
	[InlineData("key?= value")]
	public void ParserBuilderBuildsParserWithRegexObjectRegistration(string input) {
		string? matchedKeyword = null;
		string? value = null;
		var parser = new ParserBuilder()
			.WithRegex(new Regex("[key]+"), (reader, keyword) => {
				matchedKeyword = keyword;
				value = reader.GetString();
			})
			.Build();

		parser.ParseStream(new BufferedReader(input));

		Assert.Equal("key", matchedKeyword);
		Assert.Equal("value", value);
	}

	[Theory]
	[InlineData("key ?= value")]
	[InlineData("key?= value")]
	public void ParserBuilderBuildsParserWithRegexObjectSimpleRegistration(string input) {
		string? value = null;
		var parser = new ParserBuilder()
			.WithRegex(new Regex("[key]+"), reader => value = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(input));

		Assert.Equal("value", value);
	}

	[Fact]
	public void ParserBuilderCanIgnoreUnregisteredItems() {
		string? value = null;
		var parser = new ParserBuilder()
			.WithKeyword("key", reader => value = reader.GetString())
			.IgnoreUnregisteredItems()
			.Build();

		parser.ParseStream(new BufferedReader("key = value ignored = { nested = value }"));

		Assert.Equal("value", value);
	}

	[Fact]
	public void ParserBuilderCanIgnoreAndStoreUnregisteredItems() {
		var ignoredTokens = new SortedSet<string>();
		var parser = new ParserBuilder()
			.IgnoreAndStoreUnregisteredItems(ignoredTokens)
			.Build();

		parser.ParseStream(new BufferedReader("first = value second = { nested = value }"));

		Assert.Equal(["first", "second"], ignoredTokens);
	}

	[Fact]
	public void ParserBuilderCanIgnoreAndLogUnregisteredItems() {
		var appender = new CollectingAppender {
			Threshold = Level.All,
		};
		var hierarchy = (Hierarchy)LogManager.GetRepository();
		var logger = (global::log4net.Repository.Hierarchy.Logger)hierarchy.GetLogger("mainLogger");
		var previousLevel = logger.Level;
		var previousAdditivity = logger.Additivity;
		logger.Level = Level.All;
		logger.Additivity = false;
		logger.AddAppender(appender);
		appender.ActivateOptions();

		try {
			string? value = null;
			var parser = new ParserBuilder()
				.WithKeyword("key", reader => value = reader.GetString())
				.IgnoreAndLogUnregisteredItems()
				.Build();

			parser.ParseStream(new BufferedReader("key = value ignored = yes"));

			Assert.Equal("value", value);
			string[] snapshot;
			lock (appender.Messages) {
				snapshot = appender.Messages.ToArray();
			}
			Assert.Contains("Ignoring keyword: ignored", snapshot);
		} finally {
			try {
				logger.RemoveAppender(appender);
			} catch (ArgumentException) {
				// log4net can throw if the appender was not attached to the logger.
			}
			logger.Level = previousLevel;
			logger.Additivity = previousAdditivity;
			appender.Close();
		}
	}

	[Fact]
	public void ParserBuilderSupportsImplicitVariableHandling() {
		string? name = null;
		var parser = new ParserBuilder(implicitVariableHandling: true)
			.WithKeyword("name", reader => name = reader.GetString())
			.Build();

		parser.ParseStream(new BufferedReader(
			"@best_country_on_earth_name = \"Roman Empire\"\n" +
			"name = @best_country_on_earth_name"
		));

		Assert.Equal("Roman Empire", name);
	}
}