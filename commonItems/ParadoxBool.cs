using commonItems.Serialization;
using System;

namespace commonItems {
	public class ParadoxBool : IPDXSerializable {
		public bool Value { get; set; } = true;
		public string YesOrNo => Value ? "yes" : "no";
		public ParadoxBool() { }

		public ParadoxBool(bool value) {
			Value = value;
		}
		public ParadoxBool(string valueString) {
			Value = valueString switch {
				"yes" => true,
				"no" => false,
				_ => throw new FormatException("Text representation of ParadoxBool should be \"yes\" or \"no\"!")
			};
		}
		public ParadoxBool(BufferedReader reader) : this(reader.GetString()) { }
		public static implicit operator bool(ParadoxBool m) {
			return m.Value;
		}

		public string Serialize(string indent, bool withBraces) {
			return YesOrNo;
		}
	}
}
