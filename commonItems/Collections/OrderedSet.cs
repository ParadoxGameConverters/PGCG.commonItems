using System.Collections;
using System.Collections.Generic;

namespace commonItems.Collections;

// A set that preserves insertion order
// https://stackoverflow.com/a/17853085/10249243
public class OrderedSet<T> : ICollection<T> where T : notnull {
	private readonly IDictionary<T, LinkedListNode<T>> dictionary;
	private readonly LinkedList<T> linkedList;

	public OrderedSet() : this(EqualityComparer<T>.Default) { }

	public OrderedSet(IEqualityComparer<T> comparer) {
		dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
		linkedList = new();
	}

	public int Count => dictionary.Count;

	public virtual bool IsReadOnly => dictionary.IsReadOnly;

	void ICollection<T>.Add(T item) => Add(item);

	public bool Add(T item) {
		if (dictionary.ContainsKey(item)) return false;
		var node = linkedList.AddLast(item);
		dictionary.Add(item, node);
		return true;
	}

	public void Clear() {
		linkedList.Clear();
		dictionary.Clear();
	}

	public bool Remove(T item) {
		var found = dictionary.TryGetValue(item, out var node);
		if (!found) return false;
		dictionary.Remove(item);
		linkedList.Remove(node!);
		return true;
	}

	public IEnumerator<T> GetEnumerator() => linkedList.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public bool Contains(T item) => dictionary.ContainsKey(item);

	public void CopyTo(T[] array, int arrayIndex) => linkedList.CopyTo(array, arrayIndex);
}