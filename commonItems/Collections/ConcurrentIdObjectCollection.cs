using System;
using System.Collections.Concurrent;

namespace commonItems.Collections;

public class ConcurrentIdObjectCollection<TKey, T> : IdObjectCollection<TKey, T> where TKey : IComparable where T : IIdentifiable<TKey> {
    public ConcurrentIdObjectCollection() : base(new ConcurrentDictionary<TKey, T>()) { }
}
