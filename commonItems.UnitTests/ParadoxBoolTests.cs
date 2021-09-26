using System;
using Xunit;

namespace commonItems.UnitTests {
	public class ParadoxBoolTests {
		[Fact]
		public void ParadoxBoolDefaultToTrue() {
			Assert.True(new ParadoxBool());
		}
		[Fact]
		public void ParadoxBoolCanBeConstructedFromBool() {
			Assert.True(new ParadoxBool(true));
			Assert.False(new ParadoxBool(false));
		}
		[Fact]
		public void ParadoxBoolCanBeConstructedFromString() {
			Assert.True(new ParadoxBool("yes"));
			Assert.False(new ParadoxBool("no"));
		}
		[Fact]
		public void ExceptionIsThrownOnWrongValueString() {
			const string valStr = "perhaps";
			Assert.Throws<FormatException>(() => new ParadoxBool(valStr));
		}
		[Fact]
		public void ParadoxBoolCanBeConstructedFromBufferedReader() {
			var reader1 = new BufferedReader("= yes");
			var reader2 = new BufferedReader("= no");
			Assert.True(new ParadoxBool(reader1));
			Assert.False(new ParadoxBool(reader2));
		}
		[Fact]
		public void CorrectTextRepresentationIsReturned() {
			var bool1 = new ParadoxBool("yes");
			var bool2 = new ParadoxBool("no");
			Assert.Equal("yes", bool1.YesOrNo);
			Assert.Equal("no", bool2.YesOrNo);
		}
		[Fact]
		public void ValueCanBeChanged() {
			var pdxBool = new ParadoxBool("no");
			Assert.False(pdxBool);
			pdxBool.Value = true;
			Assert.True(pdxBool);
		}
	}
}
