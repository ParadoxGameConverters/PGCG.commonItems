using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		private static readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;

		public static string Serialize(object obj, string indent) {
			var sb = new StringBuilder();
			if (obj is string str) {
				sb.Append('\"').Append(str).Append('\"');
			} else if (obj is IDictionary dict) {
				sb.AppendLine("{");
				foreach (DictionaryEntry entry in dict) {
					if (entry.Value is null) {
						continue;
					}
					sb.Append(indent).Append('\t').Append(entry.Key)
						.Append(" = ")
						.AppendLine(Serialize(entry.Value, indent + '\t'));
				}
				sb.Append(indent).Append('}');
			} else if (obj is ICollection collection) {
				SerializeEnumerable(collection, sb, indent);
			} else if (obj is IEnumerable enumerable) {
				SerializeEnumerable(enumerable, sb, indent);
			} else if (obj is IPDXSerializable serializableType) {
				sb.Append(serializableType.Serialize(indent));
			} else if (obj is bool boolValue) {
				sb.Append(new ParadoxBool(boolValue).YesOrNo);
			} else if (obj.GetType().IsValueType && obj is IFormattable formattable) { // for numbers
				sb.Append(formattable.ToString("G", cultureInfo));
			} else {
				throw new SerializationException($"Objects of type {obj.GetType()} are not yet supported by PDXSerializer!");
			}

			return sb.ToString();
		}
		public static string Serialize(object obj) {
			return Serialize(obj, string.Empty);
		}

		private static void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb, string indent) {
			sb.Append("{ ");
			foreach (var entry in enumerable) {
				sb.Append(Serialize(entry, indent)).Append(' ');
			}
			sb.Append('}');
		}
	}
}
