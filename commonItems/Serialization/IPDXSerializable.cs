using commonItems.Serialization.commonItems.Serialization;
using ExtensionMethods;
using System;
using System.Reflection;
using System.Text;

namespace commonItems.Serialization {
	public interface IPDXSerializable {
		public string Serialize(string indent) {
			// default implementation: serialize members
			var sb = new StringBuilder();
			var type = GetType();
			var mi = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
			sb.AppendLine("{");
			foreach (var member in mi) {
				if (member is not PropertyInfo && member is not FieldInfo) {
					continue;
				}
				if (member.IsNonSerialized()) {
					continue;
				}
				var fieldValue = member.GetValue(this);
				if (fieldValue is null) {
					continue;
				}

				var valueRepresentation = PDXSerializer.Serialize(fieldValue, indent + '\t');
				if (!Attribute.IsDefined(member, typeof(SerializeOnlyValue))) {
					var name = member.GetName();
					sb.Append(indent).Append('\t').Append(name).Append(" = ");
				}
				sb.AppendLine(valueRepresentation);
			}
			sb.Append(indent).Append('}');
			return sb.ToString();
		}
	}
}
