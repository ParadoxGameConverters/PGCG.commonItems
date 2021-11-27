using commonItems.Serialization;

namespace commonItems.Collections {
	public interface IIdentifiable {
		public abstract string GetIdString();
	}
	public interface IIdentifiable<out TKey> : IIdentifiable where TKey : notnull {
		[NonSerialized] public TKey Id { get; }
		string IIdentifiable.GetIdString() {
			return Id.ToString() ?? string.Empty;
		}
	}
}