using commonItems.Collections;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests.Collections;

public sealed class ConcurrentIdObjectCollectionTests {
	private sealed class TestCharacter(int id, string culture = "roman") : IIdentifiable<int> {
		public int Id { get; } = id;
		public string Culture { get; } = culture;
	}

	[Fact]
	public void ObjectsCanBeAddedAndRetrieved() {
		var characters = new ConcurrentIdObjectCollection<int, TestCharacter>();

		characters.Add(new TestCharacter(1));
		characters.AddOrReplace(new TestCharacter(2, "greek"));

		Assert.Equal(2, characters.Count);
		Assert.Equal("roman", characters[1].Culture);
		Assert.Equal("greek", characters[2].Culture);
	}

	[Fact]
	public void TryAddSupportsParallelAddsWithUniqueKeys() {
		var characters = new ConcurrentIdObjectCollection<int, TestCharacter>();
		var failedAdds = 0;

		Parallel.For(0, 64, id => {
			if (!characters.TryAdd(new TestCharacter(id))) {
				Interlocked.Increment(ref failedAdds);
			}
		});

		Assert.Equal(0, failedAdds);
		Assert.Equal(64, characters.Count);
		for (var id = 0; id < 64; ++id) {
			Assert.True(characters.ContainsKey(id));
		}
	}
}