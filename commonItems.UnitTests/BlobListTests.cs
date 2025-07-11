using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests; 

public sealed class BlobListTests {
	[Fact]
	public void BlobListDefaultsToEmpty() {
		var reader = new BufferedReader(string.Empty);
		var theBlobs = new BlobList(reader);

		Assert.Empty(theBlobs.Blobs);
	}

	[Fact]
	public void BlobListAddsBlobs() {
		var reader = new BufferedReader("= { {foo} {bar} {baz} }");
		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> { "foo", "bar", "baz" };
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}

	[Fact]
	public void BlobListAddsBlobsOnExistsEquals() {
		var reader = new BufferedReader("?= { {foo} {bar} {baz} }");
		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> { "foo", "bar", "baz" };
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}

	[Fact]
	public void BlobListAddsComplicatedBlobs() {
		var reader = new BufferedReader("= { {foo=bar bar=baz} {bar=baz baz=foo} {baz=foo foo=bar} }");
		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> { "foo=bar bar=baz", "bar=baz baz=foo", "baz=foo foo=bar" };
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}

	[Fact]
	public void BlobListPreservesEverythingWithinBlobs() {
		var reader = new BufferedReader(
			"= { {foo\t=\nbar\n \n{bar\t=\tbaz\n\n}} {BROKEN\t\t\tbar\n=\nbaz\n \t\tbaz\t=\nfoo\t} {\t\nbaz\n\t=\t\n\tfoo\n " +
			"{} \n\tfoo\t=\tbar\t} }"
		);

		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> {
			"foo\t=\nbar\n \n{bar\t=\tbaz\n\n}",
			"BROKEN\t\t\tbar\n=\nbaz\n \t\tbaz\t=\nfoo\t",
			"\t\nbaz\n\t=\t\n\tfoo\n {} \n\tfoo\t=\tbar\t"
		};
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}

	[Fact]
	public void BlobListIgnoresEverythingOutsideBlobs() {
		var reader = new BufferedReader(
			"= {\n\n\t\t{foo}\nkey=value\n\t {bar}\t\nsome=value\t\n{baz}\t\n  randomLooseText   }"
		);

		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> { "foo", "bar", "baz" };
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}

	[Fact]
	public void BlobListIsEmptyOnTrivialWrongUsage() {
		var reader = new BufferedReader("= value\n");
		var theBlobs = new BlobList(reader);

		Assert.Empty(theBlobs.Blobs);
	}

	[Fact]
	public void BlobListIsEmptyOnSimpleWrongUsage() {
		var reader = new BufferedReader("= { key=value\n key2=value2 }");
		var theBlobs = new BlobList(reader);

		Assert.Empty(theBlobs.Blobs);
	}

	[Fact]
	public void BlobListIsNotAtFaultYouAreOnComplexWrongUsage() {
		var reader = new BufferedReader("= { key=value\n key2={ key3 = value2 }}");
		var theBlobs = new BlobList(reader);

		var expectedBlobs = new List<string> { " key3 = value2 " };
		Assert.Equal(expectedBlobs, theBlobs.Blobs);
	}
}