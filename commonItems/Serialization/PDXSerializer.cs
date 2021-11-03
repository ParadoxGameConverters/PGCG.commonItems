using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		private static readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;

		public static string Serialize(object obj, string indent, bool withBraces = true) {
			var sb = new StringBuilder();
			if (obj is string str) {
				sb.Append('\"').Append(str).Append('\"');
			} else if (obj is IDictionary dict) {
				SerializeDictionary(dict, withBraces, sb, indent);
			} else if (obj is ICollection collection) {
				SerializeEnumerable(collection, withBraces, sb, indent);
			} else if (obj is IEnumerable enumerable) {
				SerializeEnumerable(enumerable, withBraces, sb, indent);
			} else if (IsKeyValuePair(obj)) {
				Type valueType = obj.GetType();
				object? kvpKey = valueType.GetProperty("Key")?.GetValue(obj, null);
				object? kvpValue = valueType.GetProperty("Value")?.GetValue(obj, null);
				if (kvpKey is not null && kvpValue is not null) {
					sb.Append(indent).Append('\t').Append(kvpKey)
						.Append(" = ")
						.Append(Serialize(kvpValue, indent + '\t'));
				}
			} else if (obj is IPDXSerializable serializableType) {
				sb.Append(serializableType.Serialize(indent, withBraces));
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

		private static void SerializeEnumerable(IEnumerable enumerable, bool withBraces, StringBuilder sb, string indent) {
			var serializedEntries = enumerable.Cast<object>().Select(e => Serialize(e, indent));
			if (withBraces) {
				sb.Append("{ ").AppendJoin(' ', serializedEntries).Append(" }");
			} else {
				sb.AppendJoin(Environment.NewLine, serializedEntries);
			}
		}

		private static void SerializeDictionary(IDictionary dictionary, bool withBraces, StringBuilder sb, string indent) {
			if (withBraces) {
				sb.AppendLine("{");
			}

			var internalIndent = "";
			if (withBraces) {
				internalIndent += '\t';
			}
			var serializedEntries = CastDict(dictionary).Where(e => e.Value is not null)
				.Select(
					e => indent + internalIndent + e.Key + " = " + Serialize(e.Value!, indent + '\t')
				);
			sb.AppendJoin(Environment.NewLine, serializedEntries);

			if (withBraces) {
				sb.AppendLine();
				sb.Append(indent).Append('}');
			}

			static IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary) {
				foreach (DictionaryEntry entry in dictionary) {
					yield return entry;
				}
			}
		}

		private static bool IsKeyValuePair(object obj) {
			Type type = obj.GetType();
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
		}
	}
}
