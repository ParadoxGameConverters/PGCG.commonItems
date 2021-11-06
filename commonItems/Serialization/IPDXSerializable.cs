using ExtensionMethods;
using System;
using System.Reflection;
using System.Text;

namespace commonItems.Serialization {
	public interface IPDXSerializable {
		public string Serialize(string indent, bool withBraces) {
			// default implementation: serialize members
			var sb = new StringBuilder();
			var type = GetType();
			var mi = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
			if (withBraces) {
				sb.AppendLine("{");
			}
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

				var internalIndent = "";
				if (withBraces) {
					internalIndent += '\t';
				}
				string lineRepresentation;
				if (Attribute.IsDefined(member, typeof(SerializeOnlyValue))) {
					lineRepresentation = PDXSerializer.Serialize(fieldValue, indent + internalIndent, false);
				} else {
					lineRepresentation = member.GetName() + '=' + PDXSerializer.Serialize(fieldValue, indent + '\t');
				}
				if (!string.IsNullOrWhiteSpace(lineRepresentation)) {
					sb.Append(indent).Append(internalIndent).AppendLine(lineRepresentation);
				}
			}
			if (withBraces) {
				sb.Append(indent).Append('}');
			}
			return sb.ToString();
		}
	}
}
