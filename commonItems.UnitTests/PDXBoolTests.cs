using System;
using Xunit;

namespace commonItems.UnitTests {
	public class PDXBoolTests {
		[Fact]
		public void ParadoxBoolDefaultToTrue() {
			Assert.True(new PDXBool());
		}
		[Fact]
		public void ParadoxBoolCanBeConstructedFromBool() {
			Assert.True(new PDXBool(true));
			Assert.False(new PDXBool(false));
		}
		[Fact]
		public void ParadoxBoolCanBeConstructedFromString() {
			Assert.True(new PDXBool("yes"));
			Assert.False(new PDXBool("no"));
		}
		[Fact]
		public void ExceptionIsThrownOnWrongValueString() {
			const string valStr = "perhaps";
			Assert.Throws<FormatException>(() => new PDXBool(valStr));
		}
		[Fact]
		public void ParadoxBoolCanBeGottenFromBufferedReader() {
			var reader1 = new BufferedReader("= yes");
			var reader2 = new BufferedReader("= no");
			Assert.True(reader1.GetPDXBool());
			Assert.False(reader2.GetPDXBool());
		}
		[Fact]
		public void CorrectTextRepresentationIsReturned() {
			var bool1 = new PDXBool("yes");
			var bool2 = new PDXBool("no");
			Assert.Equal("yes", bool1.YesOrNo);
			Assert.Equal("no", bool2.YesOrNo);
		}
		[Fact]
		public void ValueCanBeChanged() {
			var pdxBool = new PDXBool("no");
			Assert.False(pdxBool);
			pdxBool.Value = true;
			Assert.True(pdxBool);
		}
	}
}
