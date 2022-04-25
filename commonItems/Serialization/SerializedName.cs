using System;

namespace commonItems.Serialization; 

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SerializedName : Attribute {
	public string Name { get; }

	public SerializedName(string name) {
		Name = name;
	}
}