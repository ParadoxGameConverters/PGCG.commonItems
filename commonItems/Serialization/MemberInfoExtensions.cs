﻿using System;
using System.Reflection;

namespace commonItems.Serialization; 

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

	public static string GetName(this MemberInfo memberInfo) {
		var attr = Attribute.GetCustomAttribute(memberInfo, typeof(SerializedName));
		return attr is null ? memberInfo.Name : ((SerializedName)attr).Name;
	}
}