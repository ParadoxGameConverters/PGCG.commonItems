using System;

namespace commonItems.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SerializedName(string name) : Attribute {
	public string Name { get; } = name;
}