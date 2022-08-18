using commonItems.Collections;
using commonItems.Serialization;
using System;
using System.Collections.Generic;
using Xunit;
// ReSharper disable InconsistentNaming

namespace commonItems.UnitTests.Serialization; 

public class PDXSerializerTests {
	private class RulerInfo : IPDXSerializable {
		public string? nickname;
	}

	private class Title : IPDXSerializable {
		// public properties
		public int id { get; set; } = 20;
		public ulong capital_prov_id { get; set; } = 420;
		public double development { get; set; } = 50.5;
		[commonItems.Serialization.NonSerialized] public int priority { get; set; } = 50;
		public string name { get; set; } = "Papal States";
		public List<string> pope_names_list { get; set; } = new() { "Peter", "John", "Hadrian" };
		public List<short> empty_list { get; set; } = new();
		public Color color1 { get; set; } = new(2, 4, 6);
		public bool definite_form { get; private set; }

		// public fields
		public bool landless = true;
		public Date creation_date = new(600, 4, 5);
		public Dictionary<string, string> textures = new() {
			{ "diffuse", "gfx/models/diffuse.dds" },
			{ "normal", "gfx/models/normal.dds" }
		};
		public Dictionary<int, string> weights = new() {
			{ 10, "roman_gfx" },
			{ 5, "italian_gfx" }
		};
		public HashSet<string> greetings = new() { "hi", "salutations", "greetings" };
		[SerializeOnlyValue] public KeyValuePair<string, string> kvPair = new("key", "value");
		public RulerInfo ruler_info = new() { nickname = "the_great" };
		public StringOfItem ai_priority = new(new BufferedReader("= { add = 70 }"));
	}

	[Fact]
	public void PDXSerializableClassIsProperlySerialized() {
		var title = new Title();
		var titleString = PDXSerializer.Serialize(title, string.Empty);

		var expectedString =
			"{" + Environment.NewLine +
			"\tid=20" + Environment.NewLine +
			"\tcapital_prov_id=420" + Environment.NewLine +
			"\tdevelopment=50.5" + Environment.NewLine +
			"\tname=\"Papal States\"" + Environment.NewLine +
			"\tpope_names_list={ \"Peter\" \"John\" \"Hadrian\" }" + Environment.NewLine +
			"\tempty_list={ }" + Environment.NewLine +
			"\tcolor1={ 2 4 6 }" + Environment.NewLine +
			"\tdefinite_form=no" + Environment.NewLine +
			"\tlandless=yes" + Environment.NewLine +
			"\tcreation_date=600.4.5" + Environment.NewLine +
			"\ttextures={" + Environment.NewLine +
			"\t\tdiffuse=\"gfx/models/diffuse.dds\"" + Environment.NewLine +
			"\t\tnormal=\"gfx/models/normal.dds\"" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tweights={" + Environment.NewLine +
			"\t\t10=\"roman_gfx\"" + Environment.NewLine +
			"\t\t5=\"italian_gfx\"" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tgreetings={ \"hi\" \"salutations\" \"greetings\" }" + Environment.NewLine +
			"\tkey=\"value\"" + Environment.NewLine +
			"\truler_info={" + Environment.NewLine +
			"\t\tnickname=\"the_great\"" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tai_priority={ add = 70 }" + Environment.NewLine +
			"}";
		Assert.Equal(expectedString, titleString);
	}

	private class TestClass : IPDXSerializable {
		[SerializedName("number1")] public double Number1 { get; set; }
		[SerializedName("number2")] public double Number2 { get; set; }
		[SerializedName("number3")] public double Number3 { get; set; }
	}

	[Fact]
	public void NumbersAreSerializedWithDigitLimit() {
		var testObj = new TestClass {Number1 = 60.800000000000004, Number2 = 60.123456789, Number3=60};
		var expectedString =
			"{" + Environment.NewLine +
			"\tnumber1=60.8" + Environment.NewLine +
			"\tnumber2=60.123457" + Environment.NewLine +
			"\tnumber3=60" + Environment.NewLine +
			"}";
		Assert.Equal(expectedString, PDXSerializer.Serialize(testObj));
	}

	[Fact]
	public void NullMembersAreNotSerialized() {
		var c1 = new RulerInfo { nickname = "the_great" };
		var c1Str = PDXSerializer.Serialize(c1, string.Empty);
		Assert.Contains("nickname=\"the_great\"", c1Str);
		var c2 = new RulerInfo { nickname = null };
		var c2Str = PDXSerializer.Serialize(c2, string.Empty);
		Assert.DoesNotContain("nickname", c2Str);
	}

	private class PascalClass : IPDXSerializable {
		[SerializedName("name")] public string Name { get; } = "Property";
		[SerializedName("culture")] public string Culture = "roman";
	}

	[Fact]
	public void MembersCanHaveCustomSerializedNames() {
		var pascal = new PascalClass();
		var str = PDXSerializer.Serialize(pascal);
		var expectedStr = "{" + Environment.NewLine +
		                  "\tname=\"Property\"" + Environment.NewLine +
		                  "\tculture=\"roman\"" + Environment.NewLine +
		                  "}";
		Assert.Equal(expectedStr, str);
	}

	private class History : IPDXSerializable {
		[SerializeOnlyValue]
		public Dictionary<string, object> HistoryFields { get; } = new() {
			{ "culture", "roman" },
			{ "development", 3.14 },
			{ "buildings", new List<string> { "baths", "aqueduct" } }
		};
	}

	[Fact]
	public void MembersCanBeSerializedWithoutNames() {
		var history = new History();
		var expectedStr =
			"culture=\"roman\"" + Environment.NewLine +
			"development=3.14" + Environment.NewLine +
			"buildings={ \"baths\" \"aqueduct\" }" + Environment.NewLine;
		Assert.Equal(expectedStr, PDXSerializer.Serialize(history, indent: string.Empty, withBraces: false));
	}

	[Fact]
	public void EnumerableCanBeSerializedWithoutBraces() {
		var list = new List<string> { "veni", "vidi", "vici" };
		var expectedStr =
			"\"veni\"" + Environment.NewLine +
			"\"vidi\"" + Environment.NewLine +
			"\"vici\"";
		Assert.Equal(expectedStr, PDXSerializer.Serialize(list, string.Empty, withBraces: false));
	}

	private class TitleCollection : IdObjectCollection<string, CK3Title> { }
	private class CK3Title : IPDXSerializable, IIdentifiable<string> {
		[commonItems.Serialization.NonSerialized] public string Id { get; }
		public Color? color { get; set; }
		[SerializeOnlyValue] public TitleCollection DeJureVassals = new();

		public CK3Title(string id, Color color) {
			Id = id;
			this.color = color;
		}
	}
	[Fact]
	public void RecursiveClassesAreSerializedWithCorrectIndentation() {
		var empire = new CK3Title("e_empire", new(1, 1, 1));
		var kingdom1 = new CK3Title("k_kingdom1", new(2, 2, 2));
		empire.DeJureVassals.Add(kingdom1);
		var duchy1 = new CK3Title("d_duchy1", new(3, 3, 3));
		kingdom1.DeJureVassals.Add(duchy1);
		var kingdom2 = new CK3Title("k_kingdom2", new(4, 4, 4));
		empire.DeJureVassals.Add(kingdom2);
		TitleCollection topLevelTitles = new() { empire };

		var expectedStr =
			"e_empire={" + Environment.NewLine +
			"\tcolor={ 1 1 1 }" + Environment.NewLine +
			"\tk_kingdom1={" + Environment.NewLine +
			"\t\tcolor={ 2 2 2 }" + Environment.NewLine +
			"\t\td_duchy1={" + Environment.NewLine +
			"\t\t\tcolor={ 3 3 3 }" + Environment.NewLine +
			"\t\t}" + Environment.NewLine +
			"\t}" + Environment.NewLine +
			"\tk_kingdom2={" + Environment.NewLine +
			"\t\tcolor={ 4 4 4 }" + Environment.NewLine +
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
		
		Assert.Equal("\"unquoted\"", PDXSerializer.Serialize(unquotedString));
		Assert.Equal("\"quoted\"", PDXSerializer.Serialize(quotedString));
	}
}