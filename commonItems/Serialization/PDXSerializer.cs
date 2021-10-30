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
					Logger.Debug("Skipping NonSerialized " + field.Name);
					continue;
				}
				Logger.Debug("\t" + field);// TODO: REMOVE DEBUG LOGGING

				var fieldValue = field.GetValue(obj);
				if (fieldValue is null) {
					continue;
				}

				sb.Append('\t').Append(field.Name).Append(" = ");

				if (field.FieldType == typeof(string)) {
					// add double quotes to string
					var str = fieldValue as string;
					sb.AppendLine(GetValueForSerializer(str ?? string.Empty));
				} else if (field.FieldType == typeof(Dictionary<Type, string>)) {
					if (fieldValue is not Dictionary<Type, string> dict) {
						Logger.Warn($"PDXSerializer: skipping outputting of {field.Name} in {type}!");
						continue;
					}

					sb.AppendLine("{");
					foreach (var (key, value) in dict) {
						sb.Append(key).Append(" = ").AppendLine(value);
					}
					sb.AppendLine("}");
				} else switch (fieldValue) {
						case IEnumerable<string> strEnumerable: {
								IEnumerable<string> list = strEnumerable.ToList();
								var strEnumerableWithDoubleQuotes = list.Select(GetValueForSerializer);
								sb.AppendLine($"{{ {string.Join(", ", strEnumerableWithDoubleQuotes)} }}");
								break;
							}
						case ParadoxBool pdxBool:
							sb.AppendLine(pdxBool.YesOrNo);
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
