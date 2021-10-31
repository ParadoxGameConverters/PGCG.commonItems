using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		private static CultureInfo cultureInfo = CultureInfo.InvariantCulture;
		public static string Serialize(IPDXSerializable obj, string indent = "") {
			var sb = new StringBuilder();
			var type = obj.GetType();
			var fieldsInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			Logger.Debug($"The fields of {type} are:");
			sb.Append(indent).AppendLine("{");
			foreach (var field in fieldsInfo) {
				if (field.IsNotSerialized) {
					Logger.Debug("Skipping NonSerialized " + field.Name); // TODO: REMOVE DEBUG LOGGING
					continue;
				}
				Logger.Debug("\t" + field); // TODO: REMOVE DEBUG LOGGING

				var fieldValue = field.GetValue(obj);
				if (fieldValue is null) {
					continue;
				}

				var valueRepresentation = GetValueRepresentation(field, fieldValue, indent);
				if (valueRepresentation is null) {
					continue;
				}
				sb.Append(indent).Append('\t').Append(field.Name).Append(" = ").AppendLine(valueRepresentation);
			}
			sb.Append(indent).AppendLine("}");
			return sb.ToString();
		}

		private static string GetStringRepresentation(string value) {
			return $"\"{value}\"";
		}

		private static string? GetValueRepresentation(MemberInfo field, object fieldValue, string indent) {
			var sb = new StringBuilder();
			if (fieldValue is string str) {
				sb.Append(indent).Append(GetStringRepresentation(str));
			} else if (fieldValue is IDictionary<string, string> stringValueDict) {
				sb.Append(indent).AppendLine("{");
				foreach (var (key, value) in stringValueDict!) {
					sb.Append(indent).Append("\t\t").Append(key).Append(" = ").AppendLine(GetStringRepresentation(value));
				}
				sb.Append(indent).Append("\t}");
			} else if (fieldValue is IEnumerable<string> strEnumerable) {
				IEnumerable<string> list = strEnumerable.ToList();
				var strEnumerableWithDoubleQuotes = list.Select(GetStringRepresentation);
				sb.Append(indent).Append("{ ").AppendJoin(", ", strEnumerableWithDoubleQuotes).Append(" }");
			} else if (fieldValue is IPDXSerializable serializableType) {
				sb.Append(indent).Append(serializableType.Serialize());
			} else if (fieldValue is bool boolValue) {
				var pdxBool = new ParadoxBool(boolValue);
				sb.Append(pdxBool.YesOrNo);
			} else if (fieldValue.GetType().IsValueType && fieldValue is IFormattable fp) { // for numbers
				sb.Append(fp.ToString("G", cultureInfo));
			} else {
				Logger.Error($"Field {field.Name} of type {fieldValue.GetType()} is not yet supported by PDXSerializer!");
				return null;
			}

			return sb.ToString();
		}
	}
}
