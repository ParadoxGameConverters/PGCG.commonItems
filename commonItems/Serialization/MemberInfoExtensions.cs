using commonItems.Serialization;
using System;
using System.Reflection;

namespace ExtensionMethods
{
	public static class MemberInfoExtensions {
		public static bool IsNonSerialized(this MemberInfo? mi) {
			return mi is not null && Attribute.IsDefined(mi, typeof(NonSerialized));
		}

		public static object? GetValue(this MemberInfo memberInfo, object forObject) {
			return memberInfo.MemberType switch {
				MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(forObject),
				MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(forObject),
				_ => throw new NotImplementedException()
			};
		}
	}
}