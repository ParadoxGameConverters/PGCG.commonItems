using commonItems.Serialization;
using System;

namespace commonItems;

[Obsolete("Use BufferedReader.GetBool instead.")]
public class PDXBool : IPDXSerializable {
	public bool Value { get; set; } = true;
	public string YesOrNo => Value ? "yes" : "no";
	public PDXBool() { }

	public PDXBool(bool value) {
		Value = value;
	}
	public PDXBool(string valueString) {
		Value = valueString switch {
			"yes" => true,
			"no" => false,
			_ => throw new FormatException("Text representation of ParadoxBool should be \"yes\" or \"no\"!")
		};
	}
	public PDXBool(BufferedReader reader) : this(reader.GetString()) { }
	public static implicit operator bool(PDXBool m) {
		return m.Value;
	}

	public string Serialize(string indent, bool withBraces) {
		return YesOrNo;
	}
}