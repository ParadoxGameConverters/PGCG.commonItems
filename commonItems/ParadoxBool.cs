using System;

namespace commonItems {
	public class ParadoxBool {
		public bool Value { get; set; } = true;
		public string YesOrNo {
			get {
				if (Value) {
					return "yes";
				}
				return "no";
			}
		}
		public ParadoxBool() { }
		public ParadoxBool(string valueString) {
			if (valueString == "yes") {
				Value = true;
			} else if (valueString == "no") {
				Value = false;
			} else {
				throw new FormatException("Text representation of ParadoxBool should be \"yes\" or \"no\"!");
			}
		}
		public ParadoxBool(BufferedReader reader) : this(ParserHelpers.GetString(reader)) { }
		public static implicit operator bool(ParadoxBool m) {
			return m?.Value == true;
		}
	}
}
