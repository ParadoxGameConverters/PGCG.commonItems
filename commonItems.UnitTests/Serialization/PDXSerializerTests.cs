using commonItems.Serialization;
using System;
using System.Collections.Generic;
using Xunit;
// ReSharper disable InconsistentNaming

namespace commonItems.UnitTests.Serialization {
	public class PDXSerializerTests {
		private class RulerInfo : IPDXSerializable {
			public string nickname;
		}

		private class Title : IPDXSerializable {
			// public properties
			public int id { get; set; } = 20;
			public ulong capital_prov_id { get; set; } = 420;
			public double development { get; set; } = 50.5;
			[commonItems.Serialization.NonSerialized] public int priority { get; set; } = 50;
			public string name { get; set; } = "Papal States";
			public List<string> pope_names_list { get; set; } = new() { "Peter", "John", "Hadrian" };
			public Color color1 { get; set; } = new(new[] { 2, 4, 6 });
			public bool definite_form { get; private set; }
			// public fields
			public ParadoxBool landless = new(true);
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
			public RulerInfo ruler_info = new() { nickname = "the_great" };
		}

		[Fact]
		public void PDXSerializableClassIsProperlySerialized() {
			var title = new Title();
			var titleString = PDXSerializer.Serialize(title, string.Empty);

			var expectedString =
				"{" + Environment.NewLine +
				"\tid = 20" + Environment.NewLine +
				"\tcapital_prov_id = 420" + Environment.NewLine +
				"\tdevelopment = 50.5" + Environment.NewLine +
				"\tname = \"Papal States\"" + Environment.NewLine +
				"\tpope_names_list = { \"Peter\" \"John\" \"Hadrian\" }" + Environment.NewLine +
				"\tcolor1 = { 2 4 6 }" + Environment.NewLine +
				"\tdefinite_form = no" + Environment.NewLine +
				"\tlandless = yes" + Environment.NewLine +
				"\tcreation_date = 600.4.5" + Environment.NewLine +
				"\ttextures = {" + Environment.NewLine +
				"\t\tdiffuse = \"gfx/models/diffuse.dds\"" + Environment.NewLine +
				"\t\tnormal = \"gfx/models/normal.dds\"" + Environment.NewLine +
				"\t}" + Environment.NewLine +
				"\tweights = {" + Environment.NewLine +
				"\t\t10 = \"roman_gfx\"" + Environment.NewLine +
				"\t\t5 = \"italian_gfx\"" + Environment.NewLine +
				"\t}" + Environment.NewLine +
				"\tgreetings = { \"hi\" \"salutations\" \"greetings\" }" + Environment.NewLine +
				"\truler_info = {" + Environment.NewLine +
				"\t\tnickname = \"the_great\"" + Environment.NewLine +
				"\t}" + Environment.NewLine +
				"}";
			Assert.Equal(expectedString, titleString);
		}

		[Fact]
		public void NullMembersAreNotSerialized() {
			var c1 = new RulerInfo { nickname = "the_great" };
			var c1Str = PDXSerializer.Serialize(c1, string.Empty);
			Assert.Contains("nickname = \"the_great\"", c1Str);
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
				"\tname = \"Property\"" + Environment.NewLine +
				"\tculture = \"roman\"" + Environment.NewLine +
				"}";
			Assert.Equal(expectedStr, str);
		}
	}
}
