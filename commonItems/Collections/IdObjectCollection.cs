using commonItems.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace commonItems.Collections {
	public class IdObjectCollection<TKey, T> : IPDXSerializable, IReadOnlyCollection<T> where TKey : notnull where T : IIdentifiable<TKey> {
		protected readonly Dictionary<TKey, T> dict = new();

		public T this[TKey key] => dict[key];
		public int Count => dict.Count;
		public bool ContainsKey(TKey key) => dict.ContainsKey(key);
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value) => dict.TryGetValue(key, out value);

		public IEnumerator<T> GetEnumerator() => dict.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public virtual bool TryAdd(T obj) => dict.TryAdd(obj.Id, obj);
		public virtual void Add(T obj) => dict.Add(obj.Id, obj);
		public virtual void Remove(TKey key) => dict.Remove(key);

		public string Serialize(string indent, bool withBraces) {
			return PDXSerializer.Serialize(dict, indent, withBraces);
		}
	}
}
