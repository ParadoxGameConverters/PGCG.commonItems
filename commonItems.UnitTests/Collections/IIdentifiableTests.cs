using commonItems.Collections;
using System;
using Xunit;

namespace commonItems.UnitTests.Collections;

public sealed class IIdentifiableTests {
	private sealed class TestIdentifiable(int id) : IIdentifiable<int> {
		public int Id { get; } = id;
	}

	[Fact]
	public void GetIdStringReturnsIdAsString() {
		IIdentifiable identifiable = new TestIdentifiable(42);

		Assert.Equal("42", identifiable.GetIdString());
	}

	[Fact]
	public void CompareToReturnsZeroForSameReference() {
		var identifiable = new TestIdentifiable(7);
		IComparable<IIdentifiable<int>> comparable = identifiable;

		Assert.Equal(0, comparable.CompareTo(identifiable));
	}

	[Fact]
	public void CompareToReturnsOneForNull() {
		var identifiable = new TestIdentifiable(7);
		IComparable<IIdentifiable<int>> comparable = identifiable;

		Assert.Equal(1, comparable.CompareTo(null));
	}

	[Fact]
	public void CompareToComparesById() {
		var lower = new TestIdentifiable(2);
		var higher = new TestIdentifiable(5);
		IComparable<IIdentifiable<int>> comparable = lower;

		Assert.True(comparable.CompareTo(higher) < 0);
	}
}