﻿using ExtensionMethods;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		private static readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
		public static string Serialize(IPDXSerializable obj, string indent) {
			var sb = new StringBuilder();
			var type = obj.GetType();
			var mi = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
			sb.AppendLine("{");
			foreach (var member in mi) {
				if (member is not PropertyInfo && member is not FieldInfo) {
					continue;
				}
				if (member.IsNonSerialized()) {
					continue;
				}

				var fieldValue = member.GetValue(obj);

				var valueRepresentation = GetValueRepresentation(fieldValue, indent + '\t');
				if (valueRepresentation is null) {
					continue;
				}

				var name = member.GetName();
				sb.Append(indent).Append('\t').Append(name).Append(" = ").AppendLine(valueRepresentation);
			}
			sb.Append(indent).Append('}');
			return sb.ToString();
		}

		public static string Serialize(IPDXSerializable obj) {
			return Serialize(obj, string.Empty);
		}

		public static string? GetValueRepresentation(object? memberValue, string indent) {
			if (memberValue is null) {
				return null;
			}

			var sb = new StringBuilder();
			if (memberValue is string str) {
				sb.Append('\"').Append(str).Append('\"');
			} else if (memberValue is IDictionary dict) {
				sb.AppendLine("{");
				foreach (DictionaryEntry entry in dict) {
					sb.Append(indent).Append('\t').Append(entry.Key).Append(" = ").AppendLine(GetValueRepresentation(entry.Value, indent + '\t'));
				}
				sb.Append(indent).Append('}');
			} else if (memberValue is ICollection enumerable) {
				sb.Append("{ ");
				foreach (var entry in enumerable) {
					sb.Append(GetValueRepresentation(entry, indent)).Append(' ');
				}
				sb.Append('}');
			} else if (memberValue is IPDXSerializable serializableType) {
				sb.Append(serializableType.Serialize(indent));
			} else if (memberValue is bool boolValue) {
				sb.Append(new ParadoxBool(boolValue).YesOrNo);
			} else if (memberValue.GetType().IsValueType && memberValue is IFormattable formattable) { // for numbers
				sb.Append(formattable.ToString("G", cultureInfo));
			} else {
				throw new SerializationException($"Fields of type {memberValue.GetType()} are not yet supported by PDXSerializer!");
			}

			return sb.ToString();
		}
	}
}
