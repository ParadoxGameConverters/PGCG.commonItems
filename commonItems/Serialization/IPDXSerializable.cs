namespace commonItems.Serialization; 

public interface IPDXSerializable {
	public string Serialize(string indent, bool withBraces);
}