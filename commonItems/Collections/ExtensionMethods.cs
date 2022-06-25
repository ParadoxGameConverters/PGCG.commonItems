using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace commonItems.Collections; 

public static class ExtensionMethods {
	public static OrderedSet<T> ToOrderedSet<T>(this IEnumerable<T> enumerable) where T : notnull {
		return new OrderedSet<T>(enumerable);
	}
	
	public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate) {
		foreach (T item in collection.ToList().Where(predicate)) {
			collection.Remove(item);
		}
	}
}