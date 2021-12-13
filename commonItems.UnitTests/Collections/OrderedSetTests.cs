using commonItems.Collections;
using Xunit;

namespace commonItems.UnitTests.Collections {
	public class OrderedSetTests {
		[Fact]
		public void InsertionOrderIsPreserved() {
			var set = new OrderedSet<int> { 1, 5, 2, 4, 3 };

			Assert.Collection(set,
				item => Assert.Equal(1, item),
				item => Assert.Equal(5, item),
				item => Assert.Equal(2, item),
				item => Assert.Equal(4, item),
				item => Assert.Equal(3, item));
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
			Assert.Contains(5, set);
			Assert.Collection(set,
				item => Assert.Equal(1, item),
				item => Assert.Equal(2, item),
				item => Assert.Equal(4, item),
				item => Assert.Equal(3, item),
				item => Assert.Equal(5, item));
		}

		[Fact]
		public void RemoveReturnsFalseOnElementNotFound() {
			var set = new OrderedSet<int>();
			Assert.False(set.Remove(3));
		}
	}
}
