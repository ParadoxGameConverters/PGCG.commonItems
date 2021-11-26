using commonItems.Collections;
using Xunit;

namespace commonItems.UnitTests.Collections {
	public class IdObjectCollectionTests {
		private class Character : IIdentifiable<string> {
			public string Id { get; }

			public Character(string id) {
				Id = id;
			}
		}
		private class Characters : IdObjectCollection<string, Character> { }

		[Fact]
		public void ObjectsCanBeAddedAndRemoved() {
			var characters = new Characters();

			characters.Add(new Character("bob"));
			characters.Add(new Character("frank"));
			Assert.Collection(characters,
				ch1 => Assert.Equal("bob", ch1.Id),
				ch2 => Assert.Equal("frank", ch2.Id)
			);
			Assert.True(characters.ContainsKey("bob"));
			Assert.True(characters.ContainsKey("frank"));
			Assert.Equal(2, characters.Count);

			characters.Remove("bob");
			Assert.Collection(characters,
				ch2 => Assert.Equal("frank", ch2.Id)
			);
			Assert.False(characters.ContainsKey("bob"));
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
			Assert.Equal(1, characters.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
		}

		[Fact]
		public void ObjectsCanBeAccessedWithIndexer() {
			var characters = new Characters();
			characters.Add(new Character("bob"));
			Assert.Equal("bob", characters["bob"].Id);
		}
		[Fact]
		public void ObjectsCanBeAccessedWithTryGetValue() {
			var characters = new Characters();
			characters.Add(new Character("bob"));

			Assert.True(characters.TryGetValue("bob", out var bob));
			Assert.Equal("bob", bob.Id);
		}

		[Fact]
		public void ObjectsCanBeAddedWithTryAdd() {
			var characters = new Characters();
			characters.Add(new Character("bob"));

			Assert.False(characters.TryAdd(new Character("bob")));
			Assert.True(characters.TryAdd(new Character("frank")));
		}
	}
}
