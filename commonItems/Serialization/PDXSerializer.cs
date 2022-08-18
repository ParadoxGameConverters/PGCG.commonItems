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
		if (obj is IPDXSerializable serializableType) {
			sb.Append(serializableType.Serialize(indent, withBraces));
		} else if (obj is string str) {
			SerializeString(str, sb);
		} else if (obj is IDictionary dict) {
			SerializeDictionary(dict, withBraces, sb, indent);
		} else if (obj is IEnumerable<IIdentifiable> idObjEnumerable) {
			SerializeIdObjEnumerable(idObjEnumerable, sb, indent);
		} else if (obj is IEnumerable enumerable) {
			SerializeEnumerable(enumerable, withBraces, sb, indent);
		} else if (IsKeyValuePair(obj)) {
			SerializeKeyValuePair(obj, sb, indent);
		} else if (obj is DictionaryEntry entry) {
			SerializeDictionaryEntry(entry, sb, indent);
		} else if (obj is bool boolValue) {
			sb.Append(boolValue ? "yes" : "no");
		} else if (obj.GetType().IsValueType && obj is IFormattable formattable) { // for numbers
			sb.Append(formattable.ToString("0.######", CultureInfo.InvariantCulture));
		} else {
			throw new SerializationException($"Objects of type {obj.GetType()} are not yet supported by PDXSerializer!");
		}

		return sb.ToString();
	}
	public static string Serialize(object obj) {
		return Serialize(obj, string.Empty);
	}

	private static bool StringIsQuoted(string str) {
		return str.StartsWith('"') && str.EndsWith('"');
	}
	private static void SerializeString(string str, StringBuilder sb) {
		if (StringIsQuoted(str)) {
			sb.Append(str);
		} else {
			sb.Append('\"').Append(str).Append('\"');
		}
	}
	
	private static void SerializeIdObjEnumerable(IEnumerable<IIdentifiable> enumerable, StringBuilder sb, string indent) {
		var dict = enumerable.ToDictionary(e => e.GetIdString(), e => e);
		SerializeDictionary(dict, false, sb, indent);
	}

	private static void SerializeEnumerable(IEnumerable enumerable, bool withBraces, StringBuilder sb, string indent) {
		var serializedEntries = enumerable.Cast<object>().Select(e => Serialize(e, indent));
		if (withBraces) {
			sb.Append("{ ");
			foreach (var entry in serializedEntries) {
				sb.Append(entry).Append(' ');
			}
			sb.Append('}');
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
			sb.Append(indent).Append(internalIndent);
		}
		var notNullEntries = CastDict(dictionary).Where(e => e.Value is not null);
		var serializedEntries = notNullEntries.Select(e => Serialize(e, indent + internalIndent));
		sb.AppendJoin(Environment.NewLine + indent + internalIndent, serializedEntries);

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
	private static void SerializeKeyValuePair(object kvPair, StringBuilder sb, string indent) {
		Type valueType = kvPair.GetType();
		object? kvpKey = valueType.GetProperty("Key")?.GetValue(kvPair, null);
		object? kvpValue = valueType.GetProperty("Value")?.GetValue(kvPair, null);
		if (kvpKey is not null && kvpValue is not null) {
			sb.Append(kvpKey)
				.Append('=')
				.Append(Serialize(kvpValue, indent));
		}
	}
	private static void SerializeDictionaryEntry(DictionaryEntry entry, StringBuilder sb, string indent) {
		if (entry.Value is not null) {
			sb.Append(entry.Key)
				.Append('=')
				.Append(Serialize(entry.Value, indent));
		}
	}

	private static bool IsKeyValuePair(object obj) {
		Type type = obj.GetType();
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
	}
}