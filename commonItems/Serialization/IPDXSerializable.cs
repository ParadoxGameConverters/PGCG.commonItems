using System;
using System.Reflection;
using System.Text;

namespace commonItems.Serialization; 

public interface IPDXSerializable {
	public string SerializeMembers(string indent) {
		var type = GetType();
		var mi = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
		var sb = new StringBuilder();
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
			
			string lineRepresentation;
			if (Attribute.IsDefined(member, typeof(SerializeOnlyValue))) {
				lineRepresentation = PDXSerializer.Serialize(fieldValue, indent, false);
			} else {
				lineRepresentation = $"{member.GetName()}={PDXSerializer.Serialize(fieldValue, indent)}";
			}
			if (!string.IsNullOrWhiteSpace(lineRepresentation)) {
				sb.Append(indent).AppendLine(lineRepresentation);
			}
		}

		return sb.ToString();
	}
	public string Serialize(string indent, bool withBraces) {
		// default implementation: serialize members
		var sb = new StringBuilder();
		if (withBraces) {
			sb.AppendLine("{");
		}
		sb.Append(SerializeMembers(withBraces ? indent + '\t' : indent));
		if (withBraces) {
			sb.Append(indent).Append('}');
		}
		return sb.ToString();
	}
}