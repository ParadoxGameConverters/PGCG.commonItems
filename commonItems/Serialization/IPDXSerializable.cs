using System;

namespace commonItems.Serialization; 

public interface IPDXSerializable {
	public string Serialize(string indent, bool withBraces) {
		throw new NotImplementedException("Serialize method must be implemented.");
	}
}