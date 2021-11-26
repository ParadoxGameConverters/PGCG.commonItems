using commonItems.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests.Collections {
	public class IdObjectCollectionTests {
		private class Character : IIdentifiable<string> {
			public string Id { get; }

			public Character(string id) {
				Id = id;
			}
		}
		private class Characters : IdObjectCollection<string,Character> { }

		[Fact]
		public void ObjectCanBeAdded() {
			var characters = new Characters();
			characters.Add(new Character("bob"));
			characters.Add(new Character("frank"));
			Assert.Collection();
		}
	}
}
