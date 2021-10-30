using commonItems.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests.Serialization {
	public class PDXSerializerTests {
		private class Title : IPDXSerializable {
			public int id = 20;
			[NonSerializedAttribute] public int priority = 50;
			public string name = "Papal States";
			public List<string> pope_names_list = new(){"Peter", "John", "Hadrian"};
			public ParadoxBool landless = new(true);
		}

		[Fact]
		public void PDXSerializableClassIsProperlySerialized() {
			var title = new Title();
			var titleString = PDXSerializer.Serialize(title);

			var expectedString = "{" + Environment.NewLine +
								 "\tid = 20" + Environment.NewLine +
								 "\tname = \"Papal States\"" + Environment.NewLine +
								 "\tpope_names_list = { \"Peter\", \"John\", \"Hadrian\" }" + Environment.NewLine +
								 "\tlandless = yes" + Environment.NewLine +
								 "}" + Environment.NewLine;
			Assert.Equal(expectedString, titleString);
		}
	}
}
