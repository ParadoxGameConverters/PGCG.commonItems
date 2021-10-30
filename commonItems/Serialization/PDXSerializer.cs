using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		public static string Serialize(IPDXSerializable obj) {
			var sb = new StringBuilder();
			var type = obj.GetType();
			var fieldsInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			Logger.Debug($"The fields of {type} are:");
			sb.AppendLine("{");
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

				sb.Append('\t').Append(field.Name).Append(" = ");

				switch (fieldValue) {
					case string str:
						// add double quotes to string
						sb.AppendLine(GetValueForSerializer(str));
						break;
					case Dictionary<Type, string> stringValueDict:
						sb.AppendLine("{");
						foreach (var (key, value) in stringValueDict) {
							sb.Append(key).Append(" = ").AppendLine(value);
						}
						sb.AppendLine("}");
						break;
					case IEnumerable<string> strEnumerable:
						IEnumerable<string> list = strEnumerable.ToList();
						var strEnumerableWithDoubleQuotes = list.Select(GetValueForSerializer);
						sb.AppendLine($"{{ {string.Join(", ", strEnumerableWithDoubleQuotes)} }}");
						break;
					case IPDXSerializable serializableType:
						sb.AppendLine(serializableType.Serialize());
						break;
					default:
						sb.AppendLine(fieldValue.ToString());
						break;
				}
			}
			sb.AppendLine("}");
			return sb.ToString();
		}

		private static string GetValueForSerializer(string value) {
			return $"\"{value}\"";
		}
	}
}
