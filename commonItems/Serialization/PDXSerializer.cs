using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace commonItems.Serialization {
	public static class PDXSerializer {
		public static string Serialize(IPDXSerializable obj) {
			var sb = new StringBuilder();
			var type = obj.GetType();
			var fieldsInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			Logger.Debug($"The fields of {type} are:");
			foreach (var field in fieldsInfo) {
				Logger.Debug("\t" + field);

				sb.Append(field.Name).Append(" = ").Append(field.GetValue(obj)).AppendLine();
			}

			return sb.ToString();
		}
	}
}
