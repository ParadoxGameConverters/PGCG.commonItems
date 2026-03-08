using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace commonItems.Collections;

/// <summary>
/// A set that preserves insertion order
/// https://stackoverflow.com/a/17853085/10249243
/// </summary>
public class OrderedSet<T> : ISet<T>, IReadOnlyCollection<T> where T : notnull {
	private readonly Dictionary<T, LinkedListNode<T>> dictionary;
	private readonly LinkedList<T> linkedList;

	public OrderedSet() : this(EqualityComparer<T>.Default) { }

	public OrderedSet(IEnumerable<T> collection) : this() {
		foreach (var item in collection) {
			Add(item);
		}
	}
	
	public OrderedSet(IEqualityComparer<T> comparer) {
		dictionary = new(comparer);
		linkedList = [];
	}

	public int Count => dictionary.Count;

	public virtual bool IsReadOnly => false;

	void ICollection<T>.Add(T item) => Add(item);
	public void ExceptWith(IEnumerable<T> other) {
		foreach (var item in other) {
			Remove(item);
		}
	}

	public void IntersectWith(IEnumerable<T> other) {
		if (Count == 0) {
			return;
		}

		var otherSet = other as HashSet<T> ?? new HashSet<T>(other, dictionary.Comparer);
		var node = linkedList.First;
		while (node is not null) {
			var next = node.Next;
			if (!otherSet.Contains(node.Value)) {
				dictionary.Remove(node.Value);
				linkedList.Remove(node);
			}
			node = next;
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other) {
		var otherSet = other as HashSet<T> ?? new HashSet<T>(other, dictionary.Comparer);
		if (Count >= otherSet.Count) {
			return false;
		}

		foreach (var item in linkedList) {
			if (!otherSet.Contains(item)) {
				return false;
			}
		}

		return true;
	}

	public bool IsProperSupersetOf(IEnumerable<T> other) {
		var otherSet = other as HashSet<T> ?? new HashSet<T>(other, dictionary.Comparer);
		if (Count <= otherSet.Count) {
			return false;
		}

		foreach (var item in otherSet) {
			if (!Contains(item)) {
				return false;
			}
		}

		return true;
	}

	public bool IsSubsetOf(IEnumerable<T> other) {
		var otherSet = other as HashSet<T> ?? new HashSet<T>(other, dictionary.Comparer);
		if (Count > otherSet.Count) {
			return false;
		}

		foreach (var item in linkedList) {
			if (!otherSet.Contains(item)) {
				return false;
			}
		}

		return true;
	}

	public bool IsSupersetOf(IEnumerable<T> other) {
		foreach (var item in other) {
			if (!Contains(item)) {
				return false;
			}
		}

		return true;
	}

	public bool Overlaps(IEnumerable<T> other) {
		foreach (var item in other) {
			if (Contains(item)) {
				return true;
			}
		}

		return false;
	}

	public bool SetEquals(IEnumerable<T> other) {
		var otherSet = other as HashSet<T> ?? new HashSet<T>(other, dictionary.Comparer);
		if (Count != otherSet.Count) {
			return false;
		}

		foreach (var item in linkedList) {
			if (!otherSet.Contains(item)) {
				return false;
			}
		}

		return true;
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