using commonItems.Serialization;

namespace commonItems.Collections {
	public interface IIdentifiable<out TKey> where TKey : notnull {
		[NonSerialized] public TKey Id { get; }
	}
}