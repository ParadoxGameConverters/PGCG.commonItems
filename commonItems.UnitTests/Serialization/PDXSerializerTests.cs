using commonItems.Collections;
using commonItems.Colors;
using commonItems.Serialization;
using commonItems.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
// ReSharper disable InconsistentNaming

namespace commonItems.UnitTests.Serialization;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed partial class PDXSerializerTests {
	[SerializationByProperties]
	private sealed partial class TestRulerInfo : IPDXSerializable {
		public string? nickname { get; init; }
		[SerializedName("health")] public double? Health { get; init; }
	}

	[SerializationByProperties]
	private sealed partial class TestTitle : IPDXSerializable {
		public int id { get; set; } = 20;
		public ulong capital_prov_id { get; set; } = 420;
		public double development { get; set; } = 50.5;
		[commonItems.Serialization.NonSerialized] public int priority { get; set; } = 50;
		public string name { get; set; } = "\"Papal States\"";
		public List<string> pope_names_list { get; set; } = ["Peter", "John", "Hadrian"];
		public List<short> empty_list { get; } = [];
		public Color color1 { get; set; } = new(2, 4, 6);
		public bool definite_form { get; }
		public bool landless => true;
		public Date creation_date { get; } = new(600, 4, 5);
		public Dictionary<string, string> textures { get; } = new() {
			{ "diffuse", "\"gfx/models/diffuse.dds\"" },
			{ "normal", "\"gfx/models/normal.dds\"" }
		};
		public Dictionary<int, string> weights { get; } = new() {
			{ 10, "roman_gfx" },
			{ 5, "italian_gfx" }
		};
		public HashSet<string> greetings { get; } = ["\"hi\"", "\"salutations\"", "\"greetings\""];
		[SerializeOnlyValue] public KeyValuePair<string, string> kvPair { get; } = new("key", "value");
		public TestRulerInfo ruler_info { get; } = new() { nickname = "the_great" };
		public StringOfItem ai_priority { get; } = new(new BufferedReader("= { add = 70 }"));
	}

	[SerializationByProperties]
	private sealed partial class TestClass : IPDXSerializable {
		[SerializedName("number1")] public double Number1 { get; init; }
		[SerializedName("number2")] public double Number2 { get; init; }
		[SerializedName("number3")] public double Number3 { get; init; }
	}

	[SerializationByProperties]
	private sealed partial class PascalCaseClass : IPDXSerializable {
		[SerializedName("name")] public string Name { get; } = "Property";
		[SerializedName("culture")] public string Culture { get; } = "roman";
	}

	[SerializationByProperties]
	private sealed partial class TestHistory : IPDXSerializable {
		[SerializeOnlyValue]
		public Dictionary<string, object> HistoryFields { get; } = new() {
			{ "culture", "roman" },
			{ "development", 3.14 },
			{ "buildings", new List<string> { "baths", "aqueduct" } }
		};
	}

	private sealed class TestTitleCollection : IdObjectCollection<string, TestCK3Title>;

	[SerializationByProperties]
	private sealed partial class TestCK3Title(string id, Color color) : IPDXSerializable, IIdentifiable<string> {
		[commonItems.Serialization.NonSerialized] public string Id { get; } = id;
		public Color? color { get; } = color;
		[SerializeOnlyValue] public TestTitleCollection DeJureVassals { get; } = [];
	}

	[Fact]
	public void PDXSerializableClassIsProperlySerialized() {
		var title = new TestTitle();
		var titleString = PDXSerializer.Serialize(title, string.Empty);

		var expectedString =
			"{" + Environment.NewLine +
			"\tid = 20" + Environment.NewLine +
			"\tcapital_prov_id = 420" + Environment.NewLine +
			"\tdevelopment = 50.5" + Environment.NewLine +
			"\tname = \"Papal States\"" + Environment.NewLine +
			"\tpope_names_list = { Peter John Hadrian }" + Environment.NewLine +
			"\tempty_list = { }" + Environment.NewLine +
			"\tcolor1 = { 2 4 6 }" + Environment.NewLine +
			"\tdefinite_form = no" + Environment.NewLine +
			"\tlandless = yes" + Environment.NewLine +
			"\tcreation_date = 600.4.5" + Environment.NewLine +
			"\ttextures = {" + Environment.NewLine +
			"\t\tdiffuse = \"gfx/models/diffuse.dds\"" + Environment.NewLine +
			"\t\tnormal = \"gfx/models/normal.dds\"" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tweights = {" + Environment.NewLine +
			"\t\t10 = roman_gfx" + Environment.NewLine +
			"\t\t5 = italian_gfx" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tgreetings = { \"hi\" \"salutations\" \"greetings\" }" + Environment.NewLine +
			"\tkey = value" + Environment.NewLine +
			"\truler_info = {" + Environment.NewLine +
			"\t\tnickname = the_great" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tai_priority = { add = 70 }" + Environment.NewLine +
			"}";
		Assert.Equal(expectedString, titleString);
	}

	[Fact]
	public void NumbersAreSerializedWithDigitLimit() {
		var testObj = new TestClass {Number1 = 60.800000000000004, Number2 = 60.123456789, Number3=60};
		var expectedString =
			"{" + Environment.NewLine +
			"\tnumber1 = 60.8" + Environment.NewLine +
			"\tnumber2 = 60.12346" + Environment.NewLine +
			"\tnumber3 = 60" + Environment.NewLine +
			"}";
		Assert.Equal(expectedString, PDXSerializer.Serialize(testObj));
	}

	[Fact]
	public void NullPropertiesAreNotSerialized() {
		// string? property (reference type)
		var i1 = new TestRulerInfo { nickname = "the_great" };
		Assert.Contains("nickname = the_great", PDXSerializer.Serialize(i1, string.Empty));
		var i2 = new TestRulerInfo { nickname = null };
		Assert.DoesNotContain("nickname", PDXSerializer.Serialize(i2, string.Empty));

		// double? property (value type)
		var i3 = new TestRulerInfo { Health = 100 };
		Assert.Contains("health = 100", PDXSerializer.Serialize(i3, string.Empty));
		var i4 = new TestRulerInfo { Health = null };
		Assert.DoesNotContain("health", PDXSerializer.Serialize(i4, string.Empty));
	}

	[Fact]
	public void PropertiesCanHaveCustomSerializedNames() {
		var pascal = new PascalCaseClass();
		var str = PDXSerializer.Serialize(pascal);
		var expectedStr = "{" + Environment.NewLine +
		                  "\tname = Property" + Environment.NewLine +
		                  "\tculture = roman" + Environment.NewLine +
		                  "}";
		Assert.Equal(expectedStr, str);
	}

	[Fact]
	public void MembersCanBeSerializedWithoutNames() {
		var history = new TestHistory();
		var expectedStr =
			"culture = roman" + Environment.NewLine +
			"development = 3.14" + Environment.NewLine +
			"buildings = { baths aqueduct }" + Environment.NewLine;
		Assert.Equal(expectedStr, PDXSerializer.Serialize(history, indent: string.Empty, withBraces: false));
	}

	[Fact]
	public void EnumerableCanBeSerializedWithoutBraces() {
		var list = new List<string> { "veni", "vidi", "vici" };
		var expectedStr =
			"\tveni" + Environment.NewLine +
			"\tvidi" + Environment.NewLine +
			"\tvici";
		Assert.Equal(expectedStr, PDXSerializer.Serialize(list, "\t", withBraces: false));
	}

	[Fact]
	public void RecursiveClassesAreSerializedWithCorrectIndentation() {
		var empire = new TestCK3Title("e_empire", new(1, 1, 1));
		var kingdom1 = new TestCK3Title("k_kingdom1", new(2, 2, 2));
		empire.DeJureVassals.Add(kingdom1);
		var duchy1 = new TestCK3Title("d_duchy1", new(3, 3, 3));
		kingdom1.DeJureVassals.Add(duchy1);
		var kingdom2 = new TestCK3Title("k_kingdom2", new(4, 4, 4));
		empire.DeJureVassals.Add(kingdom2);
		TestTitleCollection topLevelTitles = [empire];

		var expectedStr =
			"e_empire = {" + Environment.NewLine +
			"\tcolor = { 1 1 1 }" + Environment.NewLine +
			"\tk_kingdom1 = {" + Environment.NewLine +
			"\t\tcolor = { 2 2 2 }" + Environment.NewLine +
			"\t\td_duchy1 = {" + Environment.NewLine +
			"\t\t\tcolor = { 3 3 3 }" + Environment.NewLine +
			"\t\t}" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tk_kingdom2 = {" + Environment.NewLine +
			"\t\tcolor = { 4 4 4 }" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"}";
		Assert.Equal(
			expectedStr,
			PDXSerializer.Serialize(topLevelTitles, indent: string.Empty, withBraces: false)
		);
	}

	[Fact]
	public void QuotedStringsAreNotSerializedWithAdditionalQuotes() {
		const string unquotedString = "unquoted";
		const string quotedString = "\"quoted\"";

		Assert.Equal("unquoted", PDXSerializer.Serialize(unquotedString));
		Assert.Equal("\"quoted\"", PDXSerializer.Serialize(quotedString));
	}
}