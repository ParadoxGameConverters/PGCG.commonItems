using commonItems.Collections;
using Xunit;

namespace commonItems.UnitTests.Collections;

public sealed class IdObjectCollectionTests {
	private sealed class Character(string id) : IIdentifiable<string> {
		public string Id { get; } = id;
		public string Culture { get; init; } = "roman";
	}
	private sealed class Characters : IdObjectCollection<string, Character>;

	[Fact]
	public void ObjectsCanBeAddedAndRemoved() {
		// ReSharper disable once UseObjectOrCollectionInitializer
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
		var ch2 = Assert.Single(characters);
		Assert.Equal("frank", ch2.Id);
		Assert.False(characters.ContainsKey("bob"));
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
		Assert.Equal(1, characters.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
	}

	[Fact]
	public void ObjectsCanBeAccessedWithIndexer() {
		var characters = new Characters { new Character("bob") };
		Assert.Equal("bob", characters["bob"].Id);
	}
	[Fact]
	public void ObjectsCanBeAccessedWithTryGetValue() {
		var characters = new Characters { new Character("bob") };

		Assert.True(characters.TryGetValue("bob", out var bob));
		Assert.Equal("bob", bob.Id);
	}

	[Fact]
	public void ObjectsCanBeAddedWithTryAdd() {
		var characters = new Characters {
			new Character("bob"),
		};

		Assert.False(characters.TryAdd(new Character("bob")));
		Assert.True(characters.TryAdd(new Character("frank")));
	}

	[Fact]
	public void ObjectsCanBeReplacedWithAddOrReplace() {
		var characters = new Characters();

		var initialChar = new Character("1");
		characters.AddOrReplace(initialChar);
		Assert.Equal("roman", characters["1"].Culture);

		var charWithSameId = new Character("1") { Culture = "greek" };
		characters.AddOrReplace(charWithSameId);
		Assert.Equal("greek", characters["1"].Culture);
	}

	[Fact]
	public void CollectionCanBeCleared() {
		var characters = new Characters { new Character("bob"), new Character("frank") };
		Assert.Equal(2, characters.Count);

		characters.Clear();
		Assert.Empty(characters);
	}

    private sealed class TestObj(int id, string name) : IIdentifiable<int> {
		public int Id { get; } = id;
		public string Name { get; } = name;
	}

    [Fact]
    public void RemoveAll_RemovesMatchingObjects_ReturnsCorrectCount() {
        var collection = new IdObjectCollection<int, TestObj> {
			new TestObj(1, "A"),
			new TestObj(2, "B"),
			new TestObj(3, "C"),
		};

        int removed = collection.RemoveAll(obj => obj.Name == "B" || obj.Id == 3);

        Assert.Equal(2, removed);
        Assert.False(collection.ContainsKey(2));
        Assert.False(collection.ContainsKey(3));
        Assert.True(collection.ContainsKey(1));
    }

    [Fact]
    public void RemoveAll_NoObjectsMatch_RemovesNone() {
        var collection = new IdObjectCollection<int, TestObj> {
			new TestObj(1, "A"),
			new TestObj(2, "B"),
		};

        int removed = collection.RemoveAll(obj => obj.Name == "Z");

        Assert.Equal(0, removed);
        Assert.True(collection.ContainsKey(1));
        Assert.True(collection.ContainsKey(2));
    }

    [Fact]
    public void RemoveAll_AllObjectsMatch_RemovesAll() {
        var collection = new IdObjectCollection<int, TestObj> {
			new TestObj(1, "A"),
			new TestObj(2, "B"),
		};

        int removed = collection.RemoveAll(obj => true);

        Assert.Equal(2, removed);
        Assert.Empty(collection);
    }

    [Fact]
    public void RemoveAll_EmptyCollection_ReturnsZero() {
        var collection = new IdObjectCollection<int, TestObj>();

        int removed = collection.RemoveAll(obj => true);

        Assert.Equal(0, removed);
        Assert.Empty(collection);
    }
}