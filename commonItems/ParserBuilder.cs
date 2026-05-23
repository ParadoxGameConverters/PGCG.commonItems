using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace commonItems;

public sealed class ParserBuilder(bool implicitVariableHandling = false) {
	private readonly List<Action<Parser>> buildSteps = [];
	private readonly bool implicitVariableHandling = implicitVariableHandling;

	public ParserBuilder WithKeyword(string keyword, Del del) {
		buildSteps.Add(parser => parser.RegisterKeyword(keyword, del));
		return this;
	}

	public ParserBuilder WithKeyword(string keyword, SimpleDel del) {
		buildSteps.Add(parser => parser.RegisterKeyword(keyword, del));
		return this;
	}

	public ParserBuilder WithRegex(string regex, Del del) {
		buildSteps.Add(parser => parser.RegisterRegex(regex, del));
		return this;
	}

	public ParserBuilder WithRegex(string regex, SimpleDel del) {
		buildSteps.Add(parser => parser.RegisterRegex(regex, del));
		return this;
	}

	public ParserBuilder WithRegex(Regex regex, Del del) {
		buildSteps.Add(parser => parser.RegisterRegex(regex, del));
		return this;
	}

	public ParserBuilder WithRegex(Regex regex, SimpleDel del) {
		buildSteps.Add(parser => parser.RegisterRegex(regex, del));
		return this;
	}

	public ParserBuilder IgnoreUnregisteredItems() {
		buildSteps.Add(static parser => parser.IgnoreUnregisteredItems());
		return this;
	}

	public ParserBuilder IgnoreAndLogUnregisteredItems() {
		buildSteps.Add(static parser => parser.IgnoreAndLogUnregisteredItems());
		return this;
	}

	public ParserBuilder IgnoreAndStoreUnregisteredItems(ISet<string> ignoredTokenSet) {
		buildSteps.Add(parser => parser.IgnoreAndStoreUnregisteredItems(ignoredTokenSet));
		return this;
	}

	public Parser Build() {
		var parser = new Parser(implicitVariableHandling);
		foreach (var buildStep in buildSteps) {
			buildStep(parser);
		}

		return parser;
	}
}