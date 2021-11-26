using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace commonItems.Collections {
	public class IdObjectCollection<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull where TValue : IIdentifiable<TKey> {
		private readonly Dictionary<TKey, TValue> dict = new();

		public TValue this[TKey key] => dict[key];
		public IEnumerable<TKey> Keys => dict.Keys;
		public IEnumerable<TValue> Values => dict.Values;
		public int Count => dict.Count;
		public bool ContainsKey(TKey key) {
			return dict.ContainsKey(key);
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return dict.GetEnumerator();
		}
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
			return dict.TryGetValue(key, out value);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return dict.GetEnumerator();
		}

		public virtual void Add(TValue obj) {
			dict.Add(obj.Id, obj);
		}

		public virtual void Remove(TKey key) {
			dict.Remove(key);
		}
	}
}
