using commonItems.Serialization;
using System;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests.Serialization {
	public class PDXSerializerTests {
		private class Title : IPDXSerializable {
			public int id = 20;
			public ulong capital_prov_id = 420;
			public double development = 50.5;
			[NonSerialized] public int priority = 50;
			public string name = "Papal States";
			public List<string> pope_names_list = new() { "Peter", "John", "Hadrian" };
			public Color color1 = new(new[] { 2, 4, 6 });
			public ParadoxBool landless = new(true);
			public bool definite_form = false;

			public Dictionary<string, string> textures = new() {
				{ "diffuse", "gfx/models/diffuse.dds" },
				{ "normal", "gfx/models/normal.dds" }
			};

			public Dictionary<int, string> weights = new() {{10, "roman_gfx"}, {5, "italian_gfx"}};
		}

		[Fact]
		public void PDXSerializableClassIsProperlySerialized() {
			var title = new Title();
			var titleString = PDXSerializer.Serialize(title);

			var expectedString = "{" + Environment.NewLine +
								 "\tid = 20" + Environment.NewLine +
								 "\tcapital_prov_id = 420" + Environment.NewLine +
								 "\tdevelopment = 50.5" + Environment.NewLine +
								 "\tname = \"Papal States\"" + Environment.NewLine +
								 "\tpope_names_list = { \"Peter\", \"John\", \"Hadrian\" }" + Environment.NewLine +
								 "\tcolor1 = { 2 4 6 }" + Environment.NewLine +
								 "\tlandless = yes" + Environment.NewLine +
								 "\tdefinite_form = no" + Environment.NewLine +
								 "\ttextures = {" + Environment.NewLine +
								 "\t\tdiffuse = \"gfx/models/diffuse.dds\"" + Environment.NewLine +
								 "\t\tnormal = \"gfx/models/normal.dds\"" + Environment.NewLine +
								 "\t}" + Environment.NewLine +
								 "\tweights = {" + Environment.NewLine +
								 "\t\t10 = \"roman_gfx\"" + Environment.NewLine +
								 "\t\t5 = \"italian_gfx\"" + Environment.NewLine +
								 "\t}" + Environment.NewLine +
								 "}" + Environment.NewLine;
			Assert.Equal(expectedString, titleString);
		}
	}
}
