using System;

namespace commonItems.Collections; 

public interface IIdentifiable {
	public abstract string GetIdString();
}
public interface IIdentifiable<TKey> : IIdentifiable, IComparable<IIdentifiable<TKey>> where TKey: IComparable {
	[Serialization.NonSerialized] public TKey Id { get; }
	string IIdentifiable.GetIdString() {
		return Id.ToString() ?? string.Empty;
	}

	int IComparable<IIdentifiable<TKey>>.CompareTo(IIdentifiable<TKey>? other) {
		if (ReferenceEquals(this, other)) {
			return 0;
		}

		if (ReferenceEquals(null, other)) {
			return 1;
		}

		return Id.CompareTo(other.Id);
	}
}