using System;
using System.Collections.Generic;

namespace commonItems.Collections; 

public static class ExtensionMethods {
	public static OrderedSet<T> ToOrderedSet<T>(this IEnumerable<T> enumerable) where T : notnull {
		return new(enumerable);
	}
	
	public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate) {
		if (collection is List<T> list) {
			list.RemoveAll(item => predicate(item));
			return;
		}

		var snapshot = new List<T>(collection);
		foreach (T item in snapshot) {
			if (predicate(item)) {
				collection.Remove(item);
			}
		}
	}
}