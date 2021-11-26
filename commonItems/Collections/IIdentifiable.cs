namespace commonItems.Collections {
	public interface IIdentifiable<out TKey> where TKey : notnull {
		public TKey Id { get; }
	}
}