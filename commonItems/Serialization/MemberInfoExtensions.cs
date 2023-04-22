using Apparatus.AOT.Reflection;
using System;
using System.Linq;

namespace commonItems.Serialization; 

public static class MemberInfoExtensions {
	public static bool IsNonSerialized(this IPropertyInfo? pi) {
		return pi is not null && pi.Attributes.Any(a => a is NonSerializedAttribute);
	}
}