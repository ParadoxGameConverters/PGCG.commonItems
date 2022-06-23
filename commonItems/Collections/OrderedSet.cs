using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace commonItems.Collections;

/// <summary>
/// A set that preserves insertion order
/// https://stackoverflow.com/a/17853085/10249243
/// </summary>
public class OrderedSet<T> : ISet<T> where T : notnull {
	private readonly IDictionary<T, LinkedListNode<T>> dictionary;
	private readonly LinkedList<T> linkedList;

	public OrderedSet() : this(EqualityComparer<T>.Default) { }

	public OrderedSet(IEnumerable<T> collection) : this() {
		foreach (var item in collection) {
			Add(item);
		}
	}
	
	public OrderedSet(IEqualityComparer<T> comparer) {
		dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
		linkedList = new();
	}

	public int Count => dictionary.Count;

	public virtual bool IsReadOnly => dictionary.IsReadOnly;

	void ICollection<T>.Add(T item) => Add(item);
	public void ExceptWith(IEnumerable<T> other) {
		foreach (var item in other) {
			Remove(item);
		}
	}

	public void IntersectWith(IEnumerable<T> other) {
		IEnumerable<T> otherArray = other as T[] ?? other.ToArray();
		foreach (T item in this.ToList().Where(item => !otherArray.Contains(item))) {
			Remove(item);
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other) {
		return this.ToHashSet().IsProperSubsetOf(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other) {
		return this.ToHashSet().IsProperSupersetOf(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other) {
		return this.ToHashSet().IsSubsetOf(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other) {
		return this.ToHashSet().IsSupersetOf(other);
	}

	public bool Overlaps(IEnumerable<T> other) {
		return this.ToHashSet().Overlaps(other);
	}

	public bool SetEquals(IEnumerable<T> other) {
		return this.ToHashSet().SetEquals(other);
	}

	public void SymmetricExceptWith(IEnumerable<T> other) {
		var thisList = this.ToImmutableList();
		var otherSet = new OrderedSet<T>(other);
		foreach (var item in otherSet) {
			if (thisList.Contains(item)) {
				Remove(item);
			} else {
				Add(item);
			}
		}
	}

	public void UnionWith(IEnumerable<T> other) {
		foreach (var item in other) {
			Add(item);
		}
	}

	public bool Add(T item) {
		if (dictionary.ContainsKey(item)) {
			return false;
		}
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
		if (!found) {
			return false;
		}
		dictionary.Remove(item);
		linkedList.Remove(node!);
		return true;
	}

	public IEnumerator<T> GetEnumerator() => linkedList.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public bool Contains(T item) => dictionary.ContainsKey(item);

	public void CopyTo(T[] array, int arrayIndex) => linkedList.CopyTo(array, arrayIndex);
}