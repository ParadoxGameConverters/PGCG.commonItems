using commonItems.Collections;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests.Collections;

public sealed class OrderedSetTests {
	[Fact]
	public void InsertionOrderIsPreserved() {
		var set = new OrderedSet<int> { 1, 5, 2, 4, 3 };

		set.Should().Equal(1, 5, 2, 4, 3);
	}

	[Fact]
	public void OrderedSetCanBeCleared() {
		var set = new OrderedSet<int> { 1, 5, 2, 4, 3 };

		set.Clear();
		Assert.Empty(set);
	}

	[Fact]
	public void RemovingAndReaddingElementChangesInsertionOrder() {
		var set = new OrderedSet<int> { 1, 5, 2, 4, 3 };

		set.Remove(5);
		Assert.DoesNotContain(5, set);
		Assert.Collection(set,
			item => Assert.Equal(1, item),
			item => Assert.Equal(2, item),
			item => Assert.Equal(4, item),
			item => Assert.Equal(3, item));

		set.Add(5);
		
		set.Should().Equal(1, 2, 4, 3, 5);
	}

	[Fact]
	public void RemoveReturnsFalseOnElementNotFound() {
		Assert.False(new OrderedSet<int>().Remove(3));
	}

	[Fact]
	public void OrderedSetCanBeConstructedFromIEnumerableWhilePreservingOrder() {
		IEnumerable<int> collection = new List<int>{1, 5, 2, 4, 5, 3, 3};
		var set = new OrderedSet<int>(collection);

		set.Should().Equal(1, 5, 2, 4, 3);
	}

	[Fact]
	public void ExceptWithRemovesAllElementsInSpecifiedCollectionFromCurrentOrderedSet() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var otherCollection = new List<int> {1, 3, 4, 1};
		set.ExceptWith(otherCollection);

		set.Should().Equal(2, 5);
	}

	[Fact]
	public void IntersectWithModifiesSetToOnlyContainElementsPresentInOtherCollection() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var otherCollection = new List<int> {1, 3, 4, 1};
		set.IntersectWith(otherCollection);

		set.Should().Equal(1, 3, 4);
	}

	[Fact]
	public void IsProperSubsetOfReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var superset = new OrderedSet<int> {1, 2, 3, 4, 5, 6};
		
		Assert.True(set.IsProperSubsetOf(superset));
		Assert.False(set.IsProperSubsetOf(set));
	}

	[Fact]
	public void IsProperSupersetOfReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var subset = new OrderedSet<int> {1, 2, 3};
		
		Assert.True(set.IsProperSupersetOf(subset));
		Assert.False(set.IsProperSupersetOf(set));
	}

	[Fact]
	public void IsSubsetOfReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var superset = new OrderedSet<int> {1, 2, 3, 4, 5, 6};
		var otherSet = new HashSet<int> {4, 5, 6};
		
		Assert.True(set.IsSubsetOf(superset));
		Assert.True(set.IsSubsetOf(set));
		Assert.False(set.IsSubsetOf(otherSet));
	}

	[Fact]
	public void IsSupersetOfReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5};
		var subset = new OrderedSet<int> {1, 2, 3};
		var otherSet = new HashSet<int> {4, 5, 6};
		
		Assert.True(set.IsSupersetOf(subset));
		Assert.True(set.IsSupersetOf(set));
		Assert.False(set.IsSupersetOf(otherSet));
	}

	[Fact]
	public void OverlapsReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3};
		var overlappingSet = new OrderedSet<int> {1, 2, 4};
		var nonOverlappingSet = new OrderedSet<int> {4, 5, 6};
		
		Assert.True(set.Overlaps(overlappingSet));
		Assert.False(set.Overlaps(nonOverlappingSet));
	}

	[Fact]
	public void SetEqualsReturnsCorrectValue() {
		var set = new OrderedSet<int> {1, 2, 3};
		var equalArray = new[] {1, 2, 3};
		var unequalList = new List<int> {1, 2, 3, 4};
		
		Assert.True(set.SetEquals(equalArray));
		Assert.False(set.SetEquals(unequalList));
	}

	[Fact]
	public void SymmetricExceptWithModifiesTheCurrentSetSoThatItContainsOnlyElementsThatArePresentEitherInTheCurrentSetOrInTheSpecifiedCollectionButNotBoth() {
		var set = new OrderedSet<int> {1, 2, 3, 4};
		var list = new List<int> {3, 4, 5, 6};
		set.SymmetricExceptWith(list);

		set.Should().Equal(1, 2, 5, 6);
	}

	[Fact]
	public void UnionWithCreatesUnionInCurrentSet() {
		var set = new OrderedSet<int> {1, 2, 3, 4};
		var list = new List<int> {3, 3, 4, 4, 5, 6};
		set.UnionWith(list);

		set.Should().Equal(1, 2, 3, 4, 5, 6);
	}

	[Fact]
	public void RemoveWhereRemovesElementsMatchingPredicate() {
		var set = new OrderedSet<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
		set.RemoveWhere(i=>i%3==1);

		set.Should().Equal(2, 3, 5, 6, 8, 9);
	}
}