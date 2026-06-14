using commonItems.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace commonItems.Serialization; 

public static class PDXSerializer {
	public static string Serialize(object obj, string indent) {
		return Serialize(obj, indent, true);
	}
	public static string Serialize(object obj, string indent, bool withBraces) {
		var sb = new StringBuilder();
		switch (obj) {
			case IPDXSerializable serializableType:
				sb.Append(serializableType.Serialize(indent, withBraces));
				break;
			case string str:
				sb.Append(str);
				break;
			case IDictionary dict:
				SerializeDictionary(dict, withBraces, sb, indent);
				break;
			case IEnumerable<IIdentifiable> idObjEnumerable:
				SerializeIdObjEnumerable(idObjEnumerable, sb, indent);
				break;
			case IEnumerable enumerable:
				SerializeEnumerable(enumerable, withBraces, sb, indent);
				break;
			case var o when IsKeyValuePair(o):
				SerializeKeyValuePair(o, sb, indent);
				break;
			case DictionaryEntry entry:
				SerializeDictionaryEntry(entry, sb, indent);
				break;
			case bool boolValue:
				sb.Append(boolValue ? "yes" : "no");
				break;
			case IFormattable formattable when obj.GetType().IsValueType:
				sb.Append(formattable.ToString("0.#####", CultureInfo.InvariantCulture));
				break;
			default:
				throw new SerializationException($"Objects of type {obj.GetType()} are not yet supported by PDXSerializer!");
		}

		return sb.ToString();
	}
	public static string Serialize(object obj) {
		return Serialize(obj, string.Empty);
	}

	private static void SerializeIdObjEnumerable(IEnumerable<IIdentifiable> enumerable, StringBuilder sb, string indent) {
		var dict = enumerable.ToDictionary(e => e.GetIdString(), e => e);
		SerializeDictionary(dict, false, sb, indent);
	}

	private static void SerializeEnumerable(IEnumerable enumerable, bool withBraces, StringBuilder sb, string indent) {
		if (withBraces) {
			sb.Append("{ ");
			foreach (var entry in enumerable) {
				sb.Append(Serialize(entry, indent)).Append(' ');
			}
			sb.Append('}');
		} else {
			var first = true;
			foreach (var entry in enumerable) {
				if (!first) {
					sb.Append(Environment.NewLine);
				}
				sb.Append(indent).Append(Serialize(entry, indent));
				first = false;
			}
		}
	}

	private static void SerializeDictionary(IDictionary dictionary, bool withBraces, StringBuilder sb, string indent) {
		if (withBraces) {
			sb.AppendLine("{");
		}

		var internalIndent = "";
		if (withBraces) {
			internalIndent += '\t';
			sb.Append(indent).Append(internalIndent);
		}
		var first = true;
		foreach (DictionaryEntry entry in dictionary) {
			if (entry.Value is null) {
				continue;
			}

			if (!first) {
				sb.Append(Environment.NewLine).Append(indent).Append(internalIndent);
			}
			sb.Append(Serialize(entry, indent + internalIndent));
			first = false;
		}

		if (withBraces) {
			sb.AppendLine();
			sb.Append(indent).Append('}');
		}
	}

	private static void SerializeKeyValuePair(object kvPair, StringBuilder sb, string indent) {
		Type valueType = kvPair.GetType();
		object? kvpKey = valueType.GetProperty("Key")?.GetValue(kvPair, null);
		object? kvpValue = valueType.GetProperty("Value")?.GetValue(kvPair, null);
		if (kvpKey is not null && kvpValue is not null) {
			sb.Append(kvpKey)
				.Append(" = ")
				.Append(Serialize(kvpValue, indent));
		}
	}
	private static void SerializeDictionaryEntry(DictionaryEntry entry, StringBuilder sb, string indent) {
		if (entry.Value is not null) {
			sb.Append(entry.Key)
				.Append(" = ")
				.Append(Serialize(entry.Value, indent));
		}
	}

	private static bool IsKeyValuePair(object obj) {
		Type type = obj.GetType();
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
	}
}