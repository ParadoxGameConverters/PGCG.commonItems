using System;

namespace commonItems.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public class SerializeOnlyValue : Attribute { }